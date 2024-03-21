using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCard : Card
{
    public Cost myCost;
    [SerializeField] private string disguiseToIgnore;

    private GameManager manager;
    private Transform player;
    private Movement pMovement;

    private bool isDone;

    private void Start()
    {
        manager = GameManager.Instance;
        player = manager.player.transform;
        pMovement = player.GetComponent<Movement>();
        isDone = false;
    }
    private void Update()
    {
        if(manager.currentState == GameManager.turnState.Moving && !(player.position.x == transform.position.x || player.position.y == transform.position.y))
        {
            isDone = false;
        }

        if(player.position.x == transform.position.x && player.position.y == transform.position.y && !isDone && !pMovement.hasTreat && manager.currentState == GameManager.turnState.Moving)
        {
            pMovement.destination = transform.position;
            pMovement.myPos = transform.parent.GetComponent<CardSlot>().Location;
            manager.selectedCardSlot = transform.parent.gameObject;
            manager.ChangeState(GameManager.turnState.CheckCardEffect);
            manager.cardInformed = true;
            Effect(null, null);
        }
    }

    public override void Effect(GameObject card, GameObject cardSlot)
    {
        if (player.GetComponent<Movement>().costumeName == disguiseToIgnore)
        {
            if(manager.currentState == GameManager.turnState.CheckCardEffect)
            {
                manager.ChangeState(GameManager.turnState.Movecard);
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
            manager.ChangeState(GameManager.turnState.Movecard);
            return;
        }

        CardEffectManager.Instance.ActivatePayment(image, myCost);
        player.GetComponent<Movement>().hasMoved = false;
        isDone = true;
    }
}
