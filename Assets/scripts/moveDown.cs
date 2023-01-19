using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveDown : MonoBehaviour
{
    public float dropSpeed = 5.0f;
    public Vector2 positionToMoveTo;
    public float TreeLerpTime = 7;
    public float treeMaxSize = 4.0f;
    [SerializeField] public GameObject icon;

    void Start()
    {
        positionToMoveTo = new Vector2(transform.position.x,transform.position.y-30);
        StartCoroutine(LerpPosition(positionToMoveTo, TreeLerpTime));
        StartCoroutine(LerpPosition(positionToMoveTo, TreeLerpTime));
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
            transform.localScale = Vector2.Lerp(new Vector2(0,0),new Vector2(treeMaxSize,treeMaxSize), time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
