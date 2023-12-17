using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cost 
{
    [Header("Deja vacíos los campos que no se usen")]
    [Header("Reward amount tiene que estar en negativo")]

    [TextArea]public string explanation;

    public string CostName;
    public int costAmount;

    public string reward;
    public int rewardAmount;

    public string consequenceName, secondConsequenceName;
    public int consequenceAmount, secondConsequenceAmount;
}
