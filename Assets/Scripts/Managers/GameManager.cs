using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [HideInInspector] public Movement playerMove;
    public bool moveCardToHand, moveCard, cardInformed;

    private Hand handScript;

    public int cardDiscarded;

    public bool playerTurnInProgress, trapTriggered, powerUpOn;
    private bool slotErased, enemyInformed;
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
    public TurnCheck turnCounter;

    public int turnCount;

    public GameObject player, selectedCardSlot, handSlotPrefab, slotToReplace, newCardSlot, emptySlot, newCard;
    private GameObject selectedCard;

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
    }

    void Start()
    {
        handScript = Hand.Instance;
        hand = handScript.transform;
        playerTurnInProgress = true;
        turnCount = 1;
        turnCounter.turnsUntilEnemy = enemy.turnsUntilStart - 2;
           
        #region Reset hand variables
        if (hand.transform.childCount > 0)
        {
            for(int i = 0; i < hand.transform.childCount; i++)
            {
                hand.transform.GetChild(i).GetComponent<CardSlotHand>().gameManager = this;
            }
        }

        handScript.zPrompt.SetActive(false);
        #endregion
    }

    private void Update()
    {

        if (Vector3.Distance(enemy.transform.position, player.transform.position) < 0.3f && turnCount > 2) player.GetComponent<Fear>().UpdateFear(-10);

        #region Check Turn State
        switch (currentState)
        {
            //En muchos estados no pasa nada, pero estan ahía para las acciones vayan de estado en estado
            case turnState.CheckMovement:
                //Player movement is the one that changes to the next state
                cardInformed = false;
                slotErased = false;
                enemyInformed = false;
                break;

            case turnState.Moving:
                //Also changed in player movement
                break;

            case turnState.ReplaceCard:

                if (slotToReplace != null && !mustMove)
                {
                    cardManager.DistributeCard();
                    mustMove = true;
                }

                if (mustMove)
                {
                    MoveCard(newCard, slotToReplace.transform.position, newCard.GetComponent<SpriteRenderer>());
                }
                else
                {
                    ChangeState(turnState.CheckCardEffect);
                }

                break;

            case turnState.CheckCardEffect:
                //This is changed by the card's script

                if (!selectedCardSlot.transform.GetChild(0).CompareTag("Enemy"))
                {
                    emptySlot = selectedCardSlot;
                }
                else if (!slotErased)
                {
                    slotToReplace = null;
                    slotErased = true;
                }

                if (!trapTriggered && !cardInformed)
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

                    if (moveCardToHand)
                    {
                        if (hand.childCount < 5)
                        {
                            MoveCardToHand(newCardSlot);
                        }
                        else
                        {
                            MoveCard(newCardSlot, discardPile.position, newCardSlot.transform.GetChild(0).GetComponent<SpriteRenderer>());
                        }
                    }
                    else if (moveCard) MoveCard(selectedCard, discardPile.position, selectedCard.GetComponent<SpriteRenderer>());
                }
                else
                {
                    ChangeState(turnState.Endturn);
                }
                break;

            case turnState.Endturn:
                //This moves the enemy too

                if (!enemyInformed)
                {
                    turnCounter.MoveLeft();
                    EndPlayerTurn();
                }
                break;
        }
        #endregion
    }
    
    public void EndPlayerTurn()
    {
        // turno del enemigo
        if (enemy != null)
        {
            if (enemy.turnsUntilStart <= 0)
            {
                enemy.TryMove();
            }
            else
            {
                enemy.TurnInStasis(-1);
                ChangeState(turnState.CheckMovement);
            }

            turnCount++;
            enemyInformed = true;
        }
    }

    private void InformCard()
    {
        //This script tells the card that it has to activate
        cardInformed = true;
        selectedCard = selectedCardSlot.transform.GetChild(0).gameObject;
        selectedCardSlot.transform.GetChild(0).GetComponent<CardObject>().DoEffect(handSlotPrefab);
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
                if(discardPile.childCount > 1) Destroy(discardPile.transform.GetChild(0).gameObject);
                renderer.sortingOrder = -2;
                currentState = turnState.Endturn;
            }
            else if(slotToReplace != null)
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

    public void ChangeState(turnState newstate)
    {
        //Esta funcion esta porque no funciona cambiar el state dentro del switch
        currentState = newstate;
    }

    public bool CheckIsInCheckMovement()
    {
        bool istrue = false;
        if (currentState == turnState.CheckMovement) istrue = true;

        return istrue;
    }

    public void DestroyHand()
    {
        //Void is called by buttons
        Hand.Instance.NukeSelf();
    }
}
