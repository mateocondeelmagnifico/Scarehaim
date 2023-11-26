using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textBox;

    [TextArea]
    [SerializeField] private string[] phrases;

    private float textDuration, textCooldown;

    private bool displayText;
    void Start()
    {
        textBox.text = "";
        textCooldown = 10;
    }

    void Update()
    {
        //Display text on a cooldown
        if(textCooldown > 0)
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
                textBox.text = phrases[Random.Range(0, phrases.Length)];
                displayText = false;
            }
        }
        else
        {
            textBox.text = "";
        }
    }
}
