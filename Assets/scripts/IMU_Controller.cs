using System;
using System.Threading;
using UnityEngine;
using ServerReceiver;
//using UnityEngine.UI;
//using System.Collections;
//using System.Threading.Tasks;
using System.IO;
using Codice.Client.BaseCommands.Merge;
using log4net.DateFormatter;
//using Codice.Client.Common;

public class IMU_Controller : MonoBehaviour
{
    //Instance is used to ensure we only have a single version of this script at a time (see Awake())
    public static IMU_Controller Instance { get; private set; }

    public static Server _bluetoothobj;
    public bool isBluetooth = false;
    private string _lineread1;
    private string[] _splitter1;
    private string[] storeSplitter1 = new string[30];


    //csv
    private FileStream streamFile;
    private StreamWriter writeStream;
    private FileStream streamFile_threshold;
    private StreamWriter writeStream_threshold;
    //private string timeStamp;
    //private string timeStampPrint;
    //private DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    //private DateTime dateTimeNow;
    //private TimeSpan timeElapsed;
    //private double timeElapsedInseconds;
    private float Timecsv = 0;

    private string sessionTimeStamp;

    //----------------threshold mode related variables----------------------
    [SerializeField] public bool thresholdMode = false;
    //This would be thershold values for full range of motion.
    public Vector3 ThresholdMin = new Vector3(-70, -70, -50);
    public Vector3 ThresholdMax = new Vector3(70, 55, 40);

    //Used in threshold assign mode
    private float tempXMin;
    private float tempXMax;
    private float tempYMin;
    private float tempYMax;
    private float tempZMin;
    private float tempZMax;

    //When assigning thresholds via user input we need a non zero minimum value.
    //new thresholds will only be assigned if they are greater than this value
    //this is because the user could asccidently assign very small values
    //which would make the player character move irratically (sort of like super high sensitivity)
    private Vector3 ThresholdMinViable = new Vector3(-30, -30, -20);
    private Vector3 ThresholdMaxViable = new Vector3(30, 30, 20);
    //-----------------------end treshold mode varbs---------------------


    //--------------------IMU sensor values--------------------------------
    //euler is the hand sensor
    public  Vector3 euler = new Vector3(0, 0, 0);
    //euler2 is the wrist sensor
    public Vector3 euler2 = new Vector3(0, 0, 0);

    //these values become hard with respect to wrist
    //dual sensors = hand sensor - wrist sensor
    //although since euler2 defaults too 0,0,0 you can use the single hand sensor
    //with these values if you wanted. (i've seperated them for clarity)
    public float dualSensorX = 0;
    public float dualSensorY = 0;
    public float dualSensorZ = 0;
    public bool dualSensorMode = true;

    public Quaternion[] quat = new Quaternion[5];
    //delimiter for imu sensor which comes in as a string via another script.
    private char[] _delimiter = { 'R', 'r', 'o', 'l', 'P', 'p', 'i', 't', 'c', 'h', 'a', 'w', 'Y', 'x', 'y', 'z', ',', ':', '{', '}', '[', ']', '\"', ' ', '|' };
    //---------------------end imu varbs -----------------------------------

    //for checking that the strech is within x degrees of a threshold
    [SerializeField] public float compareEulerVsThreshold = 10f;
    public bool currentlyCloseToThresholdFlag = false;

