using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance {get; set;}

    public Treat[] cards;
    public float cardsUntilExit;
    public GameObject cardsOnBoard, cardPrefab;
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

        //Give a random card to all the card slots on the board
        for (int i = 0; i < cardsOnBoard.transform.childCount; i++)
        {
            #region Create Card Prefabs and asign them to each card
            GameObject cardToPut = Instantiate(cardPrefab);
            cardsOnBoard.transform.GetChild(i).GetComponent<CardSlot>().cardObject = cardToPut;
            cardToPut.transform.parent = cardsOnBoard.transform.GetChild(i);
            cardToPut.transform.position = cardToPut.transform.parent.position;
            #endregion

            DistributeCard(cardsOnBoard.transform.GetChild(i).GetComponent<CardSlot>());
        }
    }

    public void DistributeCard(CardSlot whatSlot)
    {
        int randomInt = Random.Range(0, cards.Length);
        whatSlot.cardObject.GetComponent<CardObject>().myCard = cards[randomInt];
    }
}
