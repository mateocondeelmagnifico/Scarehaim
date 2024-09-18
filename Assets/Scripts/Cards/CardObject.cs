using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardObject : MonoBehaviour
{
    public Card myCard;
    protected GameManager gameManager;
    [HideInInspector] public Transform player;

    private SpriteRenderer rendereador;
    [HideInInspector] public Transform discardLocation;

    void Start()
    {
        myCard.myObject = this;
        myCard.myCardObject = this.gameObject;

        if(GetComponent<SpriteRenderer>() != null )
        {
            rendereador = GetComponent<SpriteRenderer>();
            rendereador.sprite = myCard.image;
        }

        gameManager = GameManager.Instance;
    }

    public void DiscardCard()
    {
        transform.GetComponentInParent<CardSlot>().ReplaceCard();
        this.enabled = false;
    }

    public void MoveToHand(GameObject cardSlot)
    {
        #region Make new cardslot and change variables

        transform.parent.GetComponent<CardSlot>().cardObject = null;
        transform.parent.GetComponent<CardSlot>().unavailable = 3;

        GameObject newSlot = GameObject.Instantiate(cardSlot);

        newSlot.transform.position = transform.position;
        newSlot.GetComponent<CardSlotHand>().cardObject = this.gameObject;
        newSlot.GetComponent<CardSlotHand>().enabled = false;
        CardManager.Instance.CardDiscarded(transform.parent.GetComponent<CardSlot>());
        transform.parent = newSlot.transform;

        gameManager.newCardSlot = newSlot;
        gameManager.moveCardToHand = true;

        #endregion

        GameManager.Instance.ChangeState(GameManager.turnState.Movecard);
    }

    public void PlayCard()
    {
        myCard.PlayEffect(gameManager);
        Hand.Instance.PutCardInLimbo(transform.parent.gameObject);
    }

    public void DoEffect(GameObject slot)
    {
        myCard.myCardObject = this.gameObject;
        myCard.Effect(this.gameObject, slot);
    }
}
