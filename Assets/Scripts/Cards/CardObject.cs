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
}
