using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card 
{
    public string name;

    public Sprite image;

    public virtual void Effect(GameObject player)
    {

    }
}
