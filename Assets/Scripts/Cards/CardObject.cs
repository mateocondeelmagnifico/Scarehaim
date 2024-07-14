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

    //Constructor
    public CardObject(Card cardScriptable, Transform discardSpot)
    {
        myCard = cardScriptable;
        discardLocation = discardSpot;
    }

    void Start()
    {
        GetComponentInParent<CardSlot>().cardObject = this.gameObject;
        myCard.myObject = this;

        if(GetComponent<SpriteRenderer>() != null )
        {
            rendereador = GetComponent<SpriteRenderer>();
            rendereador.sprite = myCard.image;
        }
        
        myCard.discardCard += DiscardCard;

        gameManager = GameManager.Instance;
    }

    private void DiscardCard()
    {
        transform.GetComponentInParent<CardSlot>().ReplaceCard();
    }

    public void MoveToHand(GameObject cardSlot)
    {
        #region Make new cardslot and change variables
        transform.parent.GetComponent<CardSlot>().cardObject = null;

        GameObject newSlot = GameObject.Instantiate(cardSlot);

        newSlot.transform.position = transform.position;
        newSlot.GetComponent<CardSlotHand>().cardObject = gameObject;
        newSlot.GetComponent<CardSlotHand>().enabled = false;
        CardManager.Instance.CardDiscarded(transform.parent.GetComponent<CardSlot>());
        transform.parent = newSlot.transform;

        gameManager.newCardSlot = newSlot;
        gameManager.moveCardToHand = true;

        #endregion

        GameManager.Instance.ChangeState(GameManager.turnState.Movecard);
    }

    public void GoToGraveyard()
    {

    }

    public void PlayCard()
    {
        myCard.PlayEffect(gameManager);
        Hand.Instance.PutCardInLimbo(transform.parent.gameObject);
    }

}
