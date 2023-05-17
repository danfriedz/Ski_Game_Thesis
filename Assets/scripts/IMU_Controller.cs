using System;
using System.Threading;
using UnityEngine;
using ServerReceiver;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using System.IO;

public class IMU_Controller : MonoBehaviour
{
    public static IMU_Controller Instance { get; private set; }

    public static Server _bluetoothobj;
    public bool isBluetooth = false;
    private string _lineread1;
    private string[] _splitter1;
    private string[] storeSplitter1 = new string[30];

    public  Vector3 euler = new Vector3(0, 0, 0);
    private Vector3 euler2 = new Vector3(0, 0, 0);
    public Quaternion[] quat = new Quaternion[5];
    private char[] _delimiter = { 'R', 'r', 'o', 'l', 'P', 'p', 'i', 't', 'c', 'h', 'a', 'w', 'Y', 'x', 'y', 'z', ',', ':', '{', '}', '[', ']', '\"', ' ', '|' };

    private void Awake()
    {
        //ensures we keep the first OG version around.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //get a refrence to the dropdown buttons
        //on the level select screen

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
        //isStart = true;
    }

    void Update()
    {
        if (isBluetooth == true)
        {   //IMUdata
            // Receive from Bluetooth, each string it is assigned to is for each bluetooth 
            _lineread1 = _bluetoothobj.GetSensor1();

            _splitter1 = _lineread1.Split(_delimiter, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < _splitter1.Length; i++)
            {
                storeSplitter1[i] = _splitter1[i];
            }
/*
            //joint angle display
            jointAngle.text = "Joint Angle: " + leftJointAngle.ToString();
            RadialAngle.text = "Joint Angle: " + leftRadialAngle.ToString();
            PronationAngle.text = "Joint Angle: " + leftPronationAngle.ToString();
*/
            //sensor 1 data
            euler.x = float.Parse(storeSplitter1[0]);
            euler.y = float.Parse(storeSplitter1[1]);
            euler.z = float.Parse(storeSplitter1[2]);

            //sensor 2 data
            euler2.x = float.Parse(storeSplitter1[3]);
            euler2.y = float.Parse(storeSplitter1[4]);
            euler2.z = float.Parse(storeSplitter1[5]);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopBluetooth();
            }
/*
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //sensor 1 left wrist
                tempXAngle = euler.x;
                tempYAngle = euler.y;
                tempZAngle = euler.z;


                //sensor 2 left forearm
                tempXAngleLF = euler2.x;
                tempYAngleLF = euler2.y;
                tempZAngleLF = euler2.z;
            }
*/
/*
            //Head position
            actualXAngleH = (euler.x - tempXAngle);
            actualYAngleH = (euler.y - tempYAngle);
            actualZAngleH = (euler.z - tempZAngle);

            //Debug.Log("x: " + actualXAngleH + "y: " + actualYAngleH + "z: " + actualZAngleH);
            //left wrist postion
            actualXAngle = (euler.x - tempXAngle + 34.78f);
            actualYAngle = (euler.y - tempYAngle + 3.994f);
            actualZAngle = (euler.z - tempZAngle - 2.884f);

            //left forearm position
            actualXAngleLF = (euler2.x - tempXAngleLF - 16.145f);
            actualYAngleLF = (euler2.y - tempYAngleLF - 1.15f);
            actualZAngleLF = (euler2.z - tempZAngleLF - 15.16f);

            //Left wrist joint angle
            leftJointAngle = -(actualYAngleH);
            leftRadialAngle = -(actualZAngleH);
            leftPronationAngle = actualXAngleH;
*/

            //covert to quaternion
            ConvertEulerToQuaternion(euler.x, euler.y, euler.z);
            //Debug.Log("x:" + quat[0].x + "y:" + quat[0].y + "z:" + quat[0].z + "w:" + quat[0].w);

/*
            if (stateController.isHead)
            {
                head.transform.localRotation = Quaternion.Euler(actualYAngleH, -actualZAngleH, -actualXAngleH);
            }

            //Extension/Flexion
            if (stateController.isLeftExtension || stateController.isLeftRadial || stateController.isLeftPronation)
            {
                EnableControl();
                if (isStart && stateController.isLeftExtension)
                {
                    LeftWristExtension();
                }
                else if (isStart && stateController.isLeftRadial)
                {
                    LeftWristRadial();
                }
                else if (isStart && stateController.isLeftPronation)
                {
                    LeftWristPronation();
                }
                else
                {
                    isPronation = true;
                    isRadial = true;
                    isExtension = true;//set first exercise to be extension when button not pressed.

                }



            }

            if (resetTimer)
            {
                countDownTimer.text = "Timer";
                countDownRadialTimer.text = "Timer";
                countDownPronationTimer.text = "Timer";
            }
*/

/*
            //Record into CSV file
            setTimeStamp();
            if (isStart)
            {
                dateTimeNow = DateTime.Now;

                timeElapsed = dateTimeNow.Subtract(dateTime);

                timeElapsedInseconds = Convert.ToDouble(timeElapsed.TotalSeconds);

                writeStream.WriteLine(",,,," + timeElapsedInseconds.ToString() + "," + leftPronationAngle.ToString() + "," + leftJointAngle.ToString()
                                    + "," + leftRadialAngle.ToString()+","+ quat[0].x.ToString() + "," + quat[0].y.ToString() + "," + quat[0].z.ToString() + "," + quat[0].w.ToString());

                writeStream.Flush();
            }
*/

        }
    }


    public void StartBluetooth()
    {
        //IMUdata
        // Start receiving from bluetooth of both sensors instead of IP 
        Debug.Log("Starting");
/*
        Finger._bluetoothobj.Start();
*/
        Debug.Log("Starting Bluetooth");
        // _bluetoothobj.Calibrate();

        // Wait to ensure bluetooth communication begins before calibration
        //Thread.Sleep(300);

        //set array element to "0"
        for (int i = 0; i < storeSplitter1.Length; i++)
        {
            storeSplitter1[i] = "0";
        }

        //Initializes hold time;

/*
        currentTime = sliderScript.holdTime;
        storeCurrentTime = currentTime;
        currentRadialTime = sliderScript.holdTimeRadial;
        currentPronationTime = sliderScript.holdTimePronation;

*/
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
