 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardEffectManager : MonoBehaviour
{
    public GameObject paymentMenu, blackScreen;
    public static CardEffectManager instance;
    private Image displayImage;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        paymentMenu.SetActive(false);
        blackScreen.SetActive(false);
        displayImage = paymentMenu.transform.GetChild(0).GetComponent<Image>();
    }
    // Este script se encarga de los momentos en los que tienes que pagar por enemigos o trampas
    public void ActivatePayment(Sprite image)
   {
        displayImage.sprite = image;
        ActivateMenu();
   }

    public void ActivateMenu()
    {
        paymentMenu.SetActive(true);
        blackScreen.SetActive(true);
    }

    public void Payment(bool wantsToPay)
    {
        paymentMenu.SetActive(false);
        blackScreen.SetActive(false);
    }
}
