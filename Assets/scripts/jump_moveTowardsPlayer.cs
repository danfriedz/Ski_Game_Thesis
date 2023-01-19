using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jump_moveTowardsPlayer : MonoBehaviour
{
    //public GameObject player;
    public float speed = 1.0f;
    private Vector3 playerPos;
    private Vector3 currentPos;
    private Vector3 targetPos;
    private float startTime;
    private float distance = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        startTime = Time.time;
        currentPos = transform.position;                        //jumps spawn position
        playerPos = player.transform.position;                  //players position
        targetPos.x = -currentPos.x;                            //target is behind the player
        targetPos.y = playerPos.y;                              //tend towards players y position
        distance = Vector3.Distance(currentPos,targetPos);      //for lerp
    }

    void Update()
    {
        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / distance;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(currentPos, targetPos, fractionOfJourney);
    }
}

