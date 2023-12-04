using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBigImage : MonoBehaviour
{
    [HideInInspector]
    public float hoverTimer, otherTimer;
    public bool isHovered;

    public Sprite bigImage;
    private Sprite baseImage;

    private void Start()
    {
        baseImage = GetComponent<SpriteRenderer>().sprite;
    }

    private void Update()
    {
        if (isHovered)
        {
            hoverTimer += Time.deltaTime;
        }
        else
        {
            hoverTimer = 0;
        }

        if (otherTimer > 0)
        {
            otherTimer -= Time.deltaTime;
        }
        else
        {
            isHovered = false;
        }
    }

    public void ChangeImage(Sprite image)
    {
        bigImage = image;
    }

    public void ResetImage()
    {
        bigImage = baseImage;
    }
}
