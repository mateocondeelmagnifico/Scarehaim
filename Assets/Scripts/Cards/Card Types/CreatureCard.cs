using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCard : Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        GameManager.Instance.player.GetComponent<Fear>().fear++;
        CardEffectManager.Instance.ActivatePayment(image);
    }

    public override int GetCardType()
    {
        return 2;
    }
}
