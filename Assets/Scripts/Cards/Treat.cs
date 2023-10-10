using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Treat : Card
{
    public override void Effect(GameObject player, GameObject cardSlot)
    {
        GameObject newSlot = cardSlot;
        newSlot = GameObject.Instantiate(cardSlot);
        newSlot.GetComponent<CardSlot>().enabled = false;
        newSlot.AddComponent<CardSlotHand>();
        newSlot.GetComponent<CardSlotHand>().mycard = this;

        GameManager.Instance.selectedCard = newSlot;
        GameManager.Instance.moveCard = true;
    }
}
