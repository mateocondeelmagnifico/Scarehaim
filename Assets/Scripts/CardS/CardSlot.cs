using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public GameObject cardObject;
    bool isActivated;
    public Vector2 Location;
    public bool isInHand, isHovered;
    private bool cardNeeded;

    public float hoverTimer, otherTimer;

    private CardManager cardManager;
    public GameManager gameManager;

    private void Start()
    {
        cardManager = CardManager.Instance;
        gameManager = GameManager.Instance;
    }
    private void Update()
    {
        if(otherTimer > 0)
        {
            otherTimer -= Time.deltaTime;
        }
        else
        {
            isHovered = false;
        }

        if (isHovered)
        {
            hoverTimer += Time.deltaTime;
        }
        else
        {
            hoverTimer = 0;
        }
    }

    public void ReplaceCard()
    {
        gameManager.selectedCard = cardObject;
        gameManager.moveCard = true;

        cardManager.CardDiscarded(this);

        gameManager.currentState = GameManager.turnState.Movecard;
    }

}
