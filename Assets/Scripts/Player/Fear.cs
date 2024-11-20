using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fear : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    [SerializeField] private Animator fearAnimator;
    private TextManager textManager;
    public GameObject gameOverMenu;
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

        #region Make text red and shake
        /*
        if (hope <= 4)
        {
            text.color = new Color(1, (float)hope / 5, (float)hope / 5);
            if (hope > 2) fearAnimator.SetFloat("Intensity", 1);
            else fearAnimator.SetFloat("Intensity", 2);
        }
        else
        {
            text.color = new Color(1, 1, 1);
            fearAnimator.SetFloat("Intensity", 0);
        }
        */
        #endregion

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
            CardEffectManager.Instance.hasLost = true;
            SceneManagement.Instance.DisplayMenu(gameOverMenu);
            SceneManagement.Instance.canPause = false;

            SoundManager.Instance.PlaySound("Game Over");
            SoundManager.Instance.Sources[2].loop = false;
        }

        text.text = hope.ToString();
    }
}
