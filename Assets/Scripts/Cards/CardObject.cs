using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    public Card myCard;
    [SerializeField] private SpriteRenderer rendereador;

    void Start()
    {
        rendereador.sprite = myCard.image;
        
    }

    private void Update()
    {
        myCard.replaceCard += ReplaceCard();
    }

    private void ReplaceCard()
    {
        transform.GetComponentInParent<CardSlot>().ReplaceCard();
    }
}
