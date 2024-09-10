using UnityEngine;


public class TrickContainer : CardObject
{
    private Trick myTrick;
    private Movement playerMovement;
    private GameManager manager;
    [HideInInspector] public GameObject myIndicator;

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
            if (player.position.x != playerMovement.tempDestination.x && player.position.y != playerMovement.tempDestination.y)
            {
                manager.trapTriggered = true;
                if (manager.currentState == GameManager.turnState.CheckCardEffect)
                {
                    CardEffectManager.Instance.InformMoveHand(new Vector3(transform.position.x, transform.position.y, 0), myTrick.image, myTrick.myCost);
                    TextManager.Instance.inTrap = true;
                    if (myIndicator != null) Destroy(myIndicator);
                    Destroy(gameObject);
                }
            }
        }
    }

}
