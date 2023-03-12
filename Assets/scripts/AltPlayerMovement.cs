using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltPlayerMovement : MonoBehaviour
{
   [SerializeField] public float baseSpeed = 10.0f;
   [SerializeField] public float maxSpeed = 50.0f;
   [SerializeField] private float HorizontalSpeed = 0.0f;
   [SerializeField] public float smoothRotation = 5.0f;
   [SerializeField] public float tiltAngleMax = 60.0f;
   [SerializeField] public float timeScaleSlowdown = 0.95f;
   [SerializeField] private Rigidbody2D rb;
   [SerializeField] public Animator Photo_LHS;
   [SerializeField] public SpriteRenderer Photo_LSH_SR;
   [SerializeField] public Animator Crowd_RHS;
   [SerializeField] public Sprite[] SkiPhotos;
   [SerializeField] public bool altRotationFlag = true; //rotate in strech zone?

   Objective_Spawner objScript;
   //private Objective_Spawner objScript = ObjSpawn.GetComponent<Objective_Spawner>();
   

   void Start()
   {
      rb = GetComponent<Rigidbody2D>();
      //ObjSpawn = GameObject.Find(("ObjectiveSpawner"));
      objScript = GameObject.FindGameObjectWithTag("objSpawn").GetComponent<Objective_Spawner>();
      //Objective_Spawner objScript = ObjSpawn.GetComponent<Objective_Spawner>();
   }
   
   void FixedUpdate()
   { 
      lockPlayerToCameraBoundary();
      rotatePlayerViaKeys();
      //horizontalForceWithAngle();
      //movePlayerInDirection();
      transformPlayerToLocation();
      //Speed boost (currently unused)
      if (Input.GetKeyDown("left ctrl"))
      {
         rb.AddForce(new Vector2(5.0f,0));
      }
      //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      //print(worldPosition);
   }

   void transformPlayerToLocation()
   {
      //convert positions
      const int WorldPosX = 12, WorldPosY = 7;
      float XPosPercent = Input.GetAxis("Horizontal") * WorldPosX;
      float YPosPercent = Input.GetAxis("Vertical") * WorldPosY;
      transform.position = Vector3.Lerp(transform.position, new Vector3(XPosPercent,YPosPercent,0),Time.deltaTime * 2);
   }

   //Prevent the player from moving outside the bounds of the camera
   void lockPlayerToCameraBoundary()
   {
      Vector3 minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
      Vector3 maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
      transform.position = new Vector3(Mathf.Clamp(transform.position.x, minScreenBounds.x + 1, maxScreenBounds.x - 1),Mathf.Clamp(transform.position.y, minScreenBounds.y + 1, maxScreenBounds.y - 1), transform.position.z);
   }

   //left/right movement tilts the player sprite.
   void rotatePlayerViaKeys()
   {
      float tiltAroundZ = Input.GetAxis("Horizontal") * tiltAngleMax * -1;
      Quaternion target = Quaternion.Euler(0, 0, tiltAroundZ);
      transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * smoothRotation);
   }

   // applies force proportional to tilt angle.
   void horizontalForceWithAngle()
   {
      HorizontalSpeed = Mathf.Clamp(baseSpeed * transform.rotation.z * Mathf.Rad2Deg * -1,-maxSpeed,maxSpeed);
      rb.AddForce(new Vector2(HorizontalSpeed,0));
   }

   // move player in direction (keys 2 cardinal for now. upgrade to imu later)
   void movePlayerInDirection()
   {
      float HorizontalMove = Input.GetAxis("Horizontal") * baseSpeed * 10;
      float VerticalMove = Input.GetAxis("Vertical") * baseSpeed * 10;
      rb.AddForce(new Vector2(HorizontalMove,VerticalMove));
   }

   void OnTriggerEnter2D(Collider2D col)
   {
      //Check which region of the gamespace the player is in. 
      if((col.gameObject.tag == "Left") || (col.gameObject.tag == "Right") || (col.gameObject.tag == "Up") ||(col.gameObject.tag == "Down"))
      {
         objScript.startStrechRoutine(col.gameObject.tag);
      }

      //slow down fx (move out of character controller)
      if(col.gameObject.tag == "obstacle")
      {
         if (Time.timeScale == 1.0f)
         {
            Time.timeScale = timeScaleSlowdown;
         }
         else Time.timeScale = 1.0f;
         
         // visual feedback. Should be moved out of character controller
         Photo_LHS.Play("snapshot");
         Photo_LSH_SR.sprite = SkiPhotos[Random.Range(0,5)];
         Crowd_RHS.Play("Crowd_Rise");
      }
   }

   void OnTriggerStay2D(Collider2D col)
   {
      //applies opposite rotation when in strech zone
      if(altRotationFlag == true)
      {
         if((col.gameObject.tag == "Left") || (col.gameObject.tag == "Right") || (col.gameObject.tag == "Up") ||(col.gameObject.tag == "Down"))
         {   
            float tiltAroundZ = Input.GetAxis("Horizontal") * tiltAngleMax * -1;
            Quaternion target = Quaternion.Euler(0, 0, -tiltAroundZ*2);
            transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * smoothRotation);
         }
      }
   }

   //reset slow down FX (move out of character controller)
   void OnTriggerExit2D(Collider2D col) {
      if((col.gameObject.tag == "Left") || (col.gameObject.tag == "Right") || (col.gameObject.tag == "Up") ||(col.gameObject.tag == "Down"))
      {objScript.startStrechRoutine("Stop");}


      if(col.gameObject.tag == "obstacle"){
         if (Time.timeScale != 1.0f){
            Time.timeScale = 1.0f;
         }
      }
   }
}