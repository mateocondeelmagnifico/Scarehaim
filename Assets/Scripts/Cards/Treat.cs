using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treat : Card
{
    public override void Effect(GameObject player, GameObject cardSlot)
    {
        GameObject newSlot = cardSlot;
        newSlot = GameObject.Instantiate(cardSlot);
        newSlot.GetComponent<CardSlot>().enabled = false;
        newSlot.AddComponent<CardSlotHand>();
    }
}
