using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private GameManager manager;
    private Camera myCam;
    public Movement playerMove;

    private bool deleteAfterTesting;
    private bool cardGrabbed;
    public bool moveCard, cardInformed;

    public GameObject selectedCardSlot;

    private void Start()
    {
        manager = GameManager.Instance;
        myCam = Camera.main;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            deleteAfterTesting = true;
        }

        if (!deleteAfterTesting)
        {
            Raycast();
        } 

    }

    private void Raycast()
    {
        #region Raycast Variables
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100;
        mousePos = myCam.ScreenToWorldPoint(mousePos);
        Ray rayo = myCam.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(myCam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        #endregion

        if (hit.collider.gameObject.tag.Equals("Card Slot"))
        {
            CardSlot currentCard = hit.collider.gameObject.GetComponent<CardSlot>();
            currentCard.hoverTimer = 0.2f;
            if (Input.GetMouseButtonDown(0))
            {
                if (!currentCard.isInHand)
                {
                    if (manager.currentState == GameManager.turnState.CheckMovement)
                    {
                        manager.selectedCardSlot = hit.collider.gameObject;
                        cardInformed = false;
                        playerMove.TryMove(currentCard.Location, new Vector2(currentCard.transform.position.x, currentCard.transform.position.y));
                    }
                }
            }

            //This code is for cards in your hand
            if (currentCard.isInHand)
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
