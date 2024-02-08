using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class Card : MonoBehaviour
{

    public Sprite image, bigImage;

    CardSlot slot;

    public delegate void cardEffect();
    public event cardEffect discardCard;


    public virtual void Effect(GameObject card, GameObject cardSlot)
    {

    }

    public virtual void PlayEffect()
    {

    }
    public virtual void MoveToHand(GameObject card, GameObject cardSlot) 
    {
        #region Make new cardslot and change variables
        transform.parent.GetComponent<CardSlot>().cardObject = null;

        GetComponent<SpriteRenderer>().sortingOrder = 20;

        GameManager manager = GameManager.Instance;
        GameObject newSlot = GameObject.Instantiate(cardSlot);

        newSlot.transform.position = card.transform.position;
        newSlot.GetComponent<CardSlotHand>().cardObject = card;
        newSlot.GetComponent<CardSlotHand>().enabled = false;
        CardManager.Instance.CardDiscarded(transform.parent.GetComponent<CardSlot>());
        card.transform.parent = newSlot.transform;

        manager.newCardSlot = newSlot;
        manager.moveCardToHand = true;

        #endregion

        GameManager.Instance.currentState = GameManager.turnState.Movecard;
    }

    public void DiscardCard()
    {
        discardCard.Invoke();
    }

}
