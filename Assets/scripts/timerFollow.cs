using UnityEngine;
using System.Collections;
 
public class timerFollow : MonoBehaviour {
 
    public Vector3 pos;
 
    public GameObject player;
    public Camera camera;
    private Vector3 playerPos;
    private RectTransform rt;
    private RectTransform canvasRT;
    private Vector3 playerScreenPos;
 
    // Use this for initialization
    void Start () {
        playerPos = player.transform.position;
 
        rt = GetComponent<RectTransform>();
        canvasRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        playerScreenPos = camera.WorldToViewportPoint(player.transform.TransformPoint(playerPos));
        rt.anchorMax = playerScreenPos;
        rt.anchorMin = playerScreenPos;
    }
   
    // Update is called once per frame
    void Update () {
        playerScreenPos = camera.WorldToViewportPoint(player.transform.TransformPoint(playerPos));
        rt.anchorMax = playerScreenPos;
        rt.anchorMin = playerScreenPos;
    }
}
