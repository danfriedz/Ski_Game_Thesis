using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   public float baseSpeed = 10.0f;
   public float maxSpeed = 50.0f;
   private float HorizontalSpeed = 0.0f;
   public float smoothRotation = 5.0f;
   public float tiltAngleMax = 60.0f;
   public float timeScaleSlowdown = 0.7f;
   private Rigidbody2D rb;
   public Animator Photo_LHS;
   public SpriteRenderer Photo_LSH_SR;
   public Animator Crowd_RHS;
   public Sprite[] SkiPhotos;

   void Start()
   {
       rb = GetComponent<Rigidbody2D>();
   }
   
   void FixedUpdate()
   { 
      lockPlayerToCameraBoundary();
      rotatePlayerViaKeys();
      horizontalForceWithAngle();
      //Speed boost (currently unused)
      if (Input.GetKeyDown("left ctrl"))
      {
         rb.AddForce(new Vector2(5.0f,0));
      }
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

   void OnTriggerEnter2D(Collider2D col)
   {
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

   //reset slow down FX (move out of character controller)
   void OnTriggerExit2D(Collider2D col) {
      if(col.gameObject.tag == "obstacle"){
         if (Time.timeScale != 1.0f){
            Time.timeScale = 1.0f;
         }
      }
   }
}