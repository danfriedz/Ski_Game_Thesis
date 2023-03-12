using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particalSpawner : MonoBehaviour
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("spawn", 0, 0.1f);
    }

    void spawn()
    {
        Instantiate(prefab, parent.position,Quaternion.Euler(0, 180, 0));
    }
}
