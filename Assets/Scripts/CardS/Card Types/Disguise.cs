using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Disguise", menuName = "Cards/Disguise")]
public class Disguise : Card
{
    public string myName;
    public Sprite icon, onPlayerImage;
    GameManager manager;
    private Movement pMovement;

    public override void Effect(GameObject card, GameObject cardSlot)
    {
        //TurnState is changed in the card script
        myObject.MoveToHand(cardSlot);
        //MoveToHand(card, cardSlot);
    }

    public override void PlayEffect(GameManager manager)
    {
        pMovement = manager.player.GetComponent<Movement>();
        manager.powerUpOn = true;
        pMovement.turnsWithcostume = 3;
        pMovement.tempSprite = image;
        pMovement.costumeName = myName;
        pMovement.GetComponent<DisplayBigImage>().ChangeImageAndIcon(onPlayerImage, icon);
    }

    public override void UndoEffect()
    {
        manager.powerUpOn = true;
        pMovement.turnsWithcostume = 0;
        pMovement.TakeOffCostume();
        manager.powerUpOn = false;
    }
}

