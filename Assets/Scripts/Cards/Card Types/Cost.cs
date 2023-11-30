using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cost 
{
    [TextArea]public string explanation;

    public string CostName;
    public int costAmount;

    public string consequenceName, secondConsequenceName;
    public int consequenceAmount, secondConsequenceAmount;
}
