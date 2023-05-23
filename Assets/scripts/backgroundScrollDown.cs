using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class backgroundScrollDown : MonoBehaviour
{
    //[SerializeField]
    //+71 to -87 y
    Vector3 initPos;
    Vector3 endPos;

    void Start()
    {
        Vector3 initPos = new Vector3(0,80,0);
        Vector3 endPos = new Vector3(0,-80,0);
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(initPos,endPos,0.2f);
    }
}