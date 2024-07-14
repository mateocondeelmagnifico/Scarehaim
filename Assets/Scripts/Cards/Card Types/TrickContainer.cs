using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TrickContainer : CardObject
{
    private Trick myTrick;
    private Movement playerMovement;
    private GameManager manager;
    [HideInInspector] public GameObject myIndicator;

    //Constructor
    public TrickContainer(Card CardScript, Transform discardLocation) : base(CardScript, discardLocation)
    {
        myTrick = CardScript as Trick;
    }

    private void Start()
    {
        myTrick = myCard as Trick;
        manager = GameManager.Instance;
        player = manager.player.transform;
        playerMovement = player.gameObject.GetComponent<Movement>();
    }

    private void Update()
    {
        if (player.position.x == transform.position.x && player.position.y == transform.position.y)
        {
            if (player.position.x == playerMovement.tempDestination.x && player.position.y == playerMovement.tempDestination.y) ;
            else
            {
                manager.trapTriggered = true;
                if (manager.currentState == GameManager.turnState.CheckCardEffect)
                {
                    CardEffectManager.Instance.InformMoveHand(new Vector3(transform.position.x, transform.position.y, 0), myTrick.image, myTrick.myCost);
                    if (myIndicator != null) Destroy(myIndicator);
                    Destroy(gameObject);
                }
            }
        }
    }

}
