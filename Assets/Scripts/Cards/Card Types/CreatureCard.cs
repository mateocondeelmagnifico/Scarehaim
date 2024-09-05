using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature", menuName = "Cards/Creature")]

public class CreatureCard : Card
{
    public Cost myCost;
    public string disguiseToIgnore;

    public GameManager manager;
    public Transform player;
    [HideInInspector] public CreatureContainer container;

    public override void Effect(GameObject card, GameObject cardSlot)
    { 

        if (player.GetComponent<Movement>().costumeName == disguiseToIgnore)
        {
            if(manager.currentState == GameManager.turnState.CheckCardEffect)
            {
                manager.ChangeState(GameManager.turnState.Movecard);
                container.isDone = true;
                return;
            }
            else
            {
                return;
            }
        }

        if (container.isDone)
        {
            manager.ChangeState(GameManager.turnState.Movecard);
            return;
        }

        CardEffectManager.Instance.ActivatePayment(image, myCost);
        player.GetComponent<Movement>().hasMoved = false;
        container.isDone = true;
    }
}
