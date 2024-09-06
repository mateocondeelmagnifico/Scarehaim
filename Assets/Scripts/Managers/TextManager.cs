using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textBox;
    public static TextManager Instance { get; private set; }
    public SpriteRenderer enemyRenderer;
    [HideInInspector] public TutorialManager tutorialManager;

    [TextArea]
    [SerializeField] private string[] greetings, basicDialogue, fearOver7, annoyed, nearPlayer;
    private string[] currentTexts;
    private string tempText;

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer box;

    [HideInInspector]
    public enum EnemyStates
    {
        Greeting,
        Idle,
        Annoyed,
        NearPlayer,
        FearOver7,
        HasWon,
        HasLost,
    }

    public EnemyStates currentState;

    [HideInInspector] public float textCooldown;
    public float startTextCD;
    private float textDuration, annoyedDuration;

    public bool fearReached, closeToEnemy, inTutorial, displayButton; //accesed by enemyMovement
    private bool displayText, hasTalked;
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
        textBox.text = "";
        textCooldown = 4;
        currentState = EnemyStates.Greeting;
    }

    void Update()
    {
        if(startTextCD > 0) startTextCD -= Time.deltaTime;
        else if (!hasTalked)
        {
            Talk(currentState);
            hasTalked = true;
        }

        if (inTutorial) return;

        #region Auto Talk
        //Display text on a cooldown
        if (textCooldown > 0)
        {
            textCooldown -= Time.deltaTime;
        }
        else
        {
            textDuration = 7;
            textCooldown = 25;
            displayText = true;
        }

        if(textDuration > 0)
        {
            textDuration -= Time.deltaTime;

            if(displayText)
            {
                Talk(currentState);
            }
        }
        else 
        {
            box.enabled = false;
            textBox.text = "";
        }
        #endregion

        #region Change State
        if (currentState != EnemyStates.Greeting)
        {
            if (!closeToEnemy)
            {
                if (!fearReached)
                {
                    currentState = EnemyStates.Idle;
                    if (annoyedDuration <= 0) enemyRenderer.sprite = sprites[0];
                }
                else
                {
                    currentState = EnemyStates.FearOver7;
                    if (annoyedDuration <= 0) enemyRenderer.sprite = sprites[2];
                }
            }
            else
            {
                if (annoyedDuration <= 0) enemyRenderer.sprite = sprites[2];
                currentState = EnemyStates.NearPlayer;
            }
        }
        #endregion

        #region Timer
        if(annoyedDuration > 0)
        {
            annoyedDuration -= Time.deltaTime;
        }
        #endregion
    }

    public void Talk(EnemyStates state)
    {
        if (inTutorial && currentState != EnemyStates.Greeting) return;

        currentState = state;

        switch (currentState)
        {
            case EnemyStates.Greeting:
                currentTexts = greetings;
                break;

            case EnemyStates.Idle:
                currentTexts = basicDialogue;
                break;

            case EnemyStates.Annoyed:
                currentTexts = annoyed;
                break;

            case EnemyStates.NearPlayer:
                currentTexts = nearPlayer;
                break;

            case EnemyStates.FearOver7:
                currentTexts = fearOver7;
                break;

        }

        box.enabled = true;
        StartCoroutine(ProduceLetters(currentTexts[Random.Range(0, currentTexts.Length)]));
        displayText = false;
        textCooldown = 30;
        textDuration = 7;
    }

    public void TutorialTalk(string myText)
    {
        inTutorial = true;
        box.enabled = true;
        displayText = false;
        
        StartCoroutine(ProduceLetters(myText));
    }

    public void StopTalk()
    {
        box.enabled = false;
        textBox.text = "";
    }

    public void SwapSprite()
    {
        enemyRenderer.sprite = sprites[1];
        annoyedDuration = 2;
    }

    private IEnumerator ProduceLetters(string whatToSay)
    {
        for(int i = 0; i < whatToSay.Length + 1; i++)
        {
            tempText = whatToSay.Substring(0,i);
            textBox.text = tempText;    
            yield return new WaitForSeconds(0.03f);
        }

        if (inTutorial && displayButton)
        {
            //Activates button in tutorial
            tutorialManager.nextTutorialButton.SetActive(true);
            displayButton = false;
        }

        //Empieza la partida después del diálogo
        if (currentState == EnemyStates.Greeting)
        {
            CardManager.Instance.canDealCards = true;
            currentState = EnemyStates.Idle;
        }
    }
}
