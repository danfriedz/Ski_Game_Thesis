using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (gameObject.tag == "icon") { Destroy(gameObject); }
        else if (col.gameObject.tag == "obstacle"){ Destroy(col.gameObject);}
        else if (col.gameObject.tag == "jump"){Destroy(col.gameObject);}
    }
}
