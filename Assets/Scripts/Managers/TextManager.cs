using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    private int lastArray, lastText;

    public bool fearReached, closeToEnemy, inTutorial, displayButton, inTrap; //accesed by enemyMovement
    private bool displayText, hasTalked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //textBox.text = "";
        textCooldown = 4;
        currentState = EnemyStates.Greeting;
    }

    void Update()
    {

        if (startTextCD > 0) startTextCD -= Time.deltaTime;
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

        if (textDuration > 0)
        {
            textDuration -= Time.deltaTime;

            if (displayText)
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
            if (inTrap)
            {
                enemyRenderer.sprite = sprites[2];
            }
            else if (!closeToEnemy)
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
        if (annoyedDuration > 0)
        {
            annoyedDuration -= Time.deltaTime;
        }
        #endregion
    }

    public void Talk(EnemyStates state)
    {
        if (inTutorial && currentState != EnemyStates.Greeting) return;

        currentState = state;
        tempText = "";
        StopAllCoroutines();

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


        int randomNum = UnityEngine.Random.Range(0, currentTexts.Length);

        if (currentTexts.GetHashCode() == lastArray && randomNum == lastText)
        {
            if (randomNum >= currentTexts.Length - 1) randomNum = 0;
            else randomNum++;
        }

        box.enabled = true;

        //ProcessText(currentTexts[randomNum]);
        StartCoroutine(ProduceLetters(currentTexts[randomNum]));

        displayText = false;
        textCooldown = 30;
        textDuration = 7;

        lastArray = currentTexts.GetHashCode();
        lastText = randomNum;
    }

    public void TutorialTalk(string myText)
    {
        inTutorial = true;
        box.enabled = true;
        displayText = false;
        tempText = "";
        StopAllCoroutines();

        StartCoroutine(ProduceLetters(myText));
        //ProcessText(myText);
    }

    private void ProcessText(LocalizedString text)
    {
        //Este void hace que en la build web cargue el texto traducido
        //Cuando el texto ha cargado entonces empieza a crear las letras

        var operation = text.GetLocalizedStringAsync();

        //Get locale and neccesary values
        //Este sistema no es óptimo, en un futuro habría que cambiarlo
        var table = LocalizationSettings.StringDatabase.GetTableAsync(text.TableReference);

        //UpdateString(operation);
        //StartCoroutine(ProduceLetters(Localize(table, text.)));
        StartCoroutine(ProduceLetters(Localize(table.Result, "Tut 1")));
    }

    public static string Localize(StringTable table, string key)
    {
        if (table == null) { Debug.Log("noTable");  return key; }

        Debug.Log(key);
        var entry = table.GetEntry(key);
        if (entry == null) { return key; }

        Debug.Log(entry.GetLocalizedString());

        return entry.GetLocalizedString();
    }

    void UpdateString(AsyncOperationHandle<string> value)
    {
        if (!value.IsDone)
        {
            // Defer the callback until the operation is finished
            //waits until it translates
            value.Completed += UpdateString;
            value.WaitForCompletion();
        }
        else
        {
            string wantedText = value.Result;

            StartCoroutine(ProduceLetters(wantedText));
        }
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
            tutorialManager.buttonAnimator.SetBool("Jump", true);
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
