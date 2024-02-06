using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [HideInInspector] public Movement playerMove;
    [HideInInspector] public bool moveCardToHand, moveCard, cardInformed;

    public int cardDiscarded;

    public GameState State;
    [HideInInspector] public bool playerTurnInProgress, trapTriggered, powerUpOn;
    private bool winCondition, loseCondition;
    public bool mustMove;

    [HideInInspector]
    public enum turnState
    {
        CheckMovement,
        Moving,
        ReplaceCard,
        CheckCardEffect,
        ApplyCardEffect,
        Movecard,
        Endturn
    }

    public turnState currentState;

    [HideInInspector] public CardManager cardManager;
    [HideInInspector] public EnemyMovement enemy;

    public int turnCount;

    private List<GameObject> cardsInHand = new List<GameObject>();

    public GameObject player, selectedCardSlot, handSlotPrefab, selectedCard, newCardSlot, emptySlot, newCard, slotToReplaceOld, slotToReplaceNew;

    [HideInInspector]
    public Transform deck, discardPile, hand;
    void Awake()
    {
        Time.timeScale = 1.0f;

        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentState = turnState.CheckMovement;

        //This is to force a resolution
        Camera.main.pixelRect = new Rect(0, 0, 1920, 1080);
    }

    void Start()
    {
        playerTurnInProgress = true;
        turnCount = 1;
        updateGameState(GameState.PlayerTurn);
    }

    private void Update()
    {

        if (enemy.transform.position == player.transform.position)
            player.GetComponent<Fear>().fear = 10;

        if (winCondition)
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
                HandlePlayerTurn(currentState);
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

    private void HandlePlayerTurn(turnState currentState)
    {
        // turno del jugador

        switch (currentState)
        {
            //En muchos estados no pasa nada, pero estan ah�a para las acciones vayan de estado en estado
            case turnState.CheckMovement:
                //Player movement is the one that changes to the next state
                cardInformed = false;
            break;

            case turnState.Moving:
                //Also changed in player movement
                break;

            case turnState.ReplaceCard:
                
                if (slotToReplaceOld != null && !mustMove)
                {
                    cardManager.DistributeCard();
                    mustMove = true;
                }

                if (mustMove)
                {
                    MoveCard(newCard, slotToReplaceOld.transform.position);
                }
                else
                {
                    ChangeState(turnState.CheckCardEffect);
                }

                break;

            case turnState.CheckCardEffect:
                //This is changed by the card's script
                slotToReplaceOld = slotToReplaceNew;
                emptySlot = selectedCardSlot;
                if(!trapTriggered && !cardInformed)
                {
                    InformCard();
                }
                break;

            case turnState.ApplyCardEffect:
                //Changed by the cardSlot
            break;

            case turnState.Movecard:
                if (moveCardToHand || moveCard)
                {
                    if (moveCard)
                        //MoveCard(selectedCard, discardPile.position);
                        MoveCard(selectedCard, discardPile.position);

                    if(moveCardToHand)
                    MoveCardToHand(newCardSlot);
                }
                else
                {
                    ChangeState(turnState.Endturn);
                }
                break;

            case turnState.Endturn:
                EndPlayerTurn();
            break;
        }

       

    }
    private void HandleEnemyTurn()
    {
        // turno del enemigo
        if (enemy != null)
        {
            enemy.TryMove();
        }
        turnCount++;
        playerTurnInProgress = true;
        currentState = turnState.CheckMovement;
    }
    
    public void EndPlayerTurn()
    {
        turnCount++;
        playerTurnInProgress = false;

        // win cons and lose cons
        // de momento no tenemos la casilla de salida asi que la condicion de victoria es descartar todas las cartas

        // condicion de derrota por hacer, if(fear >= 10)
    }

    private void InformCard()
    {
        //This script tells the card that it has to activate
        cardInformed = true;
        selectedCardSlot.transform.GetChild(0).GetComponent<CardObject>().myCard.Effect(selectedCardSlot.transform.GetChild(0).gameObject, handSlotPrefab);
    }

    private void MoveCard( GameObject whatCard, Vector3 desiredPos)
    {
        //This cript is used to move cards to the deck and discard pile
        whatCard.transform.position = Vector3.MoveTowards(whatCard.transform.position, desiredPos, 8 * Time.deltaTime);

        if(whatCard.transform.position == desiredPos)
        {
            moveCard = false;

            if(desiredPos == discardPile.position)
            {
                whatCard.transform.parent = discardPile;
                currentState = turnState.Endturn;
            }
            else if(slotToReplaceOld != null)
            {
                currentState = turnState.CheckCardEffect;
                mustMove = false;
            }
        }
    }

    public void MoveCardToHand(GameObject card)
    {
        cardsInHand.Add(card);

        SortCardInHand(card);
    }

    public void SortCardInHand(GameObject card)
    {
        Vector3 desiredPos = new Vector3(-3.5f, -5f, 0);
        Vector3 offset = new Vector3(1.5f, 0, 0);
        Vector3 rotationOffset = new Vector3(0f, 0f, 10f);
        Vector3 resetRotation = new Vector3(0, 0, 0);

        card.transform.position = desiredPos + (cardsInHand.Count * offset);
        card.transform.parent = hand;

        switch (cardsInHand.Count)
        {
            case 0: break;
            
            case 1:
                cardsInHand[0].transform.rotation = Quaternion.Euler(resetRotation);
                break;
            case 2:
                cardsInHand[0].transform.rotation = Quaternion.Euler(rotationOffset);
                cardsInHand[1].transform.rotation = Quaternion.Euler(-rotationOffset);
                break;
            case 3:
                cardsInHand[0].transform.rotation = Quaternion.Euler(rotationOffset);
                cardsInHand[1].transform.rotation = Quaternion.Euler(resetRotation);
                cardsInHand[2].transform.rotation = Quaternion.Euler(-rotationOffset);
                break;
            case 4:
                cardsInHand[0].transform.rotation = Quaternion.Euler(2*rotationOffset);
                cardsInHand[1].transform.rotation = Quaternion.Euler(rotationOffset);
                cardsInHand[2].transform.rotation = Quaternion.Euler(-rotationOffset);
                cardsInHand[3].transform.rotation = Quaternion.Euler(-2*rotationOffset);
                break;
            case 5:
                cardsInHand[0].transform.rotation = Quaternion.Euler(2 * rotationOffset);
                cardsInHand[1].transform.rotation = Quaternion.Euler(rotationOffset);
                cardsInHand[2].transform.rotation = Quaternion.Euler(resetRotation);
                cardsInHand[3].transform.rotation = Quaternion.Euler(-rotationOffset);
                cardsInHand[4].transform.rotation = Quaternion.Euler(-2 * rotationOffset);
                break;
        }

        moveCardToHand = false;
        if (card.GetComponent<CardSlotHand>() != null)
        {
            //The component is disabled until it arrives to avoid bugs
            card.GetComponent<CardSlotHand>().enabled = true;
        }
        currentState = turnState.Endturn;
    }

    private void ChangeState(turnState newstate)
    {
        //Esta funcion esta porque no funciona cambiar el state dentro del switch
        currentState = newstate;
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
