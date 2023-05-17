using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class sceneChanger : MonoBehaviour
{
    public bool RHS_Scene;      //Right hand side minigame i.e Pronation 
    public bool LHS_Scene;      //Left hand side minigame
    public bool BackToHome;     //Back to hub world
    public int PronationCount;  //unused (keep track of exercise count)

    void Start()
    {
        //unused -> get exercise count from singletons
        //PronationCount = PersistentManagerScript.Instance.numOfPronation;
        //print(PronationCount);

    }

    //change to the approperate scene
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (RHS_Scene) SceneManager.LoadSceneAsync("MovingRightScene");
            if (LHS_Scene) SceneManager.LoadSceneAsync("MovingLeftScene");
            if (BackToHome) SceneManager.LoadSceneAsync("new art");
        }
    }
}
