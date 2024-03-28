using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainCamSize : MonoBehaviour
{
    private Camera cam;

    float width, height;

    Vector3 pos;

    private void Start()
    {
        cam = GetComponent<Camera>();

        pos = transform.position;
        width = cam.orthographicSize * cam.aspect;
        height = cam.orthographicSize;

        //This is to force a resolution
        //Camera.main.pixelRect = new Rect(0, 0, 1920, 1080); 
    }

    private void Update()
    {
        cam.orthographicSize = width / cam.aspect;
        transform.position = new Vector3(pos.x,-1 * (height - cam.orthographicSize),pos.z);
    }
}
