using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trick : Card
{
    public Cost myCost;
    private Transform player;
    private GameManager manager;

    private void Start()
    {
        manager = GameManager.Instance;
        player = manager.player.transform;
    }

    private void Update()
    {
        //Check if player has triggered the trap
        if (player.position == transform.position)
        {
            manager.trapTriggered = true;
            if(manager.currentState == GameManager.turnState.CheckCardEffect)
            {
                Effect(gameObject, null);
                Destroy(gameObject);
            }
        }
    }
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        CardEffectManager.Instance.ActivatePayment(image, myCost.costAmount, myCost.mainCostName, myCost.explanation);
        CardEffectManager.Instance.setConsequence(myCost.consequenceAmount, myCost.consequenceName);
    }
}
