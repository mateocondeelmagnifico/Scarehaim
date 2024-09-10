using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBigImage : MonoBehaviour
{
    [HideInInspector]
    public float hoverTimer, otherTimer;
    public bool isHovered;

    public Image playerIcon;
    public TMPro.TextMeshProUGUI playerText;

    public Sprite bigImage;
    private Sprite baseImage, baseIcon;

    private void Start()
    {
        baseImage = GetComponent<SpriteRenderer>().sprite;
        if(playerIcon != null)
        {
            baseIcon = playerIcon.sprite;
        }
    }

    public void ChangeImageAndIcon(Sprite image, Sprite icon)
    {
        bigImage = image;
        playerIcon.sprite = icon;
    }

    public void ResetImage()
    {
        bigImage = baseImage;
        playerIcon.sprite = baseIcon;
        playerText.text = "";
    }
}

