using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Treat[] cards;

    public GameObject cardsOnBoard, cardPrefab;
    void Awake()
    {
        //Give a random card to all the card slots on the board
        for(int i = 0; i < cardsOnBoard.transform.childCount; i++)
        {
            #region Create Card Prefabs and asign them to each card
            int randomInt = Random.Range(0, cards.Length);
            GameObject cardToPut = Instantiate(cardPrefab);
            cardsOnBoard.transform.GetChild(i).GetComponent<CardSlot>().cardObject = cardToPut;
            cardToPut.transform.parent = cardsOnBoard.transform.GetChild(i);
            cardToPut.transform.position = cardToPut.transform.parent.position;
            #endregion

            DistributeCard(cards[randomInt], cardsOnBoard.transform.GetChild(i).GetComponent<CardSlot>());
        }
    }

    private void DistributeCard(Card whatCard, CardSlot whatSlot)
    {
        whatSlot.cardObject.GetComponent<CardObject>().myCard = whatCard;
    }
}
