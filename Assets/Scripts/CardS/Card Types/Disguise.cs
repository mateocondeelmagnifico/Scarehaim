using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Disguise : Card
{
    public string myName;
    public Sprite icon, onPlayerImage;
    GameManager manager;
    private Movement pMovement;

    public override void Effect(GameObject card, GameObject cardSlot)
    {
        //TurnState is changed in the card script
        MoveToHand(card, cardSlot);
    }

    public override void PlayEffect()
    {
        manager = GameManager.Instance;
        pMovement = manager.player.GetComponent<Movement>();
        manager.powerUpOn = true;
        pMovement.turnsWithcostume = 3;
        pMovement.tempSprite = image;
        pMovement.costumeName = myName;
        pMovement.GetComponent<DisplayBigImage>().ChangeImageAndIcon(onPlayerImage, icon);

        Hand.Instance.PutCardInLimbo(transform.parent.gameObject);
    }

    public override void UndoEffect()
    {
        manager.powerUpOn = true;
        pMovement.turnsWithcostume = 0;
        pMovement.TakeOffCostume();
        manager.powerUpOn = false;
    }
}

