using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    [SerializeField] public Transform target;

    [SerializeField] public bool isCustomOffset;
    public Vector3 offset;
    private Vector3 OgPos;
    [SerializeField] public float rotationDivider = 1.3f;

    [SerializeField] public float smoothSpeed = 0.1f;

    [SerializeField] public float PixelsPerUnit;

    private void Start()
    {
        OgPos = transform.position;
        // You can also specify your own offset from inspector
        // by making isCustomOffset bool to true
        if (!isCustomOffset)
        {
            offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        SmoothFollow();   
    }

    public void SmoothFollow()
    {
        Vector3 targetPos = target.position + offset;
        Vector3 rotated3dFX = new Vector3(targetPos.x/rotationDivider,targetPos.y/rotationDivider,targetPos.z);        
        Vector3 smoothFollow = Vector3.Lerp(OgPos,rotated3dFX, smoothSpeed);

        transform.position = PixelPerfectClamp(smoothFollow, PixelsPerUnit);//smoothFollow;
        //transform.LookAt(target);

        print(rotated3dFX);
    }
    
    private Vector3 PixelPerfectClamp(Vector3 moveVector, float pixelsPerUnit)
    {
        Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(moveVector.x * pixelsPerUnit), Mathf.CeilToInt(moveVector.y * pixelsPerUnit), Mathf.CeilToInt(moveVector.z * pixelsPerUnit));                                         
        return vectorInPixels / pixelsPerUnit;
    }

}
