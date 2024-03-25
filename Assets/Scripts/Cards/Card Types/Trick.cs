using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trick : Card
{
    public Cost myCost;
    private Transform player;
    private Movement playerMovement;
    private GameManager manager;
    public GameObject myIndicator;

    private void Start()
    {
        manager = GameManager.Instance;
        player = manager.player.transform;
        playerMovement = player.gameObject.GetComponent<Movement>();
    }

    private void Update()
    {
        //Check if player has triggered the trap
        if (player.position.x == transform.position.x && player.position.y == transform.position.y)
        {
            if (player.position.x == playerMovement.tempDestination.x && player.position.y == playerMovement.tempDestination.y);
            else
            {
                manager.trapTriggered = true;
                if (manager.currentState == GameManager.turnState.CheckCardEffect)
                {
                    Effect(gameObject, null);
                    Destroy(gameObject);
                }
            }
            
        }
    }
    public override void Effect(GameObject card, GameObject cardSlot)
    {
        CardEffectManager.Instance.InformMoveHand(new Vector3(transform.position.x, transform.position.y, 0),image, myCost);
        if(myIndicator != null) Destroy(myIndicator);
    }
}
