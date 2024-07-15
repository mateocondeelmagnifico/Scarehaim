using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureContainer : CardObject
{
    public bool isDone;
    private Movement pMovement;
    private string chosenDisguise;

    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.player.transform;
        (myCard as CreatureCard).player = player;
        pMovement = player.GetComponent<Movement>();
        isDone = false;
        chosenDisguise = (myCard as CreatureCard).disguiseToIgnore;
        (myCard as CreatureCard).container = this;
    }

    void Update()
    {
        if (gameManager.currentState == GameManager.turnState.Moving && !(player.position.x == transform.position.x || player.position.y == transform.position.y))
        {
            isDone = false;
        }

        if (player.position.x == transform.position.x && player.position.y == transform.position.y && !isDone && !pMovement.hasTreat && gameManager.currentState == GameManager.turnState.Moving)
        {
            if (player.GetComponent<Movement>().costumeName != chosenDisguise)
            {
                pMovement.destination = transform.position;
                pMovement.myPos = transform.parent.GetComponent<CardSlot>().Location;
                gameManager.selectedCardSlot = transform.parent.gameObject;
                gameManager.ChangeState(GameManager.turnState.CheckCardEffect);
                gameManager.cardInformed = true;
                myCard.Effect(null, null);
            }
        }
    }
}
