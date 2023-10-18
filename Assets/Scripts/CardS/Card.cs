using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card 
{
    public string name;

    public Sprite image;

    public virtual void Effect(GameObject card, GameObject cardSlot)
    {

    }
    public virtual void MoveToHand(GameObject card, GameObject cardSlot) 
    {
        GameManager manager = GameManager.Instance;
        GameObject newSlot = GameObject.Instantiate(cardSlot);

        newSlot.GetComponent<CardSlotHand>().enabled = false;
        card.transform.parent = newSlot.transform;

        manager.selectedCardSlot = newSlot;
        manager.moveCard = true;
    }

}
