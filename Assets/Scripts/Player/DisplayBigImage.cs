using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBigImage : MonoBehaviour
{
    [HideInInspector]
    public float hoverTimer, otherTimer;
    public bool isHovered;

    public Sprite bigImage;

    private void Update()
    {
        if (otherTimer > 0)
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
}
