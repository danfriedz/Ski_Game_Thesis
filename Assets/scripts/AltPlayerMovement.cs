using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltPlayerMovement : MonoBehaviour
{
   [SerializeField] public GameObject IMU_data;
   [SerializeField] public float smoothRotation = 5.0f;
   [SerializeField] public float tiltAngleMax = 60.0f;
   [SerializeField] public Animator Photo_LHS;
   [SerializeField] public SpriteRenderer Photo_LSH_SR;
   [SerializeField] public Animator Crowd_RHS;
   [SerializeField] public Sprite[] SkiPhotos;
   [SerializeField] public bool altRotationFlag = true; //rotate in strech zone?
   const float originYOffset = -2.5f;  //move player center position slightly to account for background

    float IMU_horizontal_normalized;
    float IMU_vertical_normalized;

    Objective_Spawner objScript;
    IMU_Controller IMU_controller;
 

   void Start()
   {
        objScript = GameObject.FindGameObjectWithTag("objSpawn").GetComponent<Objective_Spawner>();
        IMU_controller = GameObject.FindGameObjectWithTag("IMU").GetComponent<IMU_Controller>();
    }
   
   void FixedUpdate()
   { 
      lockPlayerToCameraBoundary();
      rotatePlayer();
      scalePlayer();
      IMUdataNormalized();
      transformPlayerToLocation();
   }

    //Takes imu euler values in degrees and converts to values between -1 and 1 for use in player controller.
   void IMUdataNormalized()
   {
        // Yaw would be horizonatal axis, pitch vertical axis
        const int yawMax = 40;
        const int yawMin = -50;
        const int pitchMin = -70;
        const int pitchMax = 55;

        IMU_horizontal_normalized = -1 * (2*((IMU_controller.euler.z - yawMin) / (yawMax - yawMin)) - 1);
        IMU_vertical_normalized = 2 * ((IMU_controller.euler.y - pitchMin) / (pitchMax - pitchMin)) - 1;
   }
   void transformPlayerToLocation()
   {
        // offsets
        const int WorldPosX = 12, WorldPosY = 5;
        float x_Pos = IMU_horizontal_normalized * WorldPosX;
        float y_Pos = IMU_vertical_normalized * WorldPosY;
        transform.position = Vector3.Lerp(transform.position, new Vector3(x_Pos,y_Pos + originYOffset,0),Time.deltaTime * 2);
   }

   //player y axis effects scale (to fake depth)
   void scalePlayer()
   {
      //desmos regresion ~1.3 close to screen, ~0.4 far from screen
      float scaleFormula = -0.202f*transform.position.y + 0.926f;
      Vector3 distFromOrigin = new Vector3(scaleFormula,scaleFormula,1);
      transform.localScale = Vector3.Lerp(transform.localScale,distFromOrigin, Time.deltaTime*10);
   }

   //Prevent the player from moving outside the bounds of the camera
   void lockPlayerToCameraBoundary()
   {
      Vector3 minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
      Vector3 maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
      //y position changed to - 3.5 from - 1 to stop player entering foreground graphics
      transform.position = new Vector3(Mathf.Clamp(transform.position.x, minScreenBounds.x + 1, maxScreenBounds.x - 1),Mathf.Clamp(transform.position.y, minScreenBounds.y + 1, maxScreenBounds.y - 3.5f), transform.position.z);
   }

   //left/right movement tilts the player sprite.
   void rotatePlayer()
   {
      float tiltAroundZ = IMU_horizontal_normalized * tiltAngleMax * -1;
      Quaternion target = Quaternion.Euler(0, 0, tiltAroundZ);
      transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * smoothRotation);
   }






   void OnTriggerEnter2D(Collider2D col)
   {
      //Check which region of the gamespace the player is in. 
      if((col.gameObject.tag == "Left") || (col.gameObject.tag == "Right") || (col.gameObject.tag == "Up") ||(col.gameObject.tag == "Down"))
      {
         objScript.startStrechRoutine(col.gameObject.tag);
      }
      
      if(col.gameObject.tag == "obstacle")
      {
            //unused (trees are obstacles, was not a good design choise for this project)
      }
   }

   void OnTriggerStay2D(Collider2D col)
   {
      //applies opposite rotation when in strech zone
      if(altRotationFlag == true)
      {
         if((col.gameObject.tag == "Left") || (col.gameObject.tag == "Right") || (col.gameObject.tag == "Up") ||(col.gameObject.tag == "Down"))
         {   
            float tiltAroundZ = IMU_horizontal_normalized * tiltAngleMax * -1;
            Quaternion target = Quaternion.Euler(0, 0, -tiltAroundZ*2);
            transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * smoothRotation);
         }
      }
   }

   //reset slow down FX (move out of character controller)
   void OnTriggerExit2D(Collider2D col) {
      if((col.gameObject.tag == "Left") || (col.gameObject.tag == "Right") || (col.gameObject.tag == "Up") ||(col.gameObject.tag == "Down"))
      {
         objScript.startStrechRoutine("Stop");
      }

      /*
      if(col.gameObject.tag == "obstacle"){
         if (Time.timeScale != 1.0f){
            Time.timeScale = 1.0f;
         }
      }
      */
   }
}