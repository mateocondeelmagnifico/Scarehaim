using UnityEngine;
using UnityEngine.UI;

public class MyButton : MonoBehaviour
{
    [SerializeField] private Image arrowLeft, arrowRight;
    
    public void DisplayArrows(bool isTrue)
    {
        arrowLeft.enabled = isTrue;
        arrowRight.enabled = isTrue;
    }
}
