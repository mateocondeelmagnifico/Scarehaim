using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class Card : MonoBehaviour
{
    public string name;

    public Sprite image;

    CardSlot slot;

    public delegate void cardEffect();
    public event cardEffect replaceCard;


    public virtual void Effect(GameObject card, GameObject cardSlot)
    {

    }
    public virtual void MoveToHand(GameObject card, GameObject cardSlot) 
    {
        GameManager manager = GameManager.Instance;
        GameObject newSlot = GameObject.Instantiate(cardSlot);

        newSlot.transform.position = card.transform.position;
        newSlot.GetComponent<CardSlotHand>().enabled = false;
        card.transform.parent = newSlot.transform;

        manager.selectedCardSlot = newSlot;
        manager.moveCard = true;
    }

    public void ReplaceCard()
    {
        replaceCard.Invoke();
    }
}
