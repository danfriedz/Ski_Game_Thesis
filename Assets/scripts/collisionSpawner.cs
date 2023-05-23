using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionSpawner : MonoBehaviour
{
    [SerializeField] public GameObject Tree_Drop;
    [SerializeField] public GameObject Tree_Drop_Variation;
    [SerializeField] public GameObject Tree_Left;
    [SerializeField] public GameObject Tree_Left_smaller;
    [SerializeField] public GameObject Tree_Right;
    [SerializeField] public GameObject Tree_Right_smaller;
    [SerializeField] public float spawnFreq = 1.0f;
    [SerializeField] public bool spawnTop = false;
    [SerializeField] public bool spawnLeft = false;
    [SerializeField] public bool spawnRight = false;

    void Start()
    {
        // Spawn an object every x seconds
        InvokeRepeating("SpawnObject",1, spawnFreq);
    }

    void SpawnObject()
    {
        //for hub screen. trees drop down
        if (spawnTop)
        {
            Vector3 randomizePosition = new Vector3(Random.Range(-8, 8),2.5f,0);
            //Spawn a predefined object
            int randomNum = Random.Range(0, 2);
            if (randomNum == 0) {Instantiate(Tree_Drop, randomizePosition, Quaternion.identity);}
            if (randomNum == 1) {Instantiate(Tree_Drop_Variation, randomizePosition, Quaternion.identity);}
            
        }
        //for moving Left screen
        else if (spawnLeft)
        {
            Vector3 randomizePositionTop = new Vector3(-35,Random.Range(5,7),0);
            Vector3 randomizePositionBot = new Vector3(-35,Random.Range(-3,-12),0);
            Instantiate(Tree_Left_smaller, randomizePositionTop, Quaternion.identity);
            Instantiate(Tree_Left, randomizePositionBot, Quaternion.identity);
        }
        //for moving Right screen
        else if (spawnRight)
        {
            Vector3 randomizePositionTop = new Vector3(35,Random.Range(5,7),0);
            Vector3 randomizePositionBot = new Vector3(35,Random.Range(-3,-12),0);
            Instantiate(Tree_Right_smaller, randomizePositionTop, Quaternion.identity);
            Instantiate(Tree_Right, randomizePositionBot, Quaternion.identity);
        }
    }
}
