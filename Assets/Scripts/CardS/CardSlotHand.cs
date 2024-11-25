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
    public float zRot;

    protected override void Start()
    {
        base.Start();
        isInHand = true;
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
            Relocate(transform.position);
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
                zRot = 0;
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

        #region Check rotation
        if (transform.rotation.z > zRot/360 +0.0001f || transform.rotation.z < zRot/360 - 0.0001f)
        {
            int rotOffset = 30;
            if(transform.rotation.z > zRot/360) rotOffset = -30;
            transform.Rotate(0,0, rotOffset * Time.deltaTime);
        }
        #endregion
    }

    public void Relocate(Vector3 pos)
    {
        if (Vector3.Distance(new Vector2(pos.x, pos.y), new Vector2(startingPos.x, startingPos.y)) <= 2f) GoHome();
        else
        {
            if (!inHand) TryToPlayCard();
            else GoHome();      
        }
    }
    
    private void TryToPlayCard()
    {
        Vector3 offset = new Vector3(0.08f, 0.13f,0);
        bool cardPlayed = false;
        if (gameManager.currentState == GameManager.turnState.CheckMovement && !GameManager.Instance.powerUpOn && !inHand)
        {
             //Aqui es donde se aplicta el efecto del disfraz y del treat
             goHome = true;
             cardObject.GetComponent<CardObject>().PlayCard();
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
                        direction = blackScreen.GetChild(chosenSlot).position + offset;
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
        //Deja de ser hijo de la mano para que esta enseña que hay una carta menos

        oldParent = transform.parent;
        transform.parent = null;
        hand.DeterminePosition();
        hand.ResizeHand(false);
        inHand = false;
    }

    private void GoHome()
    {
        if (goHome) return;

        followMouse = false;
        goHome = true;

        float yPos = 0;

        if (hand.transform.childCount > 0) yPos = hand.transform.GetChild(0).transform.position.y;
        else yPos = hand.transform.position.y;

        transform.parent = hand.transform;
        inHand = true;
    }
}
