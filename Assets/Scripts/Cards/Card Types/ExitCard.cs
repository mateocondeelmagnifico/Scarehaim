using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCard: Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        //En el futuro este script cambiara la escena
        //GameManager.Instance.gameObject.GetComponent<SceneManagement>().ChangeScene("");

        GameManager.Instance.gameObject.GetComponent<SceneManagement>().NextScene();
    }

    public override int GetCardType()
    {
        return 4;
    }
}
