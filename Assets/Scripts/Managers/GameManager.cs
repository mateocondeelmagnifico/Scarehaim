using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Camera myCam;
    public Movement playerMove;
    bool deleteAfterTesting;
    private bool cardGrabbed;
    void Start()
    {
        myCam = Camera.main;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            deleteAfterTesting = true;
        }
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100;
        mousePos = myCam.ScreenToWorldPoint(mousePos);

        Ray rayo = myCam.ScreenPointToRay(mousePos);

        if (!deleteAfterTesting)
        {
            RaycastHit2D hit = Physics2D.Raycast(myCam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider.gameObject.tag.Equals("Card Slot"))
            {
                CardSlot currentCard = hit.collider.gameObject.GetComponent<CardSlot>();
                currentCard.hoverTimer = 0.2f;
                if (Input.GetMouseButtonDown(0))
                {
                    if (!currentCard.isInHand)
                    {
                        playerMove.TryMove(currentCard.Location, new Vector2(currentCard.transform.position.x, currentCard.transform.position.y));
                    }
                }

                //This code is for cards in your hand
                if(currentCard.isInHand)
                {
                    CardSlotHand currentCardHand = hit.collider.gameObject.GetComponent<CardSlotHand>();
                    if (Input.GetMouseButton(0) && !cardGrabbed)
                    {
                        currentCardHand.followMouse = true;
                        cardGrabbed = true;
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        currentCardHand.followMouse = false;
                        cardGrabbed = false;
                    }
                }
               
            }
        }
        
    }
}
