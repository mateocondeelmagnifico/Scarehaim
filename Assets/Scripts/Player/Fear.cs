using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fear : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public GameObject gameOverMenu;
    public int fear;

    private void Update()
    {
        text.text = fear.ToString();

        if(fear < 0 )
        {
            fear = 0;
        }

        if(fear >= 10 )
        {
            fear = 10;
            Time.timeScale = 0;
            gameOverMenu.SetActive(true);
        }
    }
}
