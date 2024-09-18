using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Disguise", menuName = "Cards/Disguise")]
public class Disguise : Card
{
    public string myName;
    public Sprite icon, onPlayerImage;
    private Movement pMovement;

    public override void Effect(GameObject card, GameObject cardSlot)
    {
        //TurnState is changed in the card script
        myCardObject.GetComponent<CardObject>().MoveToHand(cardSlot);
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
        GameManager.Instance.powerUpOn = true;
        pMovement.turnsWithcostume = 0;
        pMovement.TakeOffCostume();
        GameManager.Instance.powerUpOn = false;
    }
}

