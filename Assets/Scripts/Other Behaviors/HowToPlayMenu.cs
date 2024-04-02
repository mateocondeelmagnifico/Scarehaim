using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayMenu : MonoBehaviour
{
    [SerializeField] private Image imagen;
    [SerializeField] private TMPro.TextMeshProUGUI texto, titulo, contador;

    private int currentTutorial;
    [SerializeField] private TextAndImage[] tutorials;

    public void ChangeTutorial(int amount)
    { 
        //Called by buttons

        if(currentTutorial + amount >= tutorials.Length || currentTutorial + amount < 0)
        {
            return;
        }
        else
        {
            currentTutorial += amount;
        }

        titulo.text = tutorials[currentTutorial].title;
        texto.text = tutorials[currentTutorial].text;
        contador.text = (currentTutorial + 1).ToString() + "/" + tutorials.Length.ToString();
        imagen.sprite = tutorials[currentTutorial].image;
    }
}
