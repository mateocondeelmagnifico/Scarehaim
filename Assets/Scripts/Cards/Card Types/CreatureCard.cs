using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreatureCard : Card
{
    public bool conditionMet;

    public override void Effect(GameObject player, GameObject cardSlot)
    {
        player.GetComponent<Fear>().fear++;
    }

    public override int GetCardType()
    {
        return 2;
    }
}
