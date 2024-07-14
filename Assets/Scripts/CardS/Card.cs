using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Card : ScriptableObject
{
    public Sprite image, bigImage;
    [HideInInspector] public CardObject myObject;

    public virtual void Effect(GameObject card, GameObject cardSlot)
    {

    }

    public virtual void PlayEffect(GameManager manager)
    {

    }
    public virtual void UndoEffect() { }
    public virtual void MoveToHand(GameObject card, GameObject cardSlot) 
    {
       
    }

}
