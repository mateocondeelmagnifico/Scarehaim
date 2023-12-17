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
    private bool cardNeeded, soundPlayed;

    public float hoverTimer, otherTimer;

    private CardManager cardManager;
    public GameManager gameManager;

    private void Start()
    {
        cardManager = CardManager.Instance;
        gameManager = GameManager.Instance;
        cardObject = transform.GetChild(0).gameObject;
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
            soundPlayed = false;
        }

        if (isHovered)
        {
            hoverTimer += Time.deltaTime;
            if(!soundPlayed && hoverTimer > 0.8f)
            {
                SoundManager.Instance.PlaySound("Card Hovered");
                soundPlayed = true;
            }
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
