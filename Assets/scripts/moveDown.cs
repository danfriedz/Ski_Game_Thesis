using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveDown : MonoBehaviour
{
    [SerializeField] public float dropSpeed = 5.0f;
    [SerializeField] public Vector2 positionToMoveTo;
    [SerializeField] public float TreeLerpTime = 7;
    [SerializeField] public float treeMaxSize = 10.0f;
    [SerializeField] public GameObject icon;
    [SerializeField] public float sidewaysAdjustment = 2.0f;


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
            if(time > 1)
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

        float horizOffsetFormula = sidewaysAdjustment*startPosition.x;
        positionToMoveTo = new Vector2(transform.position.x+horizOffsetFormula,transform.position.y-30);

        if (transform.position.y > 7)
        {
            Instantiate(icon, new Vector3(transform.position.x,4,0), Quaternion.identity);
        }
        
        while (time < duration)
        {
            //transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            //float formula = 0.207734f*time*time-0.105f;//0.9f * Mathf.Pow(2.718f,(69f * time));
            transform.position = Vector2.Lerp(startPosition, positionToMoveTo, time / duration);

            time += Time.deltaTime;
            print(time);

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
