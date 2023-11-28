using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Treat: Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        //TurnState is changed in the card script
        MoveToHand(card, cardSlot);
    }

    public override void PlayEffect()
    {
        GameManager.Instance.powerUpOn = true;
        GameManager.Instance.player.GetComponent<Movement>().hasTreat = true;
        Destroy(this.gameObject.transform.parent.gameObject);
    }

    public override int GetCardType()
    {
        return 1;
    }
}
