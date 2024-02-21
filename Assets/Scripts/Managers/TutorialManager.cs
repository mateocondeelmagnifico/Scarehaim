using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Image displayImage;

    [SerializeField] private TMPro.TextMeshProUGUI textBox;

    [SerializeField] private GameObject blackBox, blackScreen;

    private GameManager manager;
    [SerializeField] private MouseManager mouseManager;
    private SceneManagement pause;

    private bool gamepaused, condition;
    private bool[] tutorialTriggered;

    public TextAndImage[] tutorialPackages;

    private int activeMenus, nextMenu;
    private float startTimer;

    private void Start()
    {
        manager = GameManager.Instance;
        pause = SceneManagement.Instance;
        startTimer = 2;
        tutorialTriggered = new bool[5];
        mouseManager.canClick = false;
        pause.canPause = false;
    }

    void Update()
    {
        if(startTimer > 0)
        {
            startTimer -= Time.deltaTime;
            if(startTimer <= 0)
            {
                StopGame(0);
                mouseManager.canClick = true;
            }
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (gamepaused)
            {
                Nextmenu();
            }
        }

        #region Tutorial Triggers
        if (manager.currentState == GameManager.turnState.Movecard && !tutorialTriggered[1])
        {
            StopGame(1);
            StopGame(2);
        }

        if(manager.trapTriggered && !tutorialTriggered[3])
        {
            StopGame(3);
        }

        if(manager.moveCardToHand)
        {
            condition = true;
        }

        if(condition && !manager.moveCardToHand && !tutorialTriggered[4])
        {
            StopGame(4);
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

        if (destroy && Time.timeScale == 1) Destroy(this.gameObject);
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
            blackBox.SetActive(true);
            blackScreen.SetActive(true);
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
        displayImage.enabled = true;
        textBox.gameObject.SetActive(true);
        displayImage.sprite = tutorialPackages[whichOne].image;
        textBox.text = tutorialPackages[whichOne].text;
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
            if(manager.newCardSlot != null) manager.newCardSlot.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 20;
            blackBox.SetActive(false);
            blackScreen.SetActive(false);
            displayImage.enabled = false;
            textBox.gameObject.SetActive(false);
            pause.canPause = true;
            gamepaused = false;
            Time.timeScale = 1;
        }
    }
}
