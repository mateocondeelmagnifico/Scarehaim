using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreatureCard : Card
{

    public override void Effect(GameObject player)
    {
        player.GetComponent<Fear>().fear++;
    }
}
