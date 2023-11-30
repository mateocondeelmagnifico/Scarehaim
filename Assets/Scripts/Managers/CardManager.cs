using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance {get; set;}

    public List<GameObject> cards = new List<GameObject>();
    public GameObject exitCard;
    public GameObject cardsOnBoard, cardPrefab;
    private GameObject newCard;

    public int cardsUntilExit, treatAmount, costumeAmount;

    public bool cardHasToBeReplaced, exitCardDealt;
    private GameManager gameManager;
    private CardSlot cardSlot;
    public TMPro.TextMeshProUGUI doorText;
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
        doorText.text = cardsUntilExit.ToString();
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
            newCard = Instantiate(exitCard, gameManager.deck);
            exitCardDealt = true;
        }
        else
        {
            int randomInt = Random.Range(0, cards.Count);

            newCard = Instantiate(cards[randomInt], gameManager.deck);

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
                if(randomInt == 2)
                {
                    treatAmount--;
                    if(treatAmount <= 0)
                    {
                        cards.RemoveAt(2);
                    }
                    
                }
                if(randomInt == 3)
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
        #endregion

        #region Assign card
        cardSlot.cardObject = newCard;
        newCard.transform.parent = cardSlot.gameObject.transform;
        gameManager.selectedCard = newCard;
        #endregion

        cardHasToBeReplaced = false;

        gameManager.cardDiscarded--;
    }
}
