using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreatureCard : Card
{
    public bool hasPayed;

    public override void Effect(GameObject player, GameObject cardSlot)
    {
        player.GetComponent<Fear>().fear++;
        hasPayed = false;
    }

    public override int GetCardType()
    {
        return 2;
    }
}
