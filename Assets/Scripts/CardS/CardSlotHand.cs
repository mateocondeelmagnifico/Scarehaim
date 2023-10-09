using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotHand : CardSlot
{
    private Vector3 startingPos;
    Vector3 direction;

    public bool goHome;
    public bool followMouse;

    void Start()
    {
        isInHand = true;
        startingPos = transform.position;
    }

    private void Update()
    {
        //Both Relocate and Move are called by the gameManager
        
        if (followMouse)
        {
            direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(direction.x, direction.y, transform.position.z);
        }
        else
        {
            Relocate();
        }

        if (goHome)
        {
            if (transform.position != startingPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, startingPos, 16 * Time.deltaTime);
            }
            else
            {
                goHome = false;
            }
        }
    }

    public void Relocate()
    {
        if(transform.position != startingPos)
        {
            goHome = true;
        }
    }

    public void Move(Vector3 mousePos)
    {
        transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
    }
}
