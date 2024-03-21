using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enviroment : Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        if (CardManager.Instance.cardsUntilExit > 0)
        {
            CardManager.Instance.cardsUntilExit--;
        }
        GameManager.Instance.ChangeState(GameManager.turnState.ApplyCardEffect);
        DiscardCard();
    }
}
