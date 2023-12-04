using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Disguise : Card
{
    public string myName;

    public override void Effect(GameObject card, GameObject cardSlot)
    {
        //TurnState is changed in the card script
        MoveToHand(card, cardSlot);
    }

    public override void PlayEffect()
    {
        GameManager manager = GameManager.Instance;
        manager.powerUpOn = true;
        manager.player.GetComponent<Movement>().turnsWithcostume = 3;
        manager.player.GetComponent<Movement>().tempSprite = image;
        manager.player.GetComponent<Movement>().costumeName = myName;
        manager.player.GetComponent<DisplayBigImage>().ChangeImage(bigImage);
  
        Destroy(this.gameObject.transform.parent.gameObject);
    }

    public override int GetCardType()
    {
        return 1;
    }
}

