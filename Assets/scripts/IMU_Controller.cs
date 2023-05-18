using System;
using System.Threading;
using UnityEngine;
using ServerReceiver;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using Codice.Client.Common;

public class IMU_Controller : MonoBehaviour
{
    public static IMU_Controller Instance { get; private set; }

    public static Server _bluetoothobj;
    public bool isBluetooth = false;
    private string _lineread1;
    private string[] _splitter1;
    private string[] storeSplitter1 = new string[30];
    [SerializeField] public bool dualSensorMode = true;


    //csv
    private FileStream streamFile;
    private StreamWriter writeStream;
    private string timeStamp;
    private string timeStampPrint;
    private DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private DateTime dateTimeNow;
    private TimeSpan timeElapsed;
    private double timeElapsedInseconds;

    public  Vector3 euler = new Vector3(0, 0, 0);
    public Vector3 euler2 = new Vector3(0, 0, 0);
    public float dualSensorX = 0;
    public float dualSensorY = 0;
    public float dualSensorZ = 0;

    public Quaternion[] quat = new Quaternion[5];
    private char[] _delimiter = { 'R', 'r', 'o', 'l', 'P', 'p', 'i', 't', 'c', 'h', 'a', 'w', 'Y', 'x', 'y', 'z', ',', ':', '{', '}', '[', ']', '\"', ' ', '|' };

    private void Awake()
    {
        //ensures we keep the first version of this version around.
        //destroys any duplicates
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
        {   // IMUdata
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

            dualSensorX = euler.x - euler2.x;
            dualSensorY = euler.y - euler2.y;
            dualSensorZ = euler.z - euler2.z;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopBluetooth();
            }

            //covert to quaternion
            if (dualSensorMode)
            {
                ConvertEulerToQuaternion(dualSensorX, dualSensorY, dualSensorZ);
            }
            else
            {
                ConvertEulerToQuaternion(euler.x, euler.y, euler.z);
            }
            //Debug.Log("x:" + quat[0].x + "y:" + quat[0].y + "z:" + quat[0].z + "w:" + quat[0].w);


            //Record into CSV file
            setTimeStamp();
            dateTimeNow = DateTime.Now;

            timeElapsed = dateTimeNow.Subtract(dateTime);

            timeElapsedInseconds = Convert.ToDouble(timeElapsed.TotalSeconds);
            streamFile = new FileStream("C:\\Users\\danie\\Desktop\\log" + dateTimeNow + ".csv", FileMode.OpenOrCreate);
            writeStream = new StreamWriter(streamFile);
            if (dualSensorMode)
            {
                writeStream.WriteLine(",,,," + timeElapsedInseconds.ToString() + "," + euler.x.ToString() + "," + euler.y.ToString()
                                + "," + euler.z.ToString() + "," + quat[0].x.ToString() + "," + quat[0].y.ToString() + "," + quat[0].z.ToString() + "," + quat[0].w.ToString());
            }

            writeStream.Flush();

        }
    }

    private void setTimeStamp()
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

        timeStamp = year + "-" + month + "-" + date + "-" + hour + "-" + minute + "-" + second;

        timeStampPrint = year + "/" + month + "/" + date + ":- " + hour + ":" + minute + ":" + second;
    }

   
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

    public void StopBluetooth()
    {
        _bluetoothobj.Stop();
        Debug.Log("Stopped bluetooth");
    }

}
