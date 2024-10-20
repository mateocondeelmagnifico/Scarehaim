using UnityEngine;

public class CardSlotHand: CardSlot
{
    public Vector3 direction, startingPos;

    public bool  isPayment, hasArrived, followMouse, inHand;
    [SerializeField] private bool goHome;

    public CardEffectManager effectManager;
    private Transform blackScreen;
    public Transform oldParent;
    private Hand hand;

    private int chosenSlot;
    private float accelerator = 0.5f;

    private void OnEnable()
    {
        isInHand = true;
        startingPos = transform.position;
        gameManager = GameManager.Instance;
        hand = Hand.Instance;

        if(effectManager == null) effectManager = CardEffectManager.Instance;
        blackScreen = effectManager.blackScreen.transform;

        //Give tag to self
        if (transform.childCount > 0)
        {
            cardObject = transform.GetChild(0).gameObject;
            switch(cardObject.GetComponent<CardObject>().myCard)
            {
                case Treat:
                    cardObject.tag = "Treat";
                    break;

                case Disguise:
                    cardObject.tag = "Costume";
                    break;

            }
        }
    }

    private void Update()
    {
        //Both Relocate and Move are called by the gameManager
        //Follow Mouse is changed by the Mouse manager

        #region Follow mouse
        if (followMouse && !isPayment)
        {
            direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(direction.x, direction.y, -5), 4 * Time.deltaTime * (Vector3.Distance(transform.position, direction) * 2));
        }
        #endregion

        #region Go home
        if (transform.position != startingPos && !isPayment && !followMouse && !goHome)
        {
            Relocate();
        }

        if (goHome && !isPayment && transform.position != startingPos)
        {
            if (transform.parent == null)
            {
                transform.parent = oldParent;
            }

            Move(startingPos);
            
            if(transform.position == startingPos)
            {
                accelerator = 0.5f;
                goHome = false;          
            }
        }
        #endregion

        #region Go to payment slot
        if (isPayment && !hasArrived)
        {
            if (transform.position != direction)
            {
                Move(direction);
            }
            else
            {
                accelerator = 0.5f;
                transform.rotation = Quaternion.identity;
                effectManager.CheckCanAfford();
                hasArrived = true;
                SoundManager.Instance.PlaySound("Pay");
            }
        }
        #endregion

        #region Hover Code
        if (hoverTimer < 1.5f && isHovered) hoverTimer += Time.deltaTime;

        if (!isHovered) hoverTimer = 0;
        #endregion
    }

    public void Relocate()
    {
        if(Vector3.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(startingPos.x, startingPos.y)) <= 2f) GoHome();
        else
        {
            if (!inHand) TryToPlayCard();
            else GoHome();
            
        }
    }
    
    private void TryToPlayCard()
    {
        bool cardPlayed = false;
        if (gameManager.currentState == GameManager.turnState.CheckMovement && !GameManager.Instance.powerUpOn)
        {
                //Aqui es donde se aplicta el efecto del disfraz y del treat
                goHome = true;
                cardObject.GetComponent<CardObject>().PlayCard();
                cardPlayed = true;
            hand.ResizeHand(false);
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

            if (blackScreen.childCount == 2)
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
                GoHome();
                }
                    #endregion
            }
      
        if(!cardPlayed)
        {
            GoHome();
        }
        else
        {
            SoundManager.Instance.PlaySound("Card Played");
        }
    }

    private void Move(Vector3 desiredPos)
    {
        accelerator += Time.deltaTime * 2;
        transform.position = Vector3.MoveTowards(transform.position, desiredPos, 14 * Time.deltaTime * accelerator);
    }

    public void Disown()
    {
        //Deja de ser hijo de la mano para que esta ense�a que hay una carta menos

        oldParent = transform.parent;
        transform.parent = null;
        Hand.Instance.DeterminePosition();
        Hand.Instance.ResizeHand(false);
        inHand = false;
    }

    private void GoHome()
    {
        if (goHome) return;

        followMouse = false;
        goHome = true;

        float yPos = 0;

        if (Hand.Instance.transform.childCount > 0) yPos = Hand.Instance.transform.GetChild(0).transform.position.y;
        else yPos = Hand.Instance.transform.position.y;

        //transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        //startingPos = new Vector3(transform.position.x, yPos, transform.position.z);
        transform.parent = Hand.Instance.transform;
        inHand = true;
    }
}
