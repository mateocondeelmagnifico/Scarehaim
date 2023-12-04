using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance {get; set;}

    public List<GameObject[]> cards = new List<GameObject[]>();

    public GameObject cardsOnBoard, cardPrefab, exitCard;
    private GameObject newCard;
    [SerializeField] private GameObject[] enviroments, enviroments2, treats, costumes;

    public int cardsUntilExit, treatAmount, costumeAmount;

    public bool cardHasToBeReplaced, exitCardDealt;
    private GameManager gameManager;
    private CardSlot cardSlot;
    public TMPro.TextMeshProUGUI doorText;

    private Transform playerPos;
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
        cards.Add(enviroments);
        cards.Add(enviroments2);
        cards.Add(treats);
        cards.Add(costumes);
    }

    public void CardDiscarded(CardSlot whatSlot)
    {
        cardSlot = whatSlot;
        cardHasToBeReplaced = true;
        gameManager.cardDiscarded++;
        doorText.text = cardsUntilExit.ToString();
    }
    public void DistributeCard()
    {
        //Called by gameManager

        #region Create card
        if (cardsUntilExit == 0 && !exitCardDealt)
        {
            //distance < 4
            List<GameObject> board = new List<GameObject>();
             
            for(int i = 0; i < cardsOnBoard.transform.childCount; i++)
            {
                //Get all the cards that are far away from the player
               if(Vector2.Distance(cardsOnBoard.transform.GetChild(i).position, playerPos.position) >= 4)
               {
                   board.Add(cardsOnBoard.transform.GetChild(i).gameObject);
               }
            }

            #region Replace Card with Exit Card
            int randomNum = Random.Range(0, board.Count);
            Destroy(board[randomNum].transform.GetChild(0).gameObject);
            newCard = Instantiate(exitCard, board[randomNum].transform);
            newCard.transform.parent = board[randomNum].transform;
            board[randomNum].GetComponent<CardSlot>().cardObject = newCard;
            #endregion

            ReplaceCard();
            exitCardDealt = true;
        }
        else
        {
            ReplaceCard();
        }
        #endregion

        #region Assign card
        cardSlot.cardObject = newCard;
        newCard.transform.parent = cardSlot.gameObject.transform;
        gameManager.newCard = newCard;
        #endregion

        cardHasToBeReplaced = false;

        gameManager.cardDiscarded--;
    }

    private void ReplaceCard()
    {
        //this selects an individual card within card arrays
        //The first array is for enviroments, second for treats, third for costumes
        int randomInt = Random.Range(0, cards.Count);
        GameObject[] chosenArray = cards[randomInt];
        int newRandom = Random.Range(0, chosenArray.Length);

        newCard = Instantiate(chosenArray[newRandom], gameManager.deck);

        #region Check if it has run out of treats or costumes
        if (cards.Count == 3)
        {
            if (randomInt == 2)
            {
                if (costumeAmount > 0)
                {
                    costumeAmount--;
                    if (costumeAmount <= 0)
                    {
                        cards.RemoveAt(2);
                    }

                }
                if (treatAmount > 0)
                {
                    treatAmount--;
                    if (costumeAmount <= 0)
                    {
                        cards.RemoveAt(2);
                    }
                }
            }
        }
        if (cards.Count == 4)
        {
            if (randomInt == 2)
            {
                treatAmount--;
                if (treatAmount <= 0)
                {
                    cards.RemoveAt(2);
                }

            }
            if (randomInt == 3)
            {
                costumeAmount--;
                if (costumeAmount <= 0)
                {
                    cards.RemoveAt(3);
                }
            }
        }
        #endregion
    }
}
