using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBigImage : MonoBehaviour
{
    [HideInInspector]
    public float hoverTimer, otherTimer;
    public bool isHovered;

    public Image playerIcon, bigIcon;
    public TMPro.TextMeshProUGUI playerText, bigText, bigFear;

    public Sprite bigImage;
    private Sprite baseImage, baseIcon;

    private void Start()
    {
        baseImage = GetComponent<SpriteRenderer>().sprite;
        if(playerIcon != null)
        {
            baseIcon = playerIcon.sprite;
            DeactivateIcons();
        }
    }

    private void Update()
    {
        if (isHovered)
        {
            hoverTimer += Time.deltaTime;
            if(hoverTimer > 0.4f && playerIcon != null)
            {
                //activate Ui icons in big display
                bigText.text = playerText.text;
                bigFear.text = GetComponent<Fear>().hope.ToString();
                bigIcon.sprite = playerIcon.sprite;
                bigIcon.enabled = true;
            }
        }
        else
        {
            hoverTimer = 0;
        }

        if (otherTimer > 0)
        {
            otherTimer -= Time.deltaTime;
            if(otherTimer < 0.19f && isHovered)
            {
                //Deactivate UI Icons in big display
                DeactivateIcons();
            }
        }
        else
        { 
            isHovered = false;
            //Deactivate UI Icons in big display
            DeactivateIcons();
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

    private void DeactivateIcons()
    {
        if (playerIcon != null)
        {
            bigText.text = "";
            bigIcon.enabled = false;
            bigFear.text = "";
        }
    }
}

