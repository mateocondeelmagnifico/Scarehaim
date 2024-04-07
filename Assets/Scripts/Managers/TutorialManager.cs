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

    [TextArea, SerializeField] protected string[] tutorialTexts;
    [SerializeField] protected GameObject[] chosenSlots;

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
                        StopGame();
                    }
                    break;

                 case 1:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //go to treat tutorial
                        StopGame();             
                    }
                    break;

                case 3:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //use treat tutorial
                        mouseManager.needsTreat = true;
                        StopGame();
                    }
                    break;

                case 5:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //Go to enemy tutorial
                        StopGame();
                    }
                    break;


                case 7:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //Explain enemy tutorial
                        StopGame();
                    }
                    break;

                case 9:
                    if (manager.currentState == GameManager.turnState.Endturn)
                    {
                        StopGame();
                    }
                    break;

                case 10:
                    if (manager.CheckIsInCheckMovement())
                    {
                        StopGame();
                    }
                    break;
            }
            #endregion
        }
    }

    public void StopGame()
    {
        if (manager.newCardSlot != null) manager.newCardSlot.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 4;

        DisplayTutorial();
    }

    private void DisplayTutorial()
    {

        textBox.gameObject.SetActive(true);
        textManager.TutorialTalk(tutorialTexts[currentTutorial]);
        mouseManager.DeactivateDisplay();
        mouseManager.hoverAesthetics.SetActive(false);
        tutorialPlayed = true;

        if (currentTutorial != 0)
        {
            mouseManager.canClick = false;
            textManager.displayButton = true;
        }
    }

    public virtual void Nextmenu()
    {
        //Called by buttons

        currentTutorial++;
        if (currentTutorial == 11)
        {
            //Destroy tutorial manager
            RemoveTutorial();
            nextTutorialButton.SetActive(false);
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
                break;
        }

        if(textBox.gameObject.activeInHierarchy) textManager.TutorialTalk(tutorialTexts[currentTutorial]);
    }

    public void RemoveTutorial()
    {
        textBox.gameObject.SetActive(false);
        hand.DeterminePosition();
        textManager.StopTalk();
    }
    public virtual bool IsCorrectCard(GameObject mySlot)
    {
        bool istrue = false;
        if(mySlot == chosenSlots[currentTutorial]) istrue = true;

        if ((currentTutorial == 4 && mySlot == chosenSlots[currentTutorial - 1])) istrue = true;

        return istrue;
    }
}
