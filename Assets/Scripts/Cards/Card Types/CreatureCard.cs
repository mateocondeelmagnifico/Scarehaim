using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCard : Card
{
    public Cost myCost;

    private GameManager manager;

    private bool isDone;

    private void Start()
    {
        manager = GameManager.Instance;
    }
    private void Update()
    {
        if(manager.currentState == GameManager.turnState.CheckMovement)
        {
            isDone = false;
        }
    }

    public override void Effect(GameObject card, GameObject cardSlot)
    {
        if(isDone) return; 
        CardEffectManager.Instance.ActivatePayment(image, myCost);
        isDone = true;
    }

    public override int GetCardType()
    {
        return 2;
    }
}
