using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [HideInInspector] public Movement playerMove;
    [HideInInspector] public bool moveCardToHand, moveCard, cardInformed;

    private Hand handScript;

    public int cardDiscarded;

    public GameState State;
    [HideInInspector] public bool playerTurnInProgress, trapTriggered, powerUpOn;
    private bool winCondition, loseCondition, slotErased;
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
        handScript = Hand.Instance;
        hand = handScript.transform;
        playerTurnInProgress = true;
        turnCount = 1;
        updateGameState(GameState.PlayerTurn);

        #region Reset hand variables
        if(hand.transform.childCount > 0)
        {
            for(int i = 0; i < hand.transform.childCount; i++)
            {
                hand.transform.GetChild(i).GetComponent<CardSlotHand>().gameManager = this;
            }
        }
        #endregion
    }

    private void Update()
    {

        if (enemy.transform.position == player.transform.position && enemy.turnsUntilStart <= 0)
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
                slotErased = false;
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
                    MoveCard(newCard, slotToReplaceOld.transform.position, newCard.GetComponent<SpriteRenderer>());
                }
                else
                {
                    ChangeState(turnState.CheckCardEffect);
                }

                break;

            case turnState.CheckCardEffect:
                //This is changed by the card's script
                if (!slotErased)
                {
                    slotToReplaceOld = slotToReplaceNew;
                }

                if (!selectedCardSlot.transform.GetChild(0).CompareTag("Enemy"))
                {
                    emptySlot = selectedCardSlot;
                }
                else if(!slotErased)
                {
                    slotToReplaceNew = null;
                    slotErased = true;
                }

                
                
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
                        MoveCard(selectedCard, discardPile.position, selectedCard.GetComponent<SpriteRenderer>());

                    if(moveCardToHand)
                    {
                        if(hand.childCount < 5)
                        {
                            MoveCardToHand(newCardSlot);
                        }
                        else
                        {
                            MoveCard(newCardSlot, discardPile.position, newCardSlot.transform.GetChild(0).GetComponent<SpriteRenderer>());
                        }
                    }   

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
            if(enemy.turnsUntilStart <=0)
            {
                enemy.TryMove();
            }
            else
            {
                enemy.turnsUntilStart--;
            }
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

    private void MoveCard( GameObject whatCard, Vector3 desiredPos, SpriteRenderer renderer)
    {
        //This cript is used to move cards to the deck and discard pile
        whatCard.transform.position = Vector3.MoveTowards(whatCard.transform.position, desiredPos, 8 * Time.deltaTime);
        renderer.sortingOrder = 0;

        if(whatCard.transform.position == desiredPos)
        {
            moveCard = false;

            if(desiredPos == discardPile.position)
            {
                if(moveCardToHand)
                {
                    //put card from hand into graveyard
                    whatCard.transform.GetChild(0).parent = discardPile;
                    Destroy(whatCard);
                    moveCardToHand = false;
                }
                else
                {
                    whatCard.transform.parent = discardPile;
                }
                renderer.sortingOrder = -2;
                currentState = turnState.Endturn;
            }
            else if(slotToReplaceOld != null)
            {
                renderer.sortingOrder = -2;
                currentState = turnState.CheckCardEffect;
                mustMove = false;
            }
        }
    }

    public void MoveCardToHand(GameObject card)
    {
        if(hand.childCount < 5)
        {
            Vector3 desiredPos = new Vector3(4, -5, -2);

            card.transform.position = Vector3.MoveTowards(card.transform.position, desiredPos, 8 * Time.deltaTime);


            if (card.transform.position == desiredPos)
            {
                card.transform.parent = hand;
                moveCardToHand = false;
                if (card.GetComponent<CardSlotHand>() != null)
                {
                    //The component is disabled until it arrives to avoid bugs
                    card.GetComponent<CardSlotHand>().enabled = true;
                }
                handScript.AddCardToHand(card.transform);
                currentState = turnState.Endturn;
            }
        }
       
        
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
