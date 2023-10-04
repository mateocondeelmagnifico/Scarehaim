using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot: MonoBehaviour
{
    Card mycard;
    bool isActivated;
    private bool isHovered;
    public Vector2 Location;

    public float hoverTimer;

    private void Update()
    {
        if (hoverTimer > 0)
        {
            hoverTimer -= Time.deltaTime;
            isHovered = true;
        }
        else
        {
            isHovered = true;
        }
    }
    public void changeCard(Card newCard)
    {
        mycard = newCard;
    }
}
