using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fear : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    private TextManager textManager;
    public GameObject gameOverMenu;
    public int fear;
    private bool fearReached;

    private void Start()
    {
        textManager = TextManager.Instance;
        fear = Hand.Instance.fear;
    }
    private void Update()
    {
        text.text = fear.ToString();

        if(fear < 0 )
        {
            fear = 0;
        }

        if(fear >= 7)
        {
            if(!fearReached)
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

        if(fear >= 10 )
        {
            fear = 10;
            Time.timeScale = 0;
            gameOverMenu.SetActive(true);
        }
    }
}
