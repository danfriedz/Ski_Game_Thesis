using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Mouse_Hover : MonoBehaviour
{
    public Animator camAni;
    void Start()
    {
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            camAni.SetTrigger("MoveTo_LevelSelect");
        }
    }

    void OnMouseEnter(){
        //camAni.SetTrigger("MoveTo_LevelSelect");
    }

    void OnMouseDown()
    {
        //camAni.SetTrigger("MoveTo_LevelSelect");
    }

    void OnMouseExit() {
    }
}
