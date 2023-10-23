using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot: MonoBehaviour
{
    public GameObject cardObject;
    bool isActivated;
    public Vector2 Location;
    public bool isInHand;

    public float hoverTimer;

    private void Update()
    {
        if (hoverTimer > 0)
        {
            hoverTimer -= Time.deltaTime;
        }
    }

    public void ReplaceCard()
    {
        Destroy(cardObject);
        CardManager.Instance.DistributeCard(this);
    }
}
