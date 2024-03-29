using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardSlotHand: CardSlot
{
    public Vector3 direction, startingPos;

    public bool goHome, followMouse, isPayment;

    public CardEffectManager effectManager;
    private Transform blackScreen;

    private int chosenSlot;

    private void OnEnable()
    {
        isInHand = true;
        startingPos = transform.position;
        gameManager = GameManager.Instance;
        if(effectManager == null) effectManager = CardEffectManager.Instance;
        blackScreen = effectManager.blackScreen.transform;
        if(transform.childCount > 0) cardObject = transform.GetChild(0).gameObject;
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
            else
            {
                transform.rotation = Quaternion.identity;
            }
        }

        #region Hover Code
        if (otherTimer > 0)
        {
            otherTimer -= Time.deltaTime;
        }
        else
        {
            isHovered = false;
            soundPlayed = false;
        }

        if (isHovered)
        {
            hoverTimer += Time.deltaTime;
            if (!soundPlayed && hoverTimer > 0.8f)
            {
                SoundManager.Instance.PlaySound("Card Hovered");
                soundPlayed = true;
            }
        }
        else
        {
            hoverTimer = 0;
        }
        #endregion
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
            if (gameManager.currentState == GameManager.turnState.CheckMovement && !GameManager.Instance.powerUpOn)
            {
                //Aqui es donde se aplica el efecto del disfraz y del treat
                goHome = true;
                cardObject.GetComponent<Card>().PlayEffect();
                cardPlayed = true;
            }
            
            if(gameManager.currentState == GameManager.turnState.CheckCardEffect && effectManager.effectActive)
            {
                float[] distances = new float[3];
                if (blackScreen.childCount == 2) distances[2] = 50000;

                #region Check Which Slot is Closer
                //This loop gets the position of the payment spots
                for (int i = 0; i < blackScreen.childCount; i++)
                {
                    if (blackScreen.GetChild(i).childCount < 1 && blackScreen.GetChild(i).tag == cardObject.tag)
                    {
                        distances[i] = Vector3.Distance(blackScreen.GetChild(i).transform.position, transform.position);
                    }
                    else
                    {
                        //esto es para que si el slot ya tiene dentro una carta no se tenga en cuenta
                        distances[i] = 50000;
                    }
                }

                if(blackScreen.childCount == 2)
                {
                    distances[2] = 50000;
                }
                if (blackScreen.childCount == 1)
                {
                    distances[2] = 50000;
                    distances[1] = 50000;
                }

                if (Mathf.Min(distances[0], distances[1], distances[2]) < 45000)
                {
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

                    if (blackScreen.childCount > 0)
                    {
                        direction = blackScreen.GetChild(chosenSlot).position;
                        isPayment = true;
                        transform.parent = effectManager.blackScreen.transform.GetChild(chosenSlot);
                        cardPlayed = true;
                    }
                }
                else
                {
                    goHome = true;
                }
                    #endregion
            }
      
        if(!cardPlayed)
        {
            goHome = true;
        }
        else
        {
            SoundManager.Instance.PlaySound("Card Played");
        }
    }

    private void Move(Vector3 desiredPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, desiredPos, 16 * Time.deltaTime);
    }
}
