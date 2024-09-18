using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public GameObject cardObject, redHighlight;
    private GameObject myHighlight;
    public Vector2 Location;
    public bool isInHand, isHovered;
    protected bool cardNeeded, soundPlayed, countLowered;

    public float hoverTimer, otherTimer;
    public int unavailable;

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

    private void Update()
    {
        if(unavailable > 0 && gameManager.CheckIsInCheckMovement() && !countLowered)
        {
            unavailable--;
            countLowered = true;

            if(unavailable == 1) myHighlight = GameObject.Instantiate(redHighlight, transform.position, transform.rotation);
            if (unavailable == 0) Destroy(myHighlight);
        }

        if(gameManager.currentState == GameManager.turnState.Endturn) countLowered = false;
    }

    public void ReplaceCard()
    {
        gameManager.moveCard = true;

        cardManager.CardDiscarded(this);

        gameManager.ChangeState(GameManager.turnState.Movecard);

        unavailable = 3;
    }
}
