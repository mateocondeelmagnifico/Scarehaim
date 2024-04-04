using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{

    [SerializeField] private GameObject displayImage;

    [SerializeField] private TMPro.TextMeshProUGUI textBox;

    //[SerializeField] private GameObject blackBox, blackScreen;

    [SerializeField] private Transform[] screenPositions;

    private GameManager manager;
    private SceneManagement pause;
    private CardEffectManager effectManager;
    private CardManager cardManager;
    private TextManager textManager;
    private Hand hand;

    private bool gamepaused, condition, wasActive;
    private bool[] tutorialTriggered;

    public TextAndImage[] tutorialPackages;

    private int activeMenus, nextMenu;
    [SerializeField] private float[] sizes;

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
            effectManager = CardEffectManager.Instance;
            cardManager = CardManager.Instance;
            textManager = TextManager.Instance;
            tutorialTriggered = new bool[7];
            pause.canPause = false;
        }
    }

    void Update()
    {
       

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (gamepaused)
            {
                //Nextmenu();
            }
        }

        #region Tutorial Triggers
        if (!tutorialTriggered[0])
            if (cardManager.cardsDealt)
            {
                StopGame(0);
            }

        if (!tutorialTriggered[1])
        if (manager.currentState == GameManager.turnState.Movecard)
        {
            StopGame(1);
            StopGame(2);
        }

        if(!tutorialTriggered[3])
        if(manager.trapTriggered && effectManager.paymentMenu.activeInHierarchy)
        {
            StopGame(3);
        }

        if(manager.moveCardToHand)
        {
            condition = true;
        }

        if(!tutorialTriggered[4])
        if(condition && !manager.moveCardToHand)
        {
            StopGame(4);
        }

        if (!tutorialTriggered[5])
        {
            if (manager.turnCount == 4)
            {
                StopGame(5);
            }
        }

        if (!tutorialTriggered[6])
        {
            if (effectManager.paymentMenu.activeInHierarchy)
            {
                StopGame(6);
            }
        }
        #endregion

        #region Check if destroy
        bool destroy = true;
        for (int i = 0; i < tutorialTriggered.Length; i++) 
        { 
            if(!tutorialTriggered[i])
            {
                destroy = false;
            }
        }

        if (destroy && Time.timeScale == 1)
        {
            Hand.Instance.tutorialDone = true;
            Destroy(this.gameObject);
        }
        #endregion
    }

    private void StopGame(int whichOne)
    {
        tutorialTriggered[whichOne] = true;
        if (manager.newCardSlot != null) manager.newCardSlot.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 4;

        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            gamepaused = true;
            /*
            if(blackScreen.activeInHierarchy)
            {
                wasActive = true;
            }
            else
            {
                blackScreen.SetActive(true);
                wasActive = false;
            }
            */
            activeMenus++;
            pause.canPause = false;

            DisplayTutorial(whichOne);
        }
        else
        {
            //Mirar si ya estas con un tutorial
            nextMenu = whichOne;
            activeMenus++;
        }
    }

    private void DisplayTutorial(int whichOne)
    {
        displayImage.SetActive(true);
        displayImage.transform.position = screenPositions[whichOne].position;
        displayImage.transform.localScale = displayImage.transform.localScale * sizes[whichOne];
        textBox.gameObject.SetActive(true);
        //displayImage.sprite = tutorialPackages[whichOne].image;
        textManager.TutorialTalk(tutorialPackages[whichOne].text);
    }

    private void Nextmenu()
    {
        if(activeMenus > 0)
        {
            activeMenus--;
        }

        if (activeMenus > 0)
        {
            DisplayTutorial(nextMenu);
        }
        else
        {
            /*
            blackBox.SetActive(false);
            if(!wasActive)
            {
                blackScreen.SetActive(false);
            }
            */
            displayImage.SetActive(false);
            textBox.gameObject.SetActive(false);
            pause.canPause = true;
            gamepaused = false;
            hand.DeterminePosition();
            textManager.StopTalk();
            Time.timeScale = 1;
        }
    }
}
