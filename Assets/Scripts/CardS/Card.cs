using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Card : ScriptableObject
{
    public Sprite image, bigImage;
    [HideInInspector] public CardObject myObject;
    public GameObject myCardObject;

    public virtual void Effect(GameObject card, GameObject cardSlot)
    {

    }
    public virtual void PlayEffect(GameManager manager)
    {

    }
    public virtual void UndoEffect() { }

}
