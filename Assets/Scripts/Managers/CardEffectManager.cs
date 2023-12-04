 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardEffectManager : MonoBehaviour
{
    public GameObject paymentMenu, blackScreen, treatSlot, costumeSlot;
    private GameObject newSlot, player;

    public Transform[] slotPositions;
    private Transform hand;
    public static CardEffectManager Instance { get; private set; }
    private GameManager manager;
    private Image displayImage;
    private TMPro.TextMeshProUGUI explanation;
    private Cost currentCost;

    public bool effectActive;

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
        explanation = paymentMenu.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>();
    }
    private void Start()
    {
        manager = GameManager.Instance;
        player = manager.player;
        hand = manager.hand;
    }
    // Este script se encarga de los momentos en los que tienes que pagar por enemigos o trampas
    public void ActivatePayment(Sprite image, Cost whatCost)
   {
        //This displays the payment Window
        displayImage.sprite = image;
        explanation.text = whatCost.explanation;

        paymentMenu.SetActive(true);
        blackScreen.SetActive(true);

        for(int i = 0; i < whatCost.costAmount; i++)
        {
            if(whatCost.CostName == "Treat")
            {
               newSlot = Instantiate(treatSlot);
               newSlot.transform.position = slotPositions[i].position;
               newSlot.transform.parent = blackScreen.transform;
            }

            if (whatCost.CostName == "Costume")
            {
                newSlot = Instantiate(costumeSlot);
                newSlot.transform.position = slotPositions[i].position;
                newSlot.transform.parent = blackScreen.transform;
            }
        }
        currentCost = whatCost;
        effectActive = true;
    }

    public void Payment(bool wantsToPay)
    {
        if(manager.currentState != GameManager.turnState.ReplaceCard)
        {
            //This checks if you selected pay or don't pay
            if (wantsToPay)
            {
                bool canPay = true;
                //this is to check if you have payed
                for (int i = 0; i < blackScreen.transform.childCount; i++)
                {
                    if (blackScreen.transform.GetChild(i).childCount < 1)
                    {
                        canPay = false;
                    }
                }

                if (canPay)
                {
                    for (int i = 0; i < blackScreen.transform.childCount; i++)
                    {
                        Destroy(blackScreen.transform.GetChild(i).gameObject);
                    }
                    CheckConsequence(currentCost.reward, currentCost.rewardAmount);

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

                //This is for discarding cards in your hand

                CheckConsequence(currentCost.consequenceName, currentCost.consequenceAmount);

                if (currentCost.secondConsequenceName != null)
                {
                    CheckConsequence(currentCost.secondConsequenceName, currentCost.secondConsequenceAmount);
                    DeactivateMenu();
                }
                else
                {
                    DeactivateMenu();
                }
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

        if(manager.trapTriggered)
        {
            //If you triggered a card
            manager.trapTriggered = false;
        }
        else
        {
            //If you trigerred a creature
            if(manager.currentState == GameManager.turnState.CheckMovement)
            {
                //This is in case you trigger a creature with a costume on
                manager.currentState = GameManager.turnState.ReplaceCard;
            }
            else
            {
                manager.currentState = GameManager.turnState.Endturn;
            }
        }
        effectActive = false;
    }

    private void discardCards(string cardType, int amount)
    {
        for(int i = 0; i < hand.childCount; i++)
        {
            if(hand.GetChild(i).GetChild(0).tag == cardType && amount > 0)
            {
                Destroy(hand.GetChild(i).gameObject);
                amount--;
            }
        }
    }
    private void CheckConsequence(string name, int howMuch)
    {
        if (name == "Fear")
        {
            player.GetComponent<Fear>().fear += howMuch;
        }

        if (name == "Treat")
        {
            discardCards(name, howMuch);
        }

        if (name == "Costume")
        {
            discardCards(name, howMuch);
        }
    }
}
