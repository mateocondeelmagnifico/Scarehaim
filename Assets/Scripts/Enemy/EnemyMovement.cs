using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Vector2 myPos;
    public Movement playerMove;
    public CardSlot initialSlot;
    public GameObject cardGrid;

    [SerializeField] private bool isMoving;

    private Vector2 destination;
    private Vector2 cardGridPos;
    private Vector2 cardActualPos;

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.13f);
        if(isMoving)
        {
            Move();
    
            if(transform.position.x == destination.x && transform.position.y == destination.y)
            {
                isMoving = false;
            }
        }
    }

    // logica del enemigo
    public void EnemyLogic()
    {
        // primero averigua el cardGridPos del CardSlot al que se quiere mover.
        if (playerMove.myPos.y > myPos.y)
            cardGridPos = new Vector2(myPos.x , myPos.y + 1);
        else if(playerMove.myPos.y < myPos.y)
            cardGridPos = new Vector2(myPos.x, myPos.y - 1);
        else if(playerMove.myPos.x > myPos.x)
            cardGridPos = new Vector2(myPos.x + 1, myPos.y);
        else
            cardGridPos = new Vector2(myPos.x - 1, myPos.y);

        // luego obtiene el cardActualPos de CardSlot al que se quiere mover.
        CardSlot destineCard = FindCardSlot(cardGridPos);
        cardActualPos = new Vector2(destineCard.transform.position.x, destineCard.transform.position.y);
    }
    public CardSlot FindCardSlot(Vector2 location)
    {
        CardSlot[] cardSlots = cardGrid.GetComponentsInChildren<CardSlot>();

        foreach (CardSlot cardSlot in cardSlots)
        {
            if (cardSlot.Location == location)
            {
                return cardSlot; // Return the CardSlot with the matching location
            }
        }

        // If the card slot is not found, you can return null or handle it as needed
        return null;
    }

    public void TryMove()
    {
        // EnemyLogic establece cual es el destination del enemigo
        EnemyLogic();
        if (cardGridPos.x <= myPos.x + 1 && cardGridPos.x >= myPos.x - 1 && cardGridPos.y <= myPos.y + 1 && cardGridPos.y >= myPos.y - 1 && !isMoving)
        {
            destination = cardActualPos;
            myPos = cardGridPos;
            isMoving = true;
        }
    }

    public void Move()
    {   
        transform.position = Vector2.MoveTowards(transform.position, destination, 4.5f * Time.deltaTime);
    }
}
