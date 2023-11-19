using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enviroment : Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        CardManager.Instance.cardsUntilExit--;

        DiscardCard();
    }

    public override int GetCardType()
    {
        return 3;
    }
}
