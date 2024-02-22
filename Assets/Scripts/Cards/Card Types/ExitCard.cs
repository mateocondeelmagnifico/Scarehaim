using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCard: Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        //GameManager.Instance.gameObject.GetComponent<SceneManagement>().NextScene();
        CardEffectManager.Instance.ActivateFinalScreen();
        Destroy(this);
    }
}
