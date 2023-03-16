using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionSpawner : MonoBehaviour
{
    [SerializeField] public GameObject Tree_Drop;
    [SerializeField] public GameObject Tree_Left;
    [SerializeField] public GameObject Tree_Right;
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
            //Pick a position within a set range to spawn object
                        //Vector3 randomizePosition = new Vector3(Random.Range(-8, 8),15,0);
            Vector3 randomizePosition = new Vector3(Random.Range(-8, 8),2.7f,0);
            //Spawn a predefined object
            Instantiate(Tree_Drop, randomizePosition, Quaternion.identity);
        }
        //for moving Left screen
        else if (spawnLeft)
        {
            Vector3 randomizePositionTop = new Vector3(-35,Random.Range(5,12),0);
            Vector3 randomizePositionBot = new Vector3(-35,Random.Range(-3,-12),0);
            Instantiate(Tree_Left, randomizePositionTop, Quaternion.identity);
            Instantiate(Tree_Left, randomizePositionBot, Quaternion.identity);
        }
        //for moving Right screen
        else if (spawnRight)
        {
            Vector3 randomizePositionTop = new Vector3(35,Random.Range(5,12),0);
            Vector3 randomizePositionBot = new Vector3(35,Random.Range(-3,-12),0);
            Instantiate(Tree_Right, randomizePositionTop, Quaternion.identity);
            Instantiate(Tree_Right, randomizePositionBot, Quaternion.identity);
        }
    }
}
