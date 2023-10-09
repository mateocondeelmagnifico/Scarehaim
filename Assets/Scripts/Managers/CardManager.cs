using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Card[] cards;
    public GameObject cardsOnBoard;
    void Start()
    {
        //Give a random card to all the card slots on the board
        for(int i = 0; i < cardsOnBoard.transform.childCount; i++)
        {
            int randomInt = Random.Range(0, cards.Length);
            DistributeCard(cards[randomInt], cardsOnBoard.transform.GetChild(i).GetComponent<CardSlot>());
        }
    }

    private void DistributeCard(Card whatCard, CardSlot whatSlot)
    {
        whatSlot.mycard = whatCard;
    }
}
