using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance {get; set;}

    public GameObject[] cards;
    public GameObject exitCard;
    public GameObject cardsOnBoard, cardPrefab;
    private GameObject newCard;

    public float cardsUntilExit;

    public bool cardHasToBeReplaced;
    private GameManager gameManager;
    private CardSlot cardSlot;
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
    }
    public void CardDiscarded(CardSlot whatSlot)
    {
        cardSlot = whatSlot;
        cardHasToBeReplaced = true;
        gameManager.cardDiscarded++;
    }
    public void DistributeCard()
    {
        //Called by gameManager

        #region Create card
        if (cardsUntilExit == 0)
        {
            newCard = Instantiate(exitCard, cardSlot.gameObject.transform);
        }
        else
        {
            int randomInt = Random.Range(0, cards.Length);
            newCard = Instantiate(cards[randomInt], cardSlot.gameObject.transform);
        }
        #endregion

        #region Assign card
        cardSlot.cardObject = newCard;
        newCard.transform.parent = cardSlot.gameObject.transform;
        #endregion

        cardHasToBeReplaced = false;

        gameManager.cardDiscarded--;
        gameManager.currentState = GameManager.turnState.Endturn;
    }
}
