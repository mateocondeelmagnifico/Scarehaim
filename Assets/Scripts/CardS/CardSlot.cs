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

        if (cardObject == null)
        {
            cardManager.CardDiscarded(this);
        }
    }

    public void ReplaceCard()
    {
        Destroy(cardObject);
        Debug.Log(1);
    }

}
