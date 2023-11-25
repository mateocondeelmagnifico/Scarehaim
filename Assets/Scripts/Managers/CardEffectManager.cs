 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardEffectManager : MonoBehaviour
{
    public GameObject paymentMenu, blackScreen, treatSlot, costumeSlot;
    private GameObject newSlot, player;

    public Transform[] slotPositions;
    public static CardEffectManager Instance { get; private set; }
    private GameManager manager;
    private Image displayImage;
    public bool effectActive;

    private string consequenceName;
    private int consequenceAmount;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        paymentMenu.SetActive(false);
        blackScreen.SetActive(false);
        displayImage = paymentMenu.transform.GetChild(0).GetComponent<Image>();
    }
    private void Start()
    {
        manager = GameManager.Instance;
        player = manager.player;
    }
    // Este script se encarga de los momentos en los que tienes que pagar por enemigos o trampas
    public void ActivatePayment(Sprite image, int amount, string type)
   {
        //This displays the payment Window
        displayImage.sprite = image;

        paymentMenu.SetActive(true);
        blackScreen.SetActive(true);

        for(int i = 0; i < amount; i++)
        {
            if(type == "Treat")
            {
               newSlot = Instantiate(treatSlot);
               newSlot.transform.position = slotPositions[i].position;
               newSlot.transform.parent = blackScreen.transform;
            }

            if (type == "Costume")
            {
                newSlot = Instantiate(costumeSlot);
                newSlot.transform.position = slotPositions[i].position;
                newSlot.transform.parent = blackScreen.transform;
            }
        }
        effectActive = true;
    }

    public void setConsequence(int amount, string type)
    {
        consequenceAmount = amount;
        consequenceName = type;
    }

    public void Payment(bool wantsToPay)
    {
        if(wantsToPay)
        {
            bool canPay = true;
            //this is to check if you have payed
            for(int i = 0; i < blackScreen.transform.childCount; i++) 
            { 
                if(blackScreen.transform.GetChild(i).childCount < 1)
                {
                    canPay = false;
                }
            }

            if (canPay)
            {
                for(int i = 0; i < blackScreen.transform.childCount; i++)
                {
                    Destroy(blackScreen.transform.GetChild(i).gameObject);
                }
                
                DeactivateMenu();
            }
        }
        else
        {
            //this returns the cards to your hand
            for (int i = 0; i < blackScreen.transform.childCount; i++)
            {
                if (blackScreen.transform.GetChild(i).childCount != 0)
                {
                    blackScreen.transform.GetChild(i).transform.GetChild(0).position = blackScreen.transform.GetChild(i).transform.GetChild(0).GetComponent<CardSlotHand>().startingPos;
                    blackScreen.transform.GetChild(i).transform.GetComponentInChildren<CardSlotHand>().isPayment = false;
                    blackScreen.transform.GetChild(i).transform.GetChild(0).parent = null;
                }
            }

            if (consequenceName == "Fear")
            {
                player.GetComponent<Fear>().fear += consequenceAmount;

                DeactivateMenu();
            }
        }
    }

    private void DeactivateMenu()
    {
        for (int i = 0; i < blackScreen.transform.childCount; i++)
        {
            Destroy(blackScreen.transform.GetChild(i).gameObject);
        }

        paymentMenu.SetActive(false);
        blackScreen.SetActive(false);
        manager.currentState = GameManager.turnState.Endturn;
        effectActive = false;
    }
}
