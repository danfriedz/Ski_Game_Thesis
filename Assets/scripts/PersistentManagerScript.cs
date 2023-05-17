using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManagerScript : MonoBehaviour
{
    public static PersistentManagerScript Instance {get; private set;}

    public int Value; //test value is a singleton 
    //i can call singletons form this script using
    //PersistentManagerScript.Instance.Singleton etc
    public static int Singleton_numOfPronation = 3;
    public static int Singleton_numOfSupination = 3;
    public int Singleton_numLeftStreches = 3;
    public int Singleton_numRightStreches = 3;
    public int Singleton_numUpStreches = 3;
    public int Singleton_numDownStreches = 3;

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

    private void Update()
    {
        if (Singleton_numLeftStreches < 0) Singleton_numLeftStreches = 0;
        if (Singleton_numRightStreches < 0) Singleton_numRightStreches = 0;
        if (Singleton_numUpStreches < 0) Singleton_numUpStreches = 0;
        if (Singleton_numDownStreches < 0) Singleton_numDownStreches = 0;
        if (Singleton_numOfPronation < 0) Singleton_numOfPronation = 0;
        if (Singleton_numOfSupination < 0) Singleton_numOfSupination = 0;
    }
}
