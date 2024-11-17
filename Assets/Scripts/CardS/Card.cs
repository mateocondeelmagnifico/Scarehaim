using UnityEngine;


public class Card : ScriptableObject
{
    public Sprite image, bigImage;
    [HideInInspector] public CardObject myObject;
    public GameObject myCardObject;
    public string description;

    public virtual void Effect(GameObject card, GameObject cardSlot)
    {

    }
    public virtual void PlayEffect(GameManager manager)
    {

    }
    public virtual void UndoEffect() { }

}
