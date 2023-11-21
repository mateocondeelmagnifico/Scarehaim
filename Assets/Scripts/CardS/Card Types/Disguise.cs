using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Disguise : Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        //TurnState is changed in the card script
        MoveToHand(card, cardSlot);
    }

    public override int GetCardType()
    {
        return 1;
    }
}

