using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fear : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public int fear;

    private void Update()
    {
        text.text = fear.ToString();
    }
}
