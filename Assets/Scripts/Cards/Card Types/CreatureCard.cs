using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCard : Card
{
    public Cost myCost;
    [SerializeField] private string disguiseToIgnore;

    private GameManager manager;
    private Transform player;

    private bool isDone;

    private void Start()
    {
        manager = GameManager.Instance;
        player = manager.player.transform;
    }
    private void Update()
    {
        if(manager.currentState == GameManager.turnState.Moving && !(player.position.x == transform.position.x && player.position.y == transform.position.y))
        {
            isDone = false;
        }

        if(player.position.x == transform.position.x && player.position.y == transform.position.y && !isDone && manager.currentState == GameManager.turnState.CheckMovement)
        {
            Effect(null, null);
        }
    }

    public override void Effect(GameObject card, GameObject cardSlot)
    {
        if (player.GetComponent<Movement>().costumeName == disguiseToIgnore)
        {
            if(manager.currentState == GameManager.turnState.CheckCardEffect)
            {
                manager.currentState = GameManager.turnState.Movecard;
                isDone = true;
                return;
            }
            else
            {
                return;
            }
        }

        if (isDone)
        {
            manager.currentState = GameManager.turnState.Movecard;
            return;
        }

        //manager.slotToReplaceOld = manager.slotToReplaceNew;
        //manager.slotToReplaceNew = null;
        CardEffectManager.Instance.ActivatePayment(image, myCost);
        player.GetComponent<Movement>().hasMoved = false;
        isDone = true;
    }
}
