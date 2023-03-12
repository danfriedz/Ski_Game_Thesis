using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveDown : MonoBehaviour
{
    [SerializeField] public float dropSpeed = 5.0f;
    [SerializeField] public Vector2 positionToMoveTo;
    [SerializeField] public float TreeLerpTime = 7;
    [SerializeField] public float treeMaxSize = 1.0f;
    [SerializeField] public GameObject icon;

    void Start()
    {
        positionToMoveTo = new Vector2(transform.position.x,transform.position.y-30);
        StartCoroutine(StartMovements(treeMaxSize, TreeLerpTime));
    }

    void Update()
    {
        //scaleOnYaxis();
    }

    IEnumerator StartMovements(float targetScale, float duration)
    {
        float time = 0;
        while(time < duration)
        {
            transform.localScale = Vector2.Lerp(new Vector2(0,0),new Vector2(treeMaxSize,treeMaxSize), time / duration);
            time += Time.deltaTime;
            if(time > 2)
            {
                StartCoroutine(LerpPosition(positionToMoveTo, TreeLerpTime));
            }
            yield return null;
        }
    }

    IEnumerator LerpPosition(Vector2 targetPosition, float duration)
    {
        float time = 0;
        Vector2 startPosition = transform.position;

        if (transform.position.y > 7)
        {
            Instantiate(icon, new Vector3(transform.position.x,4,0), Quaternion.identity);
        }
        
        while (time < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;

            yield return null;
        }
        transform.position = targetPosition;
    }

    void scaleOnYaxis()
    {
        float scaleFormula = -0.2117f*transform.position.y + 0.855882f;
        Vector3 distFromOrigin = new Vector3(scaleFormula,scaleFormula,1);
        transform.localScale = Vector3.Lerp(transform.localScale,distFromOrigin, Time.deltaTime*100);
    }
}
