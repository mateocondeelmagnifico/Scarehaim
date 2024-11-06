using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fear : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    private TextManager textManager;
    public GameObject gameOverMenu;
    [SerializeField] private GameObject blackscreen;  
    public int hope;
    private bool fearReached;


    private void Start()
    {
        textManager = TextManager.Instance;
        hope = Hand.Instance.hope;
        Hand.Instance.RefreshCards();
        text.text = hope.ToString();
    }

    public void UpdateFear(int howMuch)
    {
        if (hope <= 0) return;

        hope += howMuch;

        if (hope > 10)
        {
            hope = 10;
        }

        if (hope <= 3)
        {
            if (!fearReached)
            {
                textManager.Talk(TextManager.EnemyStates.FearOver7);
                textManager.fearReached = true;
                fearReached = true;
            }
            textManager.currentState = TextManager.EnemyStates.FearOver7;
        }
        else
        {
            fearReached = false;
        }

        if (hope <= 0)
        {
            hope = 0;
            Time.timeScale = 0;
            gameOverMenu.SetActive(true);
            SceneManagement.Instance.currentMenu = gameOverMenu;
            SceneManagement.Instance.canPause = false;
            blackscreen.SetActive(true);

            SoundManager.Instance.PlaySound("Game Over");
            SoundManager.Instance.Sources[2].loop = false;
        }

        text.text = hope.ToString();
    }
}
