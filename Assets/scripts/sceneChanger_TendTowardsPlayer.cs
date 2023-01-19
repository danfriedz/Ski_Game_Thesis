using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneChanger_TendTowardsPlayer : MonoBehaviour
{
    private Vector3 playerPosition;
    private Vector3 currentPosition;
    private Animator Animator;
    public GameObject player;

    void Awake()
    {
        Animator = gameObject.GetComponent<Animator>();
        currentPosition = transform.localPosition;
        playerPosition = player.transform.localPosition;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            //Animator.SetBool("Return", false);
            if (playerPosition.x < currentPosition.x)
            {
                Animator.SetBool("Return", false);
                Animator.SetBool("TendLeft", true);
            }
            if (playerPosition.x > currentPosition.x)
            {
                Animator.SetBool("Return", false);
                Animator.SetBool("TendRight", true);
            }

        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Animator.SetBool("TendLeft", false);
            Animator.SetBool("TendRight", false);
            Animator.SetBool("Return", true);
        }
    }
}
