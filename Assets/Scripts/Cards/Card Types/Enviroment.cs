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
        GameManager.Instance.currentState = GameManager.turnState.ApplyCardEffect;
        DiscardCard();
    }

    public override int GetCardType()
    {
        return 3;
    }
}
