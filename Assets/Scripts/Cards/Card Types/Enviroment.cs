using UnityEngine;

[CreateAssetMenu(fileName = "Enviroment", menuName = "Cards/Enviroment")]
public class Enviroment : Card
{
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        if (CardManager.Instance.cardsUntilExit > 0)
        {
            CardManager.Instance.cardsUntilExit--;
        }
        GameManager.Instance.ChangeState(GameManager.turnState.ApplyCardEffect);
        myCardObject.GetComponent<CardObject>().DiscardCard();
    }
}
