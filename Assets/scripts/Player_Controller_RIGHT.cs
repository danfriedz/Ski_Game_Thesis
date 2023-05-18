using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Controller_RIGHT : MonoBehaviour
{
    public float holdDuration = 5.0f;   //length of strech
    public float deadSpace = 0.3f;      //controller deadspace
    private float elapsedTime = 0.0f;
    private bool heldLongEnough = false;
    private int numberOfJumps = 0;
    private float startTime = 0.0f;
    private bool greatJumpBool = false;
    public SpriteRenderer AmazingUI;
    public SpriteRenderer crowdSprite;
    public SpriteRenderer photoSprite;
    private Vector3 RHS_Vector_Pos = new Vector3(50,0,0);
    private Vector3 LHS_Vector_Pos = new Vector3(-50,0,0);
    public GameObject jumpPrefab_Right;
    public GameObject jumpPrefab_Left;
    public Animator cameraAnimator;
    public bool rightScene;
    public bool leftScene;

    private float IMU_roll_normalized;

    public TextMeshProUGUI holdTimeTextUI;
    public TextMeshProUGUI notificationUI;

    IMU_Controller IMU_controller;


    // Start is called before the first frame update
    void Start()
    {
        IMU_controller = GameObject.FindGameObjectWithTag("IMU").GetComponent<IMU_Controller>();
        startTime = Time.time;
        AmazingUI.enabled = false;
        crowdSprite.enabled = false;
        photoSprite.enabled = false;
    }

    void Update()
    {
        lockPlayerToBoundary();
        holdStrechTimer();
        IMUdataNormalized();

        if (heldLongEnough == true)
        {
            spawnJump();
            cameraAnimator.SetBool("closeCameraZoomIn", true);
            notificationUIPrint("Do A Flip!");
            StartCoroutine(Timer(3));
        } 
    }
    void IMUdataNormalized()
    {
        const int yawMax = 40;
        const int yawMin = -50;
        const int pitchMin = -70;
        const int pitchMax = 55;
        const int rollMin = -80;
        const int rollMax = 80;

        if (IMU_controller.dualSensorMode)
        {
            //uses imu with wrist as a ref point
            IMU_roll_normalized = -1 * (2 * ((IMU_controller.dualSensorZ - yawMin) / (yawMax - yawMin)) - 1);
        }
        if (IMU_controller.dualSensorMode == false)
        {
            //single sensor mode (hand only with no wrist refrence IMU)
            IMU_roll_normalized = (2 * ((IMU_controller.euler.x - rollMin) / (rollMax - rollMin)) - 1);
        }
    }

    bool correctDirectionCheck()
    {
        if (leftScene == true && IMU_roll_normalized < 0) return true;
        if (rightScene == true && IMU_roll_normalized > 0) return true;
        else return false;
    }

    void holdStrechTimer()
    {
        if (correctDirectionCheck())
        {
            if(strechCheck() == 1)
            {
                elapsedTime += Time.deltaTime;
            }
            if(strechCheck() == 0) {
                heldLongEnough = false;
                elapsedTime = 0;
                //print("resetting timer");
                notificationUIPrint("Hold That Stretch!");
            }
            if (elapsedTime > holdDuration)
            {
                //print("Held for long enough");
                notificationUIPrint("Here Comes The Jump!");
                heldLongEnough = true;
            }
            if (elapsedTime > 4.0f)
            {
                crowdSprite.enabled = true;
            }
            canvasHoldTimePrint(elapsedTime);
        }
    }

    void spawnJump()
    {
        //we only want to instantiate one jump
        if (numberOfJumps == 0)
        {
            numberOfJumps++;
            if (rightScene) Instantiate(jumpPrefab_Right, RHS_Vector_Pos, Quaternion.identity);
            if (leftScene) Instantiate(jumpPrefab_Left, LHS_Vector_Pos, Quaternion.identity);
        }
    }

    void lockPlayerToBoundary()
   {
        Vector3 minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minScreenBounds.x + 1, maxScreenBounds.x - 1),Mathf.Clamp(transform.position.y, minScreenBounds.y + 1, maxScreenBounds.y - 1), transform.position.z);
   }

   float movementDirection()
   {
        //tending towards... -1 is left, 1 is right
        return IMU_roll_normalized;// Input.GetAxis("Horizontal");
   }

   int strechCheck()
   {
        if (Mathf.Abs(IMU_roll_normalized) > deadSpace) {return 1;}
        else return 0;
   }

   IEnumerator startHoldTimer()
   {
        yield return new WaitForSeconds(holdDuration);
        if (strechCheck() == 0)
        {
            StopCoroutine(startHoldTimer());
            yield break;
        }
        //notificationUIPrint("Don't Slow Down Now!");
        print("hold duration ended");
   }

   void OnTriggerEnter2D(Collider2D col)
   {
        if (col.gameObject.tag == "jump")
        {
            if (Time.timeScale == 1.0f)
            {
                Time.timeScale = 0.2f;
            }
            else Time.timeScale = 1.0f;
        }
        if (col.gameObject.name == "JumpRunUp")
        {
            if (Mathf.Abs(IMU_roll_normalized) > deadSpace)
            {
                greatJumpBool = true;
                AmazingUI.enabled = true;
            }
            photoSprite.enabled = true;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        //check to see if the player releases the strech on the jump
        if (col.gameObject.name == "GoodJumpZone")
        {
            print("hit trigger");
            if (Mathf.Abs(IMU_roll_normalized) < deadSpace)
            {
                if (greatJumpBool == true)
                {
                    print("Great Jump");
                }
                if (greatJumpBool == false)
                {
                    print("OK Jump");
                }
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "jump"){
            if (Time.timeScale != 1.0f){
                Time.timeScale = 1.0f;
            }
        }
    }

    IEnumerator Timer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //turns off camera zoom in animation after a few secs. stops it repeating
        cameraAnimator.SetBool("closeCameraZoomIn", false);
    }


    //UI Stuff that could be moves out of this script later
    void canvasHoldTimePrint(float elapsedTime)
    {
        if(elapsedTime > holdDuration){
            elapsedTime = holdDuration;
        }
        holdTimeTextUI.text = elapsedTime.ToString("#.00");
    }

    void notificationUIPrint(string stretchFeedbackStr)
    {
        notificationUI.text = stretchFeedbackStr;
    }
}