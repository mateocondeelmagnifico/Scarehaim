using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textBox;
    public static TextManager Instance { get; private set; }
    public SpriteRenderer enemyRenderer;

    [TextArea]
    [SerializeField] private string[] greetings, basicDialogue, fearOver7, annoyed, nearPlayer;
    private string[] currentTexts;

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image box;

    [HideInInspector]
    public enum EnemyStates
    {
        Greeting,
        Idle,
        Annoyed,
        NearPlayer,
        FearOver7,
        HasWon,
        HasLost
    }

    public EnemyStates currentState;

    [HideInInspector] public float textCooldown;
    private float textDuration, annoyedDuration;

    public bool fearReached, closeToEnemy; //accesed by enemyMovement
    private bool displayText;
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
        currentState = state;

        switch (currentState)
        {
            case EnemyStates.Greeting:
                currentTexts = greetings;
                currentState = EnemyStates.Idle;
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
        textBox.text = currentTexts[Random.Range(0, currentTexts.Length)];
        displayText = false;
        textCooldown = 25;
        textDuration = 7;
    }

    public void SwapSprite()
    {
        enemyRenderer.sprite = sprites[1];
        annoyedDuration = 2;
    }
}
