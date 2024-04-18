using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{

    [SerializeField] protected GameObject displayImage;
    public GameObject nextTutorialButton;

    [SerializeField] protected TMPro.TextMeshProUGUI textBox;

    protected GameManager manager;
    protected CardManager cardManager;
    protected TextManager textManager;
    [SerializeField] protected MouseManager mouseManager;
    protected Hand hand;

    protected bool tutorialPlayed;
    public bool radarDone; //Used by inherited member
    private bool showExit;

    [TextArea, SerializeField] protected string[] tutorialTexts;
    [SerializeField] protected GameObject[] chosenSlots;
    [SerializeField] protected Sprite[] blackScreens, exitspots1, exitspots2, exitspots3;
    [SerializeField] protected SpriteRenderer screenImage, cenefa;

    public int currentTutorial;

    public void Start()
    {
        hand = Hand.Instance;
        manager = GameManager.Instance;
        cardManager = CardManager.Instance;
        textManager = TextManager.Instance;
        mouseManager.tutorialManager = this;
        textManager.tutorialManager = this;
    }

    public virtual void Update()
    {
        if(manager.currentState == GameManager.turnState.Endturn) tutorialPlayed = false;

        if (!tutorialPlayed)
        {
            #region Tutorial Triggers

            switch(currentTutorial)
            {
                case 0:
                    if (cardManager.cardsDealt)
                    {
                        //Move down tutorial
                        DisplayTutorial();
                    }
                    break;

                 case 1:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //go to treat tutorial
                        DisplayTutorial();             
                    }
                    break;

                case 3:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //use treat tutorial
                        mouseManager.needsTreat = true;
                        DisplayTutorial();
                    }
                    break;

                case 5:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //Go to enemy tutorial
                        DisplayTutorial();
                    }
                    break;


                case 7:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //Explain enemy tutorial
                        DisplayTutorial();
                    }
                    break;

                case 9:
                    if (manager.currentState == GameManager.turnState.Endturn)
                    {
                        DisplayTutorial();
                    }
                    break;

                case 10:
                    if (manager.CheckIsInCheckMovement())
                    {
                        DisplayTutorial();
                    }
                    break;

                case 11:
                    if (manager.CheckIsInCheckMovement() && cardManager.exitCardDealt)
                    {
                        DisplayTutorial();
                    }
                    break;
            }
            #endregion
        }

        if (currentTutorial == 4)
        {
            if (!manager.powerUpOn) screenImage.sprite = blackScreens[currentTutorial - 1];
            else screenImage.sprite = blackScreens[currentTutorial];
        }

        if (showExit)
        {
            screenImage.enabled = true;
        }
    }

    protected void DisplayTutorial()
    {
        textBox.gameObject.SetActive(true);
        textManager.TutorialTalk(tutorialTexts[currentTutorial]);
        mouseManager.DeactivateDisplay();
        mouseManager.hoverAesthetics.SetActive(false);
        cenefa.sortingOrder = 8000;
        tutorialPlayed = true;

        if (currentTutorial != 0)
        {
            mouseManager.canClick = false;
            textManager.displayButton = true;
        }

        DisplayNextBlackScreen();
    }

    public virtual void Nextmenu()
    {
        //Called by buttons

        currentTutorial++;

        DisplayNextBlackScreen();

        if (currentTutorial == 12)
        {
            //Destroy tutorial manager
            showExit = false;
            RemoveTutorial();
            nextTutorialButton.SetActive(false);
            mouseManager.canClick = true;
            Destroy(this.gameObject);
            return;
        }

        nextTutorialButton.SetActive(false);
        if (currentTutorial != 8 && currentTutorial != 9 && currentTutorial != 10) mouseManager.canClick = false;
        else textManager.displayButton = true;

        mouseManager.canClick = true;

        switch (currentTutorial)
        {
            case 1:
                RemoveTutorial();
                break;

            case 3:
                RemoveTutorial();
                break;

            case 5:
                RemoveTutorial();
                break;

            case 7:
                RemoveTutorial();
                break;

            case 11:
                RemoveTutorial();
                mouseManager.tutorialManager = null;
                break;
        }

        if(textBox.gameObject.activeInHierarchy) textManager.TutorialTalk(tutorialTexts[currentTutorial]);
    }

    public void RemoveTutorial()
    {
        screenImage.enabled = false;
        textBox.gameObject.SetActive(false);
        hand.DeterminePosition();
        textManager.StopTalk();
        cenefa.sortingOrder = 4;
    }
    public virtual bool IsCorrectCard(GameObject mySlot)
    {
        bool istrue = false;
        if(mySlot == chosenSlots[currentTutorial]) istrue = true;

        if ((currentTutorial == 4 && mySlot == chosenSlots[currentTutorial - 1])) istrue = true;

        return istrue;
    }

    public void DisplayNextBlackScreen()
    {
        if (blackScreens[currentTutorial] != null)
        {
            screenImage.enabled = true;

            screenImage.sprite = blackScreens[currentTutorial];
        }
        else screenImage.enabled = false;
    }

    public void DisplayExitTutorial(Vector2 location)
    {
        switch(location.x)
        {
            case 1:
                screenImage.sprite = exitspots1[(int)location.y -1];
                break;

            case 2:
                screenImage.sprite = exitspots2[(int)location.y -1];
                break;

            case 3:
                screenImage.sprite = exitspots3[(int)location.y - 1];
                break;
        }

        showExit = true;
    }
}
