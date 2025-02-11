using UnityEngine;
using UnityEngine.UI;

public class HowToPlayMenu : MonoBehaviour
{
    [SerializeField] private Image imagen;
    [SerializeField] private TMPro.TextMeshProUGUI texto, titulo, contador;

    private int currentTutorial;
    [SerializeField] private TextAndImage[] tutorials;

    [SerializeField] private GameObject arrowLeft, arrowRight;

    private void Start()
    {
        arrowLeft.SetActive(false);
    }

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

        #region Disable Arrows
        if (currentTutorial == 0) arrowLeft.SetActive(false);
        else arrowLeft.SetActive(true);

        if (currentTutorial == (tutorials.Length - 1)) arrowRight.SetActive(false);
        else arrowRight.SetActive(true);
        #endregion

        //Esto está al revés porque soy bobo
        titulo.text = tutorials[currentTutorial].text.GetLocalizedString();
        texto.text = tutorials[currentTutorial].title.GetLocalizedString();
        contador.text = (currentTutorial + 1).ToString() + "/" + tutorials.Length.ToString();
        imagen.sprite = tutorials[currentTutorial].image;
    }
}
