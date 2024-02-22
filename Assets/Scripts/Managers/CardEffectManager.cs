 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardEffectManager : MonoBehaviour
{
    //this script also manages paying treats at the end of a stage

    public GameObject paymentMenu, blackScreen, treatSlot, costumeSlot;
    private GameObject newSlot, player;
    [SerializeField] private Transform merrowHand;

    public Transform[] slotPositions;
    private Transform hand;
    public static CardEffectManager Instance { get; private set; }
    private GameManager manager;
    private Image displayImage;
    private TMPro.TextMeshProUGUI explanation;
    private Cost currentCost;
    private Fear playerFear;
    [SerializeField] private Cost endCost;
    private Hand handScript;
    private Sprite mySprite;
    [SerializeField] private Sprite endSprite;

    public bool effectActive, moveHand, isEnding;

    private Vector3 desiredPos, originalPos;

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
        explanation = paymentMenu.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
    }
    private void Start()
    {
        manager = GameManager.Instance;
        player = manager.player;
        hand = Hand.Instance.transform;
        handScript = hand.GetComponent<Hand>();
        playerFear = player.GetComponent<Fear>();

        originalPos = merrowHand.transform.position;
    }

    private void Update()
    {
        //Esto esta solo para mover la mano

        if(moveHand)
        {
            merrowHand.position = Vector3.MoveTowards(merrowHand.position, desiredPos, 5 * Time.deltaTime);

            if(merrowHand.position == desiredPos)
            {
                if (merrowHand.transform.position != originalPos)
                {
                    ActivatePayment(mySprite, currentCost);
                }
                else
                {
                    manager.trapTriggered = false;
                }
                moveHand = false;
            }
        }
    }
    // Este script se encarga de los momentos en los que tienes que pagar por enemigos o trampas
    public void ActivatePayment(Sprite image, Cost whatCost)
   {
        //This displays the payment Window

        mySprite = image;
        currentCost = whatCost;

        displayImage.sprite = mySprite;
        explanation.text = currentCost.explanation;

        paymentMenu.SetActive(true);
        blackScreen.SetActive(true);

        for(int i = 0; i < currentCost.costAmount; i++)
        {
            if(currentCost.CostName == "Treat")
            {
               newSlot = Instantiate(treatSlot);
               newSlot.transform.position = slotPositions[i].position;
               newSlot.transform.parent = blackScreen.transform;
            }

            if (currentCost.CostName == "Costume")
            {
                newSlot = Instantiate(costumeSlot);
                newSlot.transform.position = slotPositions[i].position;
                newSlot.transform.parent = blackScreen.transform;
            }
        }
        effectActive = true;
    }

    public void Payment(bool wantsToPay)
    {
        //called by buttons
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

                if(blackScreen.transform.childCount == 0)
                {
                    canPay = false;
                }

                #region Ending Screen Check
                if (isEnding)
                {
                    //reduce fear by 2 for each treat payed
                    //hardcoded but functional
                    for (int i = 0; i < blackScreen.transform.childCount; i++)
                    {
                        if (blackScreen.transform.GetChild(i).childCount > 0)
                        {
                            playerFear.fear -= 2;
                        }
                    }
                }
                #endregion

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

            if(isEnding)
            {
                handScript.UpdateFear();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
            //Move hand to original position;
            moveHand = true;
            desiredPos = originalPos;
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

    private void DiscardCards(string cardType, int amount)
    {
        if(hand.childCount > 0)
        {
            for (int i = 0; i < hand.childCount; i++)
            {
                if (hand.GetChild(i).GetChild(0).tag == cardType && amount > 0 && hand.childCount > 0)
                {
                    Destroy(hand.GetChild(i).gameObject);
                    amount--;
                    handScript.DeterminePosition();
                }
            }
        }
    }
    private void CheckConsequence(string name, int howMuch)
    {
        if (name == "Fear")
        {
            playerFear.fear += howMuch;
        }

        if (name == "Treat")
        {
            DiscardCards(name, howMuch);
        }

        if (name == "Costume")
        {
            DiscardCards(name, howMuch);
        }
    }

    public void InformMoveHand(Vector3 cardPos, Sprite image, Cost whatCost)
    {
        moveHand = true;

        desiredPos = cardPos;

        currentCost = whatCost;

        mySprite = image;
    }

    public void ActivateFinalScreen()
    {
        isEnding = true;
        ActivatePayment(endSprite, endCost);
    }
}
