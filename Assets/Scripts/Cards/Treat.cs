using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Treat : Card
{
    GameManager manager;
    public override void Effect(GameObject player, GameObject cardSlot)
    {
        GameObject newSlot = cardSlot;
        newSlot = GameObject.Instantiate(cardSlot);
        newSlot.GetComponent<CardSlot>().enabled = false;
        newSlot.AddComponent<CardSlotHand>();
        newSlot.GetComponent<CardSlotHand>().enabled = false;

        manager = GameManager.Instance;

        manager.selectedCardSlot = newSlot;
        manager.moveCard = true;
    }
}
