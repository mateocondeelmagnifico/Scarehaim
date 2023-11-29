using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cost 
{
    [TextArea]public string explanation;

    public string mainCostName;
    public int costAmount;

    public string consequenceName;
    public int consequenceAmount;
}
