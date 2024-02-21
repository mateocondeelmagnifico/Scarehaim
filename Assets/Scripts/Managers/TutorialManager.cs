using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Image displayImage;

    [SerializeField] private TMPro.TextMeshProUGUI textBox;

    [SerializeField] private GameObject blackBox;

    private bool gamepaused;

    public string[] texts;

    public Sprite[] images;

    private int currentTutorial;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (gamepaused)
            {
                Nextmenu();
            }
        }
    }

    private void StopGame()
    {
        Time.timeScale = 0;
        gamepaused = true;
        blackBox.SetActive(true);
    }

    private void DisplayTutorial()
    {
        displayImage.enabled = false;
        textBox.enabled = true;
        displayImage.sprite = images[currentTutorial];
        textBox.text = texts[currentTutorial];

        currentTutorial++;
    }

    private void Nextmenu()
    {
        displayImage.enabled = false;
        textBox.enabled = true;
    }
}
