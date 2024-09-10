using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public GameObject cardObject;
    public Vector2 Location;
    public bool isInHand, isHovered;
    protected bool cardNeeded, soundPlayed;

    public float hoverTimer, otherTimer;

    private CardManager cardManager;
    private SoundManager soundManager;
    public GameManager gameManager;

    public Sprite objectSprite;

    private void Start()
    {
        soundManager = SoundManager.Instance;
        cardManager = CardManager.Instance;
        gameManager = GameManager.Instance;
        cardObject = transform.GetChild(0).gameObject;
        objectSprite = cardObject.GetComponent<CardObject>().myCard.bigImage;
    }

    public void ReplaceCard()
    {
        gameManager.moveCard = true;

        cardManager.CardDiscarded(this);

        gameManager.ChangeState(GameManager.turnState.Movecard);
    }
}
