using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Treat: Card
{
    GameManager manager;
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        //TurnState is changed in the card script
        MoveToHand(card, cardSlot);
    }

    public override void PlayEffect()
    {
        manager = GameManager.Instance;
        manager.powerUpOn = true;
        manager.player.GetComponent<Movement>().hasTreat = true;
        BoardOverlay.instance.ACtivateOverlay("Blue");

        Hand.Instance.PutCardInLimbo(transform.parent.gameObject);
    }

    public override void UndoEffect()
    {
        manager.powerUpOn = false;
        manager.player.GetComponent<Movement>().hasTreat = false;
    }
}
