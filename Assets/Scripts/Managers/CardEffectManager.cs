using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardEffectManager : MonoBehaviour
{
    //this script also manages paying treats at the end of a stage

    public GameObject paymentMenu, blackScreen, blackscreen2, treatSlot, costumeSlot, paymentButtons, bigButton, hand1, hand2, gameWonMenu;
    private GameObject newSlot, player;
    [SerializeField] private Transform merrowHand, fearCounter;

    public Transform slotPosition;
    private Transform hand;
    public static CardEffectManager Instance { get; private set; }
    private GameManager manager;
    private TextManager textManager;
    [SerializeField] MouseManager mouseManager;
    [SerializeField] private Image displayImage, tryPayButton;
    [SerializeField] private TMPro.TextMeshProUGUI explanation, fearText, fearModTxt;
    private Cost currentCost;
    private Fear playerFear;
    private SoundManager soundManager;
    [SerializeField] private Cost endCost;
    [SerializeField] private Button optionsButton;
    private Hand handScript;
    private BoardOverlay overlay;
    private Sprite mySprite;
    [SerializeField] private Sprite endSprite;

    public bool effectActive, moveHand, isEnding, hasLost;

    private Vector3 desiredPos, originalPos, originalPos2;

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
    }
    private void Start()
    {
        manager = GameManager.Instance;
        player = manager.player;
        hand = Hand.Instance.transform;
        textManager = TextManager.Instance;
        handScript = hand.GetComponent<Hand>();
        playerFear = player.GetComponent<Fear>();
        fearText = fearCounter.GetComponent<TMPro.TextMeshProUGUI>();
        overlay = BoardOverlay.instance;
        soundManager = SoundManager.Instance;
        tryPayButton = paymentButtons.transform.GetChild(0).GetComponent<Image>();
        originalPos2 = fearModTxt.transform.position;

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
                    hand1.SetActive(true);
                    hand2.SetActive(false);
                    overlay.DeactivatOverlay();
                }
                moveHand = false;
            }
        }
    }
    // Este script se encarga de los momentos en los que tienes que pagar por enemigos o trampas
    public void ActivatePayment(Sprite image, Cost whatCost)
    {
        //This displays the payment Window
        #region Set variables and activate things
        mySprite = image;
        currentCost = whatCost;

        displayImage.sprite = mySprite;
        explanation.text = currentCost.explanation;

        mouseManager.hover2Pos = null;

        paymentMenu.SetActive(true);
        blackScreen.SetActive(true);
        blackscreen2.SetActive(true);
        optionsButton.enabled = false;
        DisplayFear();
        handScript.MoveHand(0);
        SetFearModTxt(whatCost.consequenceAmount);
        #endregion

        #region Spawn Cost Slots
        Vector3 offset = Vector3.zero;
        GameObject slotPrefab = null;

        if (currentCost.CostName == "Costume") slotPrefab = costumeSlot;
        if (currentCost.CostName == "Treat") slotPrefab = treatSlot;


        switch (currentCost.costAmount)
        {
            case 1:
                newSlot = Instantiate(slotPrefab);
                newSlot.transform.position = slotPosition.position;
                newSlot.transform.parent = blackScreen.transform;
                break;

            case 2:
                for(int i = 0; i < 2; i++)
                {
                    if (i == 0) offset = new Vector3(-1.4f,0,0);
                    else offset = new Vector3(1.4f, 0, 0);

                    newSlot = Instantiate(slotPrefab);
                    newSlot.transform.position = slotPosition.position + offset;
                    newSlot.transform.parent = blackScreen.transform;
                }
                
                break;

            case 3:
                for (int i = 0; i < 3; i++)
                {
                    switch(i)
                    {
                        case 0:
                            offset = new Vector3(2.8f, 0, 0);
                            break;

                        case 1:
                            offset = Vector3.zero;
                            break;

                        case 2:
                            offset = new Vector3(-2.8f, 0, 0);
                            break;
                    }


                    newSlot = Instantiate(slotPrefab);
                    newSlot.transform.position = slotPosition.position + offset;
                    newSlot.transform.parent = blackScreen.transform;
                }
                break;
        }
        #endregion

        #region Activate Buttons
        if (currentCost.costAmount == 0)
        {
            bigButton.SetActive(true);
        }
        else
        {
            paymentButtons.SetActive(true);
            CheckCanAfford();
        }
        #endregion

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
                //this is to check if you have payed
                bool canPay = CanPay();  

                #region Ending Screen Check
                if (isEnding && canPay)
                {
                    //reduce fear by 2 for each treat payed
                    //hardcoded but functional
                    for (int i = 0; i < blackScreen.transform.childCount; i++)
                    {
                        if (blackScreen.transform.GetChild(i).childCount > 0)
                        {
                            playerFear.UpdateFear(2);
                        }
                    }

                    //Return hand to original pos
                    handScript.MoveHand(4);
                    if (isEnding)
                    {
                        handScript.UpdateFear();
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    }
                }
                #endregion

                if (canPay)
                {
                    for (int i = 0; i < blackScreen.transform.childCount; i++)
                    {
                        Destroy(blackScreen.transform.GetChild(i).gameObject);
                    }

                    if(!isEnding) CheckConsequence(currentCost.reward, currentCost.rewardAmount);

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
                        blackScreen.transform.GetChild(i).transform.GetComponentInChildren<CardSlotHand>().isPayment = false;
                        blackScreen.transform.GetChild(i).transform.GetChild(0).parent = hand;
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

                if (isEnding)
                {
                    handScript.UpdateFear();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }

            if (textManager.inTrap) textManager.inTrap = false;

        }
    }
    private void DeactivateMenu()
    {
        for (int i = 0; i < blackScreen.transform.childCount; i++)
        {
            Destroy(blackScreen.transform.GetChild(i).gameObject);
        }

        #region Deactivate Stuff
        bigButton.SetActive(false);
        paymentButtons.SetActive(false);
        paymentMenu.SetActive(false);
        if(!hasLost) blackScreen.SetActive(false);
        blackscreen2.SetActive(false);
        fearCounter.gameObject.SetActive(false);
        optionsButton.enabled = true;
        handScript.MoveHand(4);
        #endregion

        if (manager.trapTriggered)
        {
            //If you triggered a card
            //Move hand to original position;
            moveHand = true;
            desiredPos = originalPos;
        }
        else
        {
            //If you trigerred a creature
            if(manager.currentState == GameManager.turnState.Moving)
            {
                //This is in case you trigger a creature with a costume on
                manager.ChangeState(GameManager.turnState.ReplaceCard);        
            }
            else
            {
                manager.ChangeState(GameManager.turnState.Endturn);
            }

            mouseManager.hover2Pos = null;
        }

        mouseManager.hoverAesthetics.SetActive(false);
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
            playerFear.UpdateFear(howMuch);
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

        hand1.SetActive(false);
        hand2.SetActive(true);
    }
    public void ActivateFinalScreen()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 4)
        {
            Time.timeScale = 0;
            manager.GetComponent<SceneManagement>().DisplayMenu(gameWonMenu);
            mouseManager.canClick = false;
        }
        else
        {
            isEnding = true;
            ActivatePayment(endSprite, endCost);
        }

        soundManager.PlaySound("Game Won");
        soundManager.Sources[2].loop = false; 
    }
    private void DisplayFear()
    {
        fearCounter.gameObject.SetActive(true);
        fearText.text = playerFear.hope.ToString();
    }
    public void CheckCanAfford()
    {
        bool canAfford = CanPay();
        TextMeshProUGUI myText = tryPayButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (canAfford)
        {
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, 1);
            tryPayButton.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            tryPayButton.transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            #region Display amount of courage gained
            if (!isEnding) SetFearModTxt(currentCost.rewardAmount);
            else
            {
                int amount = 0;
                for (int i = 0; i < blackScreen.transform.childCount; i++)
                {
                    if (blackScreen.transform.GetChild(i).childCount > 0)
                    {
                        amount += 2;
                    }
                }

                SetFearModTxt(amount);
            }
            #endregion
        }
        else
        {
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, 0.5f);
            tryPayButton.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            tryPayButton.transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            if (!isEnding) SetFearModTxt(currentCost.consequenceAmount);
            else SetFearModTxt(0);
        }
    }
    private bool CanPay()
    {
        bool canPay = true;

        for (int i = 0; i < blackScreen.transform.childCount; i++)
        {
            if (blackScreen.transform.GetChild(i).childCount < 1)
            {
                canPay = false;
            }
        }

        if (blackScreen.transform.childCount == 0)
        {
            canPay = false;
        }

        if (isEnding)
        {
            for (int i = 0; i <= 2; i++)
            {
                if(blackScreen.transform.GetChild(i).childCount > 0)
                {
                    canPay = true;
                }
            }
        }

        return canPay;
    }
    private void SetFearModTxt(int amount)
    {
        //Set text to show if the player loses or gains courage
        string sign = "";
        if(amount == 0)
        {
            fearModTxt.text = "";
            return;
        }

        if (amount < 0) fearModTxt.color = Color.red;
        else
        {
            fearModTxt.color = Color.green;
            sign = "+";
        }

        fearModTxt.text = sign + amount.ToString();

        //If player is spending too much or going to ide, Warning
        if (playerFear.hope + amount <= 0 || playerFear.hope + amount > 10)
        {
            if (playerFear.hope + amount <= 0) fearModTxt.text += "<br> Death";
            if (playerFear.hope + amount > 10) fearModTxt.text += "<br> Too Much!";

            fearModTxt.transform.position = new Vector3(originalPos2.x, originalPos2.y - 24, originalPos2.z);
        }
        else fearModTxt.transform.position = originalPos2;
    }
}
