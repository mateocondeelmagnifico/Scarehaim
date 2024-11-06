using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private GameObject blackScreen;
    private float timer;
    private TMPro.TextMeshProUGUI textBox;
    private Image background;
    private SpriteRenderer screenRenderer;
    private SceneManagement sceneManagement;
    private SoundManager soundManager;

    private void Start()
    {
        background = transform.GetChild(0).GetComponent<Image>();
        textBox = transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        screenRenderer = blackScreen.GetComponent<SpriteRenderer>();
        timer = 4;
        blackScreen.SetActive(true);
        sceneManagement = SceneManagement.Instance;
        soundManager = SoundManager.Instance;
        soundManager.PlaySound("Next Stage");
        soundManager.Sources[2].loop = false;
    }
    private void Update()
    {
        if(timer > 0)
        {
            timer-=Time.deltaTime;

            background.color = new Color(background.color.r, background.color.g, background.color.b, timer/2);
            textBox.color = new Color(1, 1, 1, timer / 2);
            screenRenderer.color = new Color(0, 0, 0, timer / 2);
        }
        else
        {
            soundManager.PlaySound("Music");
            soundManager.Sources[2].loop = true;
            sceneManagement.canPause = true;
            blackScreen.SetActive(false);
            screenRenderer.color = new Color(0, 0, 0, 0);
            Destroy(gameObject);
        }
    }
}
