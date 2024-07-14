using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="Treat", menuName = "Cards/Treat")]
public class Treat: Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        //TurnState is changed in the card script
        myObject.MoveToHand(cardSlot);
    }

    public override void PlayEffect(GameManager manager)
    {
        manager.powerUpOn = true;
        manager.player.GetComponent<Movement>().hasTreat = true;
        BoardOverlay.instance.ACtivateOverlay("Blue");
    }

    public override void UndoEffect()
    {
        GameManager.Instance.powerUpOn = false;
        GameManager.Instance.player.GetComponent<Movement>().hasTreat = false;
    }
}
