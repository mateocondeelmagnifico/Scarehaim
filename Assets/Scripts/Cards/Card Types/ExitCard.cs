using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCard: Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
         SceneManagement.Instance.NextScene();
    }

    public override int GetCardType()
    {
        return 4;
    }
}
