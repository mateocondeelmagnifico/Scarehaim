using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{

    [SerializeField] private GameObject displayImage, nextTutorialButton;

    [SerializeField] private TMPro.TextMeshProUGUI textBox;

    private GameManager manager;
    private SceneManagement pause;
    private CardManager cardManager;
    private TextManager textManager;
    [SerializeField] private MouseManager mouseManager;
    private Hand hand;

    private bool tutorialPlayed;

    [TextArea, SerializeField] private string[] tutorialTexts;
    [SerializeField] private GameObject[] chosenSlots;

    public int currentTutorial;

    private void Start()
    {
        hand = Hand.Instance;
        manager = GameManager.Instance;
        pause = SceneManagement.Instance;
        cardManager = CardManager.Instance;
        textManager = TextManager.Instance;
        mouseManager.tutorialManager = this;
        pause.canPause = false;
        
    }

    void Update()
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

    private void StopGame()
    {
        if (manager.newCardSlot != null) manager.newCardSlot.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 4;

        pause.canPause = false;

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
            nextTutorialButton.SetActive(true);
        }
    }

    public void Nextmenu()
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
        textManager.TutorialTalk(tutorialTexts[currentTutorial]);

        if(currentTutorial != 8 && currentTutorial != 9 && currentTutorial != 10) nextTutorialButton.SetActive(false);
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
    }

    private void RemoveTutorial()
    {
        textBox.gameObject.SetActive(false);
        pause.canPause = true;
        hand.DeterminePosition();
        textManager.StopTalk();
    }

    public bool IsCorrectCard(GameObject mySlot)
    {
        bool istrue = false;
        if(mySlot == chosenSlots[currentTutorial]) istrue = true;

        return istrue;
    }
}
