using TMPro;
using UnityEngine;

public class ButtonArranger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myText;
    [SerializeField] private RectTransform leftArrow, rightArrow;
    TMP_TextInfo tInfo;

    [Header("Marca esto para los que tengan las flechas raras")]
    [SerializeField] private bool specialArrows;

    private float timer, offset;
    private bool call;

    //reposiciona las flechas y el tamaño del texto para que la UI no se joda con la traducción del texto

    private void Start()
    {
        if (specialArrows) offset = 24;
        else offset = 15;
        MoveArrows();
    }

    private void MoveArrows()
    {
        tInfo = myText.textInfo;
        if (tInfo.characterCount == 0) return;

        TMP_CharacterInfo first = tInfo.characterInfo[0];
        TMP_CharacterInfo sec = tInfo.characterInfo[tInfo.characterCount - 1];

        leftArrow.localPosition = new Vector2(-(Mathf.Abs(first.bottomLeft.x) + offset), 0);
        rightArrow.localPosition = new Vector2(sec.bottomRight.x + offset, 0);
    }

    public void CallMoveArrows()
    {
        timer = 0.1f;
        call = true;
    }

    private void Update()
    {
        if(timer > 0) timer -= Time.deltaTime;
        else if(call)
        {
            MoveArrows();
            call = false;
        }
    }
}
