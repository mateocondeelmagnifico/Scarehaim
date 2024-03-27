using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardOverlay : MonoBehaviour
{
    public static BoardOverlay instance { get; set; }
    private Image overlay;
    private Color myColor;
    private bool isActive;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            myColor = Color.white;
            overlay = GetComponent<Image>();
        }
        else Destroy(this.gameObject);
    }
    private void Update()
    {
        if(isActive)
        {
            if (overlay.color.a < 0.3f) overlay.color = new Color(myColor.r, myColor.g, myColor.b, overlay.color.a + Time.deltaTime);
            else if (overlay.color.r != myColor.r || overlay.color.g != myColor.g || overlay.color.b != myColor.b) overlay.color = new Color(myColor.r, myColor.g, myColor.b, overlay.color.a);
        }
        else
        {
            if (overlay.color.a > 0) overlay.color = new Color(myColor.r, myColor.g, myColor.b, overlay.color.a - Time.deltaTime);
        }
    }

    public void ACtivateOverlay(string color)
    {
        isActive = true;

        if (color == "Yellow") myColor = Color.yellow;
        if (color == "Green") myColor = Color.green;
        if (color == "Blue") myColor = Color.blue;
        if (color == "Red") myColor = Color.red;
    }

    public void DeactivatOverlay()
    {
        isActive = false;
    }
}

