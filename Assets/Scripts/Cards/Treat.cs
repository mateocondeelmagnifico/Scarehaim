using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Treat: Card
{
    GameManager manager;
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        MoveToHand(card, cardSlot);
    }
}