    //very important.
    //To ensure that this script (attached to a game object) sticks around between scene changes.
    //also prevents duplicates from spawning.
    //this is important for this script since its the entry point for the bluetooth.
    //We don't want multiple open ports (unity will just crash, or infinite load times)
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _bluetoothobj = new Server();
        Debug.Log("IUM_Controller Script Online");
        Initialise();
    }

    public void Initialise()
    {
        //----------csv for euler value recording durring session -------------
        sessionTimeStamp = setTimeStamp();
        streamFile = new FileStream("C:\\Users\\danie\\Documents\\Ski_Game_Thesis\\log\\" + sessionTimeStamp + ".csv", FileMode.OpenOrCreate);//, FileAccess.ReadWrite, FileShare.None);
        writeStream = new StreamWriter(streamFile);
        writeStream.WriteLine("delta time, euler x , euler y , euler z");// , quat x , quat y , quat z , quat w");

        //---------threshold csv report --------------------
        streamFile_threshold = new FileStream("C:\\Users\\danie\\Documents\\Ski_Game_Thesis\\log\\thresholds\\" + "Thresholds" + ".csv", FileMode.OpenOrCreate);//, FileAccess.ReadWrite, FileShare.None);
        writeStream_threshold = new StreamWriter(streamFile_threshold);
        writeStream.WriteLine("Min x, Max x, Min y, Max y, Min z, Max z");

        //start the bluetooth server
        Debug.Log("Created");
        _bluetoothobj.Start();
        Debug.Log("Starting Bluetooth");

        // Wait to ensure bluetooth communication begins before calibration
        Thread.Sleep(200);
        for (int i = 0; i < storeSplitter1.Length; i++)
        {
            storeSplitter1[i] = "0";
        }
        isBluetooth = true;
    }

    void Update()
    {
        if (isBluetooth == true)
        {   //--------------IMUdata--------------------
            // Receive from Bluetooth, each string it is assigned to is for each bluetooth 
            _lineread1 = _bluetoothobj.GetSensor1();

            _splitter1 = _lineread1.Split(_delimiter, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < _splitter1.Length; i++)
            {
                storeSplitter1[i] = _splitter1[i];
            }

            //Sensor data comes in as a single long string for two IMUs

            //sensor 1 data
            euler.x = float.Parse(storeSplitter1[0]);
            euler.y = float.Parse(storeSplitter1[1]);
            euler.z = float.Parse(storeSplitter1[2]);

            //sensor 2 data
            euler2.x = float.Parse(storeSplitter1[3]);
            euler2.y = float.Parse(storeSplitter1[4]);
            euler2.z = float.Parse(storeSplitter1[5]);

            //Note: as euler2 defults to 0,0,0, dualSensor also works as a single wrist sensor
            dualSensorX = euler.x - euler2.x;
            dualSensorY = euler.y - euler2.y;
            dualSensorZ = euler.z - euler2.z;

            //You must press escape before exiting gameplay
            //otherwise unity will attempt to open a new bluetooth server when one already exists.
            //i would fix this but it will likely all be restructued when all the games are compiled
            //i would suggest putting all bluetooth in the framework (not the game)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopBluetooth();
            }

            //pressing and holding 'T' lets the user set threshold values
            if (Input.GetKeyDown(KeyCode.T))
            {
                thresholdMode = true;
            }
            if (Input.GetKeyUp(KeyCode.T))
            {
                thresholdMode = false;
                print("exiting threshold recording mode");
            }

            //pressing "Y" dumps the values to a csv so we can see threshold changes over multiple days/weeks
            //Also applys those thresholds in game
            //needs some simple changes before deployment
            //      1) increment an add a new timestamped item to the csv (currently overwipes first line)
            //      2) reading in the last recorded threshold value would be a nice strech goal
            if (Input.GetKeyDown(KeyCode.Y))
            {
                dumpThresholdCSV();
                applyThresholds();
            }
            //print thresholds (debugging)
            if (Input.GetKeyDown(KeyCode.U))
            {
                print("Mins: " + ThresholdMin);
                print("Maxs: " + ThresholdMax);
            }

            //coverts to quaternion (Unused but may be useful when combining the games)
            if (dualSensorMode) {ConvertEulerToQuaternion(dualSensorX, dualSensorY, dualSensorZ);}
            else { ConvertEulerToQuaternion(euler.x, euler.y, euler.z);}

            //Record euler values to CSV file each frame.
            recordEulerValuesToCSV();
        }

        //enter threshold recording mode
        if (thresholdMode)
        {
            //assigns temp values with users thresholds
            //however these values aren't assigned until
            //applyThresholds() is run. (by pressing 'Y')
            thresholdValueAssignment();
        }

        //project strech goal (lol)
        //close to threshold flag
        //strech conditions are met when player is in the correct position AND the user is close to a threshold.
        //currentlyCloseToThresholdCheck();
    }
    

    //currently unused by might be good to implement in the future. Could be part of higher level framework though
    public void currentlyCloseToThresholdCheck()
    {
        //check if euler value is within x% of threshold value
        if (MathF.Abs(euler.x - ThresholdMax.x) <= compareEulerVsThreshold ||
            MathF.Abs(euler.x - ThresholdMin.x) <= compareEulerVsThreshold)// ||
            /*percentage(euler.y, ThresholdMax.y) <= comparePercentageForThresholds ||
            percentage(euler.y, ThresholdMin.y) <= comparePercentageForThresholds ||
            percentage(euler.z, ThresholdMax.z) <= comparePercentageForThresholds ||
            percentage(euler.z, ThresholdMin.z) <= comparePercentageForThresholds)
        */{
            currentlyCloseToThresholdFlag = true;
            print("flag: " + currentlyCloseToThresholdFlag + "xMin Diff: "+ MathF.Abs(euler.x - ThresholdMin.x)+
                "xMax Diff: "+ MathF.Abs(euler.x - ThresholdMax.x));
        }
    }

    private string setTimeStamp()
    {
        string year;
        string month;
        string date;
        string hour;
        string minute;
        string second;

        year = DateTime.Now.Year.ToString("0000");
        month = DateTime.Now.Month.ToString("00");
        date = DateTime.Now.Day.ToString("00");
        hour = DateTime.Now.Hour.ToString("00");
        minute = DateTime.Now.Minute.ToString("00");
        second = DateTime.Now.Second.ToString("00");

        return year + "-" + month + "-" + date + "-" + hour + "-" + minute + "-" + second;
    }

    public void dumpThresholdCSV()
    {
        writeStream_threshold.WriteLine(ThresholdMin.x.ToString() + "," + ThresholdMax.x.ToString() + "," + ThresholdMin.y.ToString()
                            + "," + ThresholdMax.y.ToString() + "," + ThresholdMin.z.ToString() + "," + ThresholdMax.z.ToString());
        writeStream_threshold.Flush();
        writeStream_threshold.WriteLine(System.Environment.NewLine);
        writeStream_threshold.Close();
        print("Thresholds logfile created");
        print("Mins: " + ThresholdMin);
        print("Maxs: " + ThresholdMax);
    }

    public void applyThresholds()
    {
        
        ThresholdMin = new Vector3(tempXMin, tempYMin, tempZMin);
        ThresholdMax = new Vector3(tempXMax, tempYMax, tempZMax);
        print("Mins: " + ThresholdMin);
        print("Maxs: " + ThresholdMax);
    }
    /*
    public void StartBluetooth()
    {
        //IMUdata
        // Start receiving from bluetooth of both sensors instead of IP 
        Debug.Log("Starting");
        Debug.Log("Starting Bluetooth");
        // _bluetoothobj.Calibrate();

        // Wait to ensure bluetooth communication begins before calibration
        //Thread.Sleep(300);

        //set array element to "0"
        for (int i = 0; i < storeSplitter1.Length; i++)
        {
            storeSplitter1[i] = "0";
        }

        isBluetooth = true;
    }
    */
    public void ConvertEulerToQuaternion(float roll, float pitch, float yaw)
    {

        //convert angle to radians
        float radRoll = roll * Mathf.PI / 180;
        float radPitch = pitch * Mathf.PI / 180;
        float radYaw = yaw * Mathf.PI / 180;
        // Assuming the angles are in radians.
        float c1 = Mathf.Cos(radRoll/ 2);
        float s1 = Mathf.Sin(radRoll / 2);
        float c2 = Mathf.Cos(radPitch / 2);
        float s2 = Mathf.Sin(radPitch / 2);
        float c3 = Mathf.Cos(radYaw / 2);
        float s3 = Mathf.Sin(radYaw / 2);
        float c1c2 = c1 * c2;
        float s1s2 = s1 * s2;
        quat[0].x = c1c2 * s3 + s1s2 * c3;
        quat[0].y = s1 * c2 * c3 + c1 * s2 * s3;
        quat[0].z = c1 * s2 * c3 - s1 * c2 * s3;
        quat[0].w = c1c2 * c3 - s1s2 * s3;
        
    }

    public void thresholdValueAssignment()
    {
        print("threshold recording mode activated");
        //if value if less than current value overwrite current value
        if (euler.x < tempXMin && euler.x > ThresholdMinViable.x) { tempXMin = euler.x; } else { tempXMin = ThresholdMinViable.x; }
        if (euler.x > tempXMax && euler.x > ThresholdMaxViable.x) { tempXMax = euler.x; } else { tempXMax = ThresholdMaxViable.x; }
        if (euler.y < tempYMin && euler.y > ThresholdMinViable.y) { tempYMin = euler.y; } else { tempYMin = ThresholdMinViable.y; }
        if (euler.y > tempYMax && euler.y > ThresholdMaxViable.y) { tempYMax = euler.y; } else { tempYMax = ThresholdMaxViable.y; }
        if (euler.z < tempZMin && euler.z > ThresholdMinViable.z) { tempZMin = euler.z; } else { tempZMin = ThresholdMinViable.z; }
        if (euler.z > tempZMax && euler.z > ThresholdMaxViable.z) { tempZMax = euler.z; } else { tempZMax = ThresholdMaxViable.z; }
    }

    public void recordEulerValuesToCSV()
    {
        Timecsv += UnityEngine.Time.deltaTime;

        if (dualSensorMode)
        {
            writeStream.WriteLine(Timecsv.ToString() + "," + dualSensorX.ToString() + "," + dualSensorY.ToString()
                            + "," + dualSensorZ.ToString()); //+ "," + quat[0].x.ToString() + "," + quat[0].y.ToString() + "," + quat[0].z.ToString() + "," + quat[0].w.ToString());
        }
        writeStream.Flush();
        writeStream.WriteLine(System.Environment.NewLine);
    }

    public void StopBluetooth()
    {
        _bluetoothobj.Stop();
        Debug.Log("Stopped bluetooth");
        isBluetooth = false;
        writeStream.Close();
    }

}
