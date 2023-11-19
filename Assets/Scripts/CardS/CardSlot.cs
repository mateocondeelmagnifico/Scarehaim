using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public GameObject cardObject;
    bool isActivated;
    public Vector2 Location;
    public bool isInHand;
    private bool cardNeeded;

    public float hoverTimer;

    private CardManager cardManager;

    private void Start()
    {
        cardManager = CardManager.Instance;
    }
    private void Update()
    {
        if (hoverTimer > 0)
        {
            hoverTimer -= Time.deltaTime;
        }
    }

    public void ReplaceCard()
    {
        if (cardObject != null)
        {
            Destroy(cardObject);
        }
        
        cardManager.CardDiscarded(this);

        GameManager.Instance.currentState = GameManager.turnState.Movecard;
    }

}
