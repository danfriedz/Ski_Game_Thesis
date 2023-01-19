using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManagerScript : MonoBehaviour
{
    public static PersistentManagerScript Instance {get; private set;}

    public int Value; //test value is a singleton 
    //i can call singletons form this script using
    //PersistentManagerScript.Instance.Singleton etc
    public int numOfPronation = 3;
    public int numOfSupination = 3;

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
}
