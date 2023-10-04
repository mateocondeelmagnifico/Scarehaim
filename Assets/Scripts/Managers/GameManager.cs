using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Camera myCam;
    void Start()
    {
        myCam = Camera.main;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100;
        mousePos = myCam.ScreenToWorldPoint(mousePos);

        Ray rayo = myCam.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(myCam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        
        if (hit.collider.gameObject.tag.Equals("Card Slot"))
        {
            hit.collider.gameObject.GetComponent<CardSlot>().hoverTimer = 0.2f;
        }
        
        
    }
}
