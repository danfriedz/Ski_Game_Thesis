using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCollisionLeft_Right : MonoBehaviour
{
    [SerializeField] public bool Left_Tree = false;
    [SerializeField] public bool Right_Tree = false;
    private Vector2 positionToMoveTo;

    void Start()
    {
        if (Left_Tree){
            positionToMoveTo = new Vector2(transform.position.x+90,transform.position.y);
        }
        if (Right_Tree){
            positionToMoveTo = new Vector2(transform.position.x-90,transform.position.y);
        }
        StartCoroutine(LerpPosition(positionToMoveTo,3));
    }
   
    IEnumerator LerpPosition(Vector2 targetPosition, float duration)
    {
        float time = 0;
        Vector2 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
