using TMPro;
using UnityEngine;

public class ButtonArranger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myText;
    [SerializeField] private RectTransform leftArrow, rightArrow;
    TMP_TextInfo tInfo;
    private string prevText;

    [SerializeField] private bool specialArrows;

    //reposiciona las flechas y el tamaño del texto para que la UI no se joda con la traducción del texto

    private void Start()
    {
        prevText = myText.text;
        MoveArrows();
    }

    public void MoveArrows()
    {
        tInfo = myText.textInfo;
        if (tInfo.characterCount == 0) return;

        TMP_CharacterInfo first = tInfo.characterInfo[0];
        TMP_CharacterInfo sec = tInfo.characterInfo[tInfo.characterCount - 1];

        leftArrow.localPosition = new Vector2(-(Mathf.Abs(first.bottomLeft.x) + 15), 0);
        rightArrow.localPosition = new Vector2(sec.bottomRight.x + 15, 0);
        Debug.Log(first.bottomLeft.x + ", " + sec.bottomRight.x + " " + name);
    }

    private void Update()
    {
        //Esto detecta cuando se cambia de idioma
        if(prevText != myText.text)
        {
            prevText = myText.text;
           
            MoveArrows();
        }
    }
}
