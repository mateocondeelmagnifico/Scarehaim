using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance {get; set;}

    public List<Card[]> cards = new List<Card[]>();

    public GameObject cardsOnBoard, cardPrefab, exitCard;
    private GameObject newCard;
    [SerializeField] private GameObject[] allCards;
    [SerializeField] private Card[] enviroments, enviroments2, enviroments3, treats, costumes;
    private Transform[] cardObjects;
    [SerializeField] private Transform enemy;

    public int cardsUntilExit, treatAmount, costumeAmount;
    private int resetTimes, cardsDealtAmount, turnsWithoutSpecial;
    private float startTimer;

    public bool exitCardDealt, cardsDealt, tricksNotRandom, canDealCards;
    private bool canEnd, cardChosen;
    private GameManager gameManager;
    [SerializeField] private MouseManager mouseManager;
    [SerializeField] private TutorialManager tutorialManager;
    private CardSlot cardSlot;
    public TMPro.TextMeshProUGUI doorText;
    private List<GameObject> board;

    [SerializeField] private Transform deck, enemyPos;
    private Transform playerPos, currentPos;
    public Transform tricks;

    private Vector3 originalPlayerPos, originalEnemyPos, desiredPos;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        gameManager = GameManager.Instance;
        playerPos = gameManager.player.transform;
        doorText.text = cardsUntilExit.ToString();
        startTimer = 3;

        #region Create Cards List
        cards.Add(enviroments);
        cards.Add(enviroments2);
        cards.Add(enviroments3);
        cards.Add(treats);
        cards.Add(costumes);

        allCards = new GameObject[cardsOnBoard.transform.childCount];
        #endregion
    }
    private void Start()
    {
        DistributeTricks();

        board = new List<GameObject>();

        for (int i = 0; i < cardsOnBoard.transform.childCount; i++)
        {
            //Get all the cards that are far away from the player
            if (Vector2.Distance(cardsOnBoard.transform.GetChild(i).position, playerPos.position) >= 4)
            {
                board.Add(cardsOnBoard.transform.GetChild(i).gameObject);
            }

            //Array de las cartas del tablero
            allCards[i] = cardsOnBoard.transform.GetChild(i).gameObject;
        }

        if(costumeAmount <= 0) cards.RemoveAt(4);

        #region Hide Cards at Start
        cardObjects = new Transform[cardsOnBoard.transform.childCount];

        for(int i = 0; i < cardsOnBoard.transform.childCount + 1; i++)
        {
            if ( i < cardsOnBoard.transform.childCount)
            {
                cardObjects[i] = cardsOnBoard.transform.GetChild(i).GetChild(0);
                cardObjects[i].position = deck.position;
                cardObjects[i].gameObject.SetActive(false);
            }
            else
            {
                originalPlayerPos = playerPos.position;
                originalEnemyPos = enemyPos.position;
                playerPos.position = deck.position;
                enemyPos.position = deck.position;
                playerPos.gameObject.SetActive(false);
                enemyPos.gameObject.SetActive(false);
            }
        }
        #endregion
    }

    private void FixedUpdate()
    {
        #region Deals cards at the start
        if (!cardsDealt && canDealCards)
        {
            if (startTimer > 0) startTimer -= Time.deltaTime;
            else
            {
                if (!cardChosen) ActivateCard();

                if (currentPos.position != desiredPos)
                {
                    if (cardsDealtAmount < cardsOnBoard.transform.childCount)
                    {
                        cardObjects[cardsDealtAmount].position = Vector3.MoveTowards(currentPos.position, desiredPos, (7 * Time.deltaTime) + (Vector2.Distance(currentPos.position, desiredPos) / 7));
                    }
                    else
                    {
                        if (cardsDealtAmount == cardsOnBoard.transform.childCount)
                        {
                            enemyPos.position = Vector3.MoveTowards(enemyPos.position, originalEnemyPos, 9 * Time.deltaTime);
                        }
                        else
                        {  
                            playerPos.position = Vector3.MoveTowards(playerPos.position, originalPlayerPos, 9 * Time.deltaTime);
                        }
                    }
                }
                else
                {
                    if (cardsDealtAmount <= cardsOnBoard.transform.childCount -2) cardObjects[cardsDealtAmount].GetComponent<SpriteRenderer>().sortingOrder = -2;

                    cardsDealtAmount++;
                    cardChosen = false;

                    if (cardsDealtAmount >= cardsOnBoard.transform.childCount + 2)
                    {
                        mouseManager.canClick = true;
                        cardsDealt = true;
                    }
                }
            }
        }

        #endregion

        if (cardsUntilExit == 0 && !exitCardDealt && gameManager.currentState == GameManager.turnState.CheckMovement)
        {
            #region Replace Card with Exit Card
            int randomNum = Random.Range(0, cardsOnBoard.transform.childCount);
            
            for (int i = randomNum; i < cardsOnBoard.transform.childCount; i++)
            {
                Transform chosenCard = cardsOnBoard.transform.GetChild(i);

                if(chosenCard.transform.childCount > 0)
                {
                    if (!chosenCard.GetChild(0).CompareTag("Enemy") && (chosenCard.position.x != playerPos.position.x && chosenCard.position.y != playerPos.position.y))
                    {
                        if(Vector3.Distance(cardsOnBoard.transform.GetChild(i).transform.position, enemy.position) > 0.1f)
                        {
                            if (allCards[i].transform.childCount > 0) Destroy(allCards[i].transform.GetChild(0).gameObject);
 
                            newCard = Instantiate(exitCard, allCards[i].transform);
                            newCard.transform.parent = allCards[i].transform;
                            allCards[i].GetComponent<CardSlot>().cardObject = newCard;
                            exitCardDealt = true;
                            if(tricks != null) tricks.gameObject.SetActive(false);
                            if(tutorialManager != null) tutorialManager.DisplayExitTutorial(allCards[i].GetComponent<CardSlot>().Location);

                            i = cardsOnBoard.transform.childCount;
                        }
                    }
                }
            }
            #endregion
        }
    }

    public void CardDiscarded(CardSlot whatSlot)
    {
        gameManager.slotToReplace = whatSlot.gameObject;
        doorText.text = cardsUntilExit.ToString();
    }
    public void DistributeCard()
    {
        //Called by gameManager
        cardSlot = gameManager.slotToReplace.GetComponent<CardSlot>();
        ReplaceCard();

        #region Assign card
        cardSlot.cardObject = newCard;
        newCard.transform.parent = cardSlot.gameObject.transform;
        gameManager.newCard = newCard;
        #endregion
    }

    private void ReplaceCard()
    {
        //this selects an individual card within card arrays
        //The first array is for enviroments, second for treats, third for costumes
        int randomInt = 0;
        if (turnsWithoutSpecial == 0)
        {
            //This is so you don't get two treats or costumes in a row
            randomInt = Random.Range(0, 3);
        }
        else
        {
            randomInt = Random.Range(0, cards.Count);
        }

        //This is so that you get a treat after 3 cards without getting one
        if (turnsWithoutSpecial >= 3 && cards.Count > 3)
        {
            if (cards.Count == 5)
            {
                randomInt = Random.Range(3, 5);
            }
            else randomInt = 3;
        }

        Card[] chosenArray = cards[randomInt];
        int newRandom = Random.Range(0, chosenArray.Length);

        //Check if you have received treat
        if ((randomInt != 3 || randomInt != 4) && cards.Count > 3) turnsWithoutSpecial++;
        else turnsWithoutSpecial = 0;

        newCard = Instantiate(cardPrefab, deck);
        newCard.GetComponent<CardObject>().myCard = chosenArray[newRandom];

        #region Check if it has run out of treats or costumes
        if (cards.Count == 4)
        {
            if (randomInt == 3)
            {
                if (costumeAmount > 0)
                {
                    costumeAmount--;
                    if (costumeAmount <= 0)
                    {
                        cards.RemoveAt(3);
                    }

                }

                if (treatAmount > 0)
                {
                    treatAmount--;
                    if (treatAmount <= 0)
                    {
                        cards.RemoveAt(3);
                    }
                
                }

            }
        }
        if (cards.Count == 5)
        {
            if (randomInt == 3)
            {
                treatAmount--;
                if (treatAmount <= 0)
                {
                    cards.RemoveAt(3);
                }
            }
            if (randomInt == 4)
            {
                costumeAmount--;
                if (costumeAmount <= 0)
                {
                    cards.RemoveAt(4);
                }
            }
        }
        #endregion
        
    }
    private void DistributeTricks()
    {
        if (!tricksNotRandom)
        {
            Vector3[] assignedPositions = new Vector3[tricks.childCount];
            int[] cardAdjacent = new int[3];

            for (int i = 0; i < cardsOnBoard.transform.childCount; i++)
            {
                //cada casilla tiene una probabilidad sobre 3 de tener una trampa
                //reset times esta para evitar bucles infinitos

                if (Random.Range(0, 7) + resetTimes > 4)
                {
                    for (int e = 0; e < assignedPositions.Length; e++)
                    {
                        if (assignedPositions[e] == Vector3.zero)
                        {                         
                            //Chekea si el lugar tiene un enemigo, o si ahi esta el jugador
                            Transform chosenCard = cardsOnBoard.transform.GetChild(i);
                            canEnd = false;

                            if (chosenCard.GetChild(0).tag != "Enemy")
                            {
                                if (chosenCard.position.x != playerPos.position.x || chosenCard.position.y != playerPos.position.y)
                                {
                                    bool canplace = true;

                                    //Check if there are any adjacent cards
                                    for (int u = 0; u < cardAdjacent.Length; u++)
                                    {
                                        if (i <= cardAdjacent[u] + 1 && i >= cardAdjacent[u] - 1) canplace = false;
                                    }

                                    //If you were to place two traps in the same position
                                    if (canplace)
                                    {
                                        assignedPositions[e] = chosenCard.position;
                                        tricks.GetChild(e).position = assignedPositions[e];
                                        cardAdjacent[e] = i;
                                    }
                                }                                
                            }
                        }
                        else if (e == assignedPositions.Length - 1)
                        {
                            canEnd = true;
                        }
                    }
                }

                if (resetTimes > 5)
                {
                    canEnd = true;
                }

                if (!canEnd)
                {
                    if (i == cardsOnBoard.transform.childCount - 1)
                    {
                        i = 0;
                        resetTimes++;
                    }
                }
                else
                {
                    i = cardsOnBoard.transform.childCount;
                }
            }
        }
    }
    private void ActivateCard()
    {
        //chooses and activates a card when dealing at the start

        if (cardsDealtAmount < cardsOnBoard.transform.childCount)
        {
            //Activa una carta
            cardObjects[cardsDealtAmount].gameObject.SetActive(true);
            cardObjects[cardsDealtAmount].GetComponent<SpriteRenderer>().sortingOrder = -1;

            currentPos = cardObjects[cardsDealtAmount].transform;
            desiredPos = cardsOnBoard.transform.GetChild(cardsDealtAmount).position;
        }
        else
        {
            //Activa al jugador o enemigo
            if (cardsDealtAmount == cardsOnBoard.transform.childCount)
            {
                enemyPos.gameObject.SetActive(true);
                currentPos = enemyPos;
                desiredPos = originalEnemyPos;
            }
            else
            {
                playerPos.gameObject.SetActive(true);
                currentPos = playerPos;
                desiredPos = originalPlayerPos;
            }
        }

        cardChosen = true;
    }
}
