using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardObject : MonoBehaviour
{
    public Card myCard;
    [SerializeField] private SpriteRenderer rendereador;

    void Start()
    {
        GetComponentInParent<CardSlot>().cardObject = this.gameObject;
        rendereador = GetComponent<SpriteRenderer>();
        if(GetComponent<Card>() != null)
        {
            myCard = GetComponent<Card>();
        }
        rendereador.sprite = myCard.image;
        myCard.replaceCard += ReplaceCard;
    }
    private void ReplaceCard()
    {
        transform.GetComponentInParent<CardSlot>().ReplaceCard();
    }
}
