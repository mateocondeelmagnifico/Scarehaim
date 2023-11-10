using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    Camera myCam;
    public Movement playerMove;
    bool deleteAfterTesting;
    private bool cardGrabbed;
    public bool moveCard, cardInformed;

    public GameState State;
    public bool playerTurnInProgress;
    private bool winCondition;
    private bool loseCondition;
    public CardManager cardManager;
    public EnemyMovement enemy;

    public int turnCount;

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

    void Start()
    {       
        //El jugador empieza teniendo dos turnos seguidos
        playerTurnInProgress = true;
        turnCount = 1;
        updateGameState(GameState.PlayerTurn);
    }

    private void Update()
    {
        if(winCondition)
            updateGameState(GameState.Victory);
        else if(loseCondition)
            updateGameState(GameState.Defeat);
        else if (playerTurnInProgress)
            updateGameState(GameState.PlayerTurn);
        else
            updateGameState(GameState.EnemyTurn);
    }
    public void updateGameState(GameState newState)
    {
        State = newState;
        switch (newState)
        {
            case GameState.PlayerTurn:
                HandlePlayerTurn();
                break;
            case GameState.EnemyTurn:
                HandleEnemyTurn();
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void HandlePlayerTurn()
    {
        // turno del jugador
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            deleteAfterTesting = true;
        }

        if (!deleteAfterTesting)
        {
            Raycast();
        }

        if (selectedCardSlot != null)
        {
            if (player.transform.position == selectedCardSlot.transform.position && !cardInformed)
            {
                InformCard();
            }
        }

        if (moveCard)
        {
            MoveCardToHand(selectedCardSlot);
        }

    }
    private void HandleEnemyTurn()
    {
        // turno del enemigo
        if (enemy != null)
        {
            enemy.TryMove();

            if (enemy.myPos == playerMove.myPos) 
                loseCondition = true;
        }
        turnCount++;
        playerTurnInProgress = true;
    }
    
    public void EndPlayerTurn()
    {
        turnCount++;
        playerTurnInProgress = false;

        // win cons and lose cons
        // de momento no tenemos la casilla de salida asi que la condicion de victoria es descartar todas las cartas
        if (cardManager.cardsUntilExit == 0)
            winCondition = true;

        // condicion de derrota por hacer, if(fear >= 10)

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
                        cardInformed = false;
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
        selectedCardSlot.transform.GetChild(0).GetComponent<CardObject>().myCard.Effect(selectedCardSlot.transform.GetChild(0).gameObject, handSlotPrefab);
        cardInformed = true;
    }
    public void MoveCardToHand(GameObject card)
    {
        Vector3 desiredPos = new Vector3(0, -5f, 0);
        card.transform.position = Vector3.MoveTowards(card.transform.position,desiredPos, 5 * Time.deltaTime);
        if(card.transform.position == desiredPos)
        {
            moveCard = false;
            if(card.GetComponent<CardSlotHand>() != null)
            {
                //The component is disabled until it arrives to avoid bugs
                card.GetComponent<CardSlotHand>().enabled = true;
            }
        }
    }

    public enum GameState
    {
        PlayerTurn,
            MonsterCardEffect,
            CardToHandEffect,
            CardToDiscardPileEffect,
        EnemyTurn,
        Victory,
        Defeat
    }
}
