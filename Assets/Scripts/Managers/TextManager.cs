using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textBox;
    public static TextManager Instance { get; private set; }

    [TextArea]
    [SerializeField] private string[] greetings, basicDialogue, fearOver7, annoyed, nearPlayer;
    private string[] currentTexts;

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

    [HideInInspector] public EnemyStates currentState;

    public float textCooldown;
    private float textDuration;

    public bool fearReached, closeToEnemy;
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
            textCooldown = 14;
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
                }
                else
                {
                    currentState = EnemyStates.NearPlayer;
                }
            }
            else
            {
                currentState = EnemyStates.NearPlayer;
            }
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

        textBox.text = currentTexts[Random.Range(0, currentTexts.Length)];
        displayText = false;
        textCooldown = 10;
    }
}
