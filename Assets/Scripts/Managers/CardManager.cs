using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance {get; set;}

    public GameObject[] cards;
    public GameObject cardsOnBoard, cardPrefab;

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

    private void Update()
    {
        if(cardHasToBeReplaced && !gameManager.cardInformed)
        {
            DistributeCard();
        }
    }
    public void CardDiscarded(CardSlot whatSlot)
    {
        cardSlot = whatSlot;
        cardHasToBeReplaced = true;
    }
    public void DistributeCard()
    {
        #region Create card
        int randomInt = Random.Range(0, cards.Length);
        GameObject newCard = Instantiate(cards[randomInt], cardSlot.gameObject.transform);
        #endregion

        #region Assign card
        cardSlot.cardObject = newCard;
        newCard.transform.parent = cardSlot.gameObject.transform;
        #endregion

        cardHasToBeReplaced = false;
    }
}
