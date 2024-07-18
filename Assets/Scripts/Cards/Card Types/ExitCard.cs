using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Exit", menuName = "Cards/Exit")]
public class ExitCard: Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        CardEffectManager.Instance.ActivateFinalScreen();
    }
}
