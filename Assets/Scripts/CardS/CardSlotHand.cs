using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotHand: CardSlot
{
    private Vector3 startingPos;
    public Vector3 direction;

    public bool goHome, followMouse;
    private bool isPayment;

    private CardEffectManager effectManager;
    private Transform blackScreen;

    private int chosenSlot;

    private void OnEnable()
    {
        isInHand = true;
        startingPos = transform.position;
        gameManager = GameManager.Instance;
        effectManager = CardEffectManager.Instance;
        blackScreen = effectManager.blackScreen.transform;
    }

    private void Update()
    {
        //Both Relocate and Move are called by the gameManager
        //Follow Mouse is changed by the Mouse manager

        if (followMouse && !isPayment)
        {
            direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(direction.x, direction.y, transform.position.z);
        }
        
        if(transform.position != startingPos && !isPayment && !followMouse)
        {
            Relocate();
        }

        if (goHome && !isPayment)
        {
            if (transform.position != startingPos)
            {
                Move(startingPos);
            }
            else
            {
                goHome = false;
            }
        }

        if(isPayment)
        {
            if (transform.position != direction)
            {
                Move(direction);
            }
        }
    }

    public void Relocate()
    {
        if(Vector3.Distance(transform.position, startingPos) <= 2)
        {
            goHome = true;
        }
        else
        {
            TryToPlayCard();
        }
    }
    
    private void TryToPlayCard()
    {
        bool cardPlayed = false;
            if (gameManager.currentState == GameManager.turnState.CheckMovement)
            {
                //Aqui es donde se aplica el efecto del disfraz
                goHome = true;
                cardPlayed = true;
            }
            
            if(gameManager.currentState == GameManager.turnState.CheckCardEffect && effectManager.effectActive)
            {
                float[] distances = new float[3];

                #region Check Which Slot is Closer
                //This loop gets the position of the payment spots
                for (int i = 0; i < 3; i++)
                {
                    if (blackScreen.GetChild(i).childCount == 0)
                    {
                        distances[i] = Vector3.Distance(blackScreen.GetChild(i).transform.position, transform.position);
                    }
                    else
                    {
                        //esto es para que si el slot ya tiene dentro una carta no se tenga en cuenta
                        distances[i] = 50000;
                    }
                }


                if (Mathf.Min(distances[0], distances[1], distances[2]) == distances[0])
                {
                    chosenSlot = 0;
                }
                if (Mathf.Min(distances[0], distances[1], distances[2]) == distances[1])
                {
                    chosenSlot = 1;
                }
                if (Mathf.Min(distances[0], distances[1], distances[2]) == distances[2])
                {
                    chosenSlot = 2;
                }
            #endregion

                direction = blackScreen.GetChild(chosenSlot).position;
                isPayment = true;
                transform.parent = effectManager.blackScreen.transform.GetChild(chosenSlot);
                cardPlayed = true;
            }
      
            if(!cardPlayed)
            goHome = true;
    }

    private void Move(Vector3 desiredPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, desiredPos, 16 * Time.deltaTime);
    }
}
