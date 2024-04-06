using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{

    [SerializeField] private GameObject displayImage;

    [SerializeField] private TMPro.TextMeshProUGUI textBox;

    private GameManager manager;
    private SceneManagement pause;
    private CardManager cardManager;
    private TextManager textManager;
    private Hand hand;

    private bool tutorialPlayed;

    [TextArea, SerializeField] private string[] tutorialTexts;

    public int currentTutorial;

    private void Start()
    {
        hand = Hand.Instance;

        if (hand.tutorialDone)
        {
            Destroy(displayImage.gameObject);
            Destroy(textBox.gameObject);
            Destroy(this.gameObject);
        }
        else
        {
            manager = GameManager.Instance;
            pause = SceneManagement.Instance;
            cardManager = CardManager.Instance;
            textManager = TextManager.Instance;
            pause.canPause = false;
        }
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
                        StopGame();
                    }
                    break;

                 case 1:
                    if (manager.CheckIsInCheckMovement())
                    {
                        StopGame();             
                    }
                    break;

                case 3:
                    if (manager.CheckIsInCheckMovement())
                    {
                        StopGame();
                        Debug.Log(1);
                    }
                    break;

                case 5:
                    if (manager.CheckIsInCheckMovement())
                    {
                        StopGame();
                        Debug.Log(2);
                    }
                    break;


                case 7:
                    if (manager.CheckIsInCheckMovement())
                    {
                        StopGame();
                        Debug.Log(3);
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

        #region Check if destroy

            if (currentTutorial == 10 && Time.timeScale == 1)
            {
                Hand.Instance.tutorialDone = true;
                Destroy(this.gameObject);
            }
        
        
        #endregion
    }

    private void StopGame()
    {
        if (manager.newCardSlot != null) manager.newCardSlot.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 4;

        Time.timeScale = 0;
        pause.canPause = false;

        DisplayTutorial();
    }

    private void DisplayTutorial()
    {
        textBox.gameObject.SetActive(true);
        textManager.TutorialTalk(tutorialTexts[currentTutorial]);
        tutorialPlayed = true;
    }

    public void Nextmenu()
    {
        //Called by buttons

        currentTutorial++;
        textManager.TutorialTalk(tutorialTexts[currentTutorial]);

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

            case 8:
                RemoveTutorial();
                break;

            case 9:
                RemoveTutorial();
                break;

            case 10:
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
        Time.timeScale = 1;
    }
}
