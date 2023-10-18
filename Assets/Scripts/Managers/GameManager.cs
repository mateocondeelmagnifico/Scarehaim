using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    Camera myCam;
    public Movement playerMove;
    bool deleteAfterTesting;
    private bool cardGrabbed;
    public bool moveCard;

    //Player should be a singleton later on
    public GameObject player;
    public GameObject selectedCardSlot, handSlotPrefab;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        myCam = Camera.main;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            deleteAfterTesting = true;
        }

        if (!deleteAfterTesting)
        {
            Raycast();
        }
        
        if(selectedCardSlot != null)
        {
            if (player.transform.position == selectedCardSlot.transform.position)
            {
                InformCard();
            }
        }

        if(moveCard)
        {
            MoveCardToHand(selectedCardSlot);
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
                    if(!moveCard)
                    {
                        selectedCardSlot = hit.collider.gameObject;
                    }
                    playerMove.TryMove(currentCard.Location, new Vector2(currentCard.transform.position.x, currentCard.transform.position.y));
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
    private void InformCard()
    {
        //This script tells the card that it has to activate
        //selectedCardSlot.transform.GetChild(0).GetComponent<CardObject>().myCard.MoveToHand(selectedCardSlot.transform.GetChild(0).gameObject, handSlotPrefab);
        selectedCardSlot.transform.GetChild(0).GetComponent<CardObject>().myCard.Effect(selectedCardSlot.transform.GetChild(0).gameObject, handSlotPrefab);
    }
    public void MoveCardToHand(GameObject card)
    {
        Vector3 desiredPos = new Vector3(0, -5f, 0);
        card.transform.position = Vector3.MoveTowards(card.transform.position,desiredPos, 5 * Time.deltaTime);
        if(card.transform.position == desiredPos)
        {
            moveCard = false;
        }
    }
}
