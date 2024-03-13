using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Vector2 myPos;
    public Movement playerMove;
    public CardSlot initialSlot;
    public GameObject cardGrid;
    private TextManager textManager;
    private Animator animador;
    private GameManager gameManager;

    [SerializeField] private bool isMoving;
    private bool hasTalked, hasMoved;

    private Vector2 destination;
    private Vector2 cardGridPos;
    private Vector2 cardActualPos;

    public float turnsUntilStart;

    private void Start()
    {
        textManager = TextManager.Instance;
        turnsUntilStart = 2;
        animador = GetComponent<Animator>();
        gameManager = GameManager.Instance;
    }
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

        #region Talking
        if ((cardGridPos.x == playerMove.myPos.x && (cardGridPos.y == playerMove.myPos.y + 1 || cardGridPos.y == playerMove.myPos.y - 1)) || (cardGridPos.y == playerMove.myPos.y && (cardGridPos.x == playerMove.myPos.x + 1 || cardGridPos.x == playerMove.myPos.x - 1)))
        {
            if(!hasTalked)
            {
                textManager.Talk(TextManager.EnemyStates.NearPlayer);
                hasTalked = true;
            }
            textManager.closeToEnemy = true;
        }
        else
        {
            if(textManager.closeToEnemy && (Mathf.Abs(cardGridPos.x) - Mathf.Abs(playerMove.myPos.x) > 1 || Mathf.Abs(cardGridPos.y) - Mathf.Abs(playerMove.myPos.y) > 1))
            {
                textManager.closeToEnemy = false;
                textManager.SwapSprite();
                textManager.Talk(TextManager.EnemyStates.Annoyed);
            }
            
            hasTalked = false;
        }
        #endregion

        if (turnsUntilStart == 0 && !hasMoved) animador.SetBool("shaking", true);
        else animador.SetBool("shaking", false);

    }

    // logica del enemigo
    public void EnemyLogic()
    {
        // primero averigua el cardGridPos del CardSlot al que se quiere mover.
        if(playerMove.myPos.y == myPos.y || playerMove.myPos.x == myPos.x)
        {
            #region Not random calculation
            if (playerMove.myPos.y == myPos.y)
            {
                if(playerMove.myPos.x > myPos.x) cardGridPos = new Vector2(myPos.x + 1, myPos.y);
                else cardGridPos = new Vector2(myPos.x - 1, myPos.y); ;
            }

            if (playerMove.myPos.x == myPos.x)
            {
                if(playerMove.myPos.y > myPos.y) cardGridPos = new Vector2(myPos.x, myPos.y + 1);
                else cardGridPos = new Vector2(myPos.x, myPos.y - 1);
            }
            #endregion
        }
        else
        {
            #region Random calculation
            int random = Random.Range(0, 2);

            if(random == 0)
            {
                if(playerMove.myPos.x > myPos.x)
                {
                    cardGridPos = new Vector2(myPos.x + 1, myPos.y);
                }
                else
                {
                    cardGridPos = new Vector2(myPos.x - 1, myPos.y);
                }
            }
            else
            {
                if (playerMove.myPos.y > myPos.y)
                {
                    cardGridPos = new Vector2(myPos.x, myPos.y + 1);
                }
                else
                {
                    cardGridPos = new Vector2(myPos.x, myPos.y - 1);
                }
            }
            #endregion
        }

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
        Debug.Log(1);
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
        hasMoved = true;

        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        if(pos != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 4.5f * Time.deltaTime);
        }
        else
        {
            if(gameManager.currentState == GameManager.turnState.Endturn)
            {
                gameManager.currentState = GameManager.turnState.CheckMovement;
            }
        }
    }
}
