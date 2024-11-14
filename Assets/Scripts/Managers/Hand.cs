using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hand : MonoBehaviour
{
    private Transform[] cards;
    private Vector3 defaultPos;
    public GameObject[] cardsStart;
    public GameObject  zPrompt, cardStorage;
    private GameObject cardInLimbo;
    private BoardOverlay overlay;
    public Movement movimiento;

    public static Hand Instance { get; set;}

    //la mano guarda el fear entre escenas y sabe si has hecho el tutorial
    public int hope;
    [SerializeField] private int yPos;
    public bool firstGame, costumeOn, activateColliders, resetCards, hasMoved;


    private float timer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(cardStorage);
            DontDestroyOnLoad(zPrompt);
            firstGame = true;
            DetermineStartCards();
            hope = 5;          

            if (SceneManager.GetActiveScene().buildIndex == 1) hope = 10;
        }
        else 
        {
            #region Give cards to old Hand
            Hand myHand = Hand.Instance;

            if (myHand.resetCards) 
            {
                
                int childAmount1 = transform.childCount;
                Destroy(myHand.cardStorage.gameObject);
                DontDestroyOnLoad(cardStorage);
                myHand.cardsStart = new GameObject[transform.childCount];

                for (int i = 0; i < childAmount1; i++)
                {
                    Transform mycard = transform.GetChild(i);
                    mycard.GetComponent<CardSlotHand>().inHand = true;
                    mycard.GetComponent<CardSlotHand>().enabled = true;
                    mycard.GetComponent<BoxCollider2D>().enabled = true;
                    myHand.cardsStart[i] = mycard.gameObject;
                }

                myHand.cardStorage = cardStorage;
                myHand.resetCards = false;
            }

            Debug.Log(cardStorage.transform.childCount);
            Destroy(this.gameObject);
            #endregion
        }

        defaultPos = new Vector3(transform.position.x, transform.position.y, -4);

        DeterminePosition();
    }

    private void Update()
    {
        //Check hand size
        if (transform.childCount != 0)
        if(transform.childCount != cards.Length) DeterminePosition();

        //Undo play card
        if(Input.GetKeyDown(KeyCode.Z) && cardInLimbo != null) Undo();

        //Reset colliders after undo
        if(activateColliders)
        {
            timer -=Time.deltaTime;

            if(timer <= 0)
            {
                ActivateColliders(true);
                activateColliders = false;
            }
        }
    }

    public void AddCardToHand(Transform card)
    {
        card.parent = this.transform;
    }
    public void DeterminePosition()
    {
        //Give the cards the position in the hand they're meant to have

        cards = new Transform[transform.childCount];

        if (transform.childCount > 0)
        {
            #region Reset position and rotation
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = transform.GetChild(i);

                cards[i].rotation = Quaternion.identity;
                cards[i].GetComponent<CardSlotHand>().startingPos = new Vector3(defaultPos.x, yPos, defaultPos.z);
                cards[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 20 - i;
            }
            #endregion

            #region Give new Position
            for (int i = 0; i < cards.Length; i++)
            {
                //Moves cads in hand
                //This if is so that the card in the middle stays put

                #region Rotation multiplier and offset
                float rotMultiplier;
                Vector3 positionOffset;

                if (cards.Length == 2 || cards.Length == 4)
                {
                    rotMultiplier = 0.6f;
                    positionOffset = new Vector3(0.7f, 0, 0.1f);
                }
                else
                {
                    rotMultiplier = 1;
                    positionOffset = new Vector3(1, 0, 0.1f);
                }
                #endregion

                if ((cards.Length == 3 && i == 1) || (cards.Length == 5 && i == 2) || cards.Length == 1)
                {
                    //Do nothing
                    cards[i].rotation = Quaternion.identity;
                    cards[i].GetComponent<CardSlotHand>().startingPos = new Vector3(defaultPos.x, yPos, defaultPos.z);
                }
                else
                {
                    if (cards.Length / (i + 1) < 2)
                    {
                        //Upper half
                        MoveAndRot(i, positionOffset, -rotMultiplier);

                        if (i + 2 == cards.Length && cards.Length > 3)
                        {
                            //Cards in the extremes are more rotated
                            MoveAndRot(i + 1, positionOffset, -rotMultiplier);
                        }
                    }
                    else
                    {
                        //Lower Half
                        MoveAndRot(i, -positionOffset, rotMultiplier);

                        if (i - 1 == 0 && cards.Length > 3)
                        {
                            //Cards in the extremes are more rotated
                            MoveAndRot(i - 1, -positionOffset, rotMultiplier);
                        }
                    }
                }
            }
            #endregion
        }
    }
    private void MoveAndRot(int whatcard, Vector3 offset, float multiplier)
    {
        cards[whatcard].GetComponent<CardSlotHand>().startingPos += offset;
        cards[whatcard].Rotate(0, 0, 10 * multiplier);
    }
    public void ResizeHand(bool makeBig)
    {
        if (transform.childCount < 1)
        {
            yPos = -5;
            return;
        }

        bool goUp = false;
        bool goDown = false;
        if (makeBig)
        {
            if(transform.GetChild(0).position.y <= -4) goUp = true;
        }
        else 
        {
            switch(transform.GetChild(0).position.y)
            {
                case <-5:
                    goUp = true;
                    break;

                case >-5:
                    goDown = true;
                    break;
            }
        }

        if (transform.childCount > 0)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (goUp)
                {
                    yPos = -4;
                    DeterminePosition();
                }

                if (goDown)
                {
                    yPos = -5;
                    DeterminePosition();
                }
            }
        } 
    }
    public void UpdateFear()
    {
        //llamado por botones
        hope = GameManager.Instance.player.GetComponent<Fear>().hope;

        DetermineStartCards();
    }

    public void RefreshCards()
    {
        if (firstGame)
        {
            firstGame = false;
            return;
        }
        Debug.Log(cardStorage.transform.childCount);
        int childAmount = transform.childCount;

        for (int i = childAmount; i > 0; i--)
        {
           Destroy(transform.GetChild(i - 1).gameObject);
        }


        for (int i = 0; i < cardsStart.Length; i++)
        {
           GameObject currentCard = GameObject.Instantiate(cardsStart[i].gameObject, transform);
           currentCard.SetActive(true);
        }

        yPos = -8;
        Debug.Log(cardStorage.transform.childCount);
        DeterminePosition();
    }

    private void DetermineStartCards()
    {
        if (resetCards) return;
        Debug.Log("exe");
        cardsStart = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            cardsStart[i] = GameObject.Instantiate(transform.GetChild(i).gameObject, cardStorage.transform);
            cardsStart[i].SetActive(false);
        }
        Debug.Log(cardStorage.transform.childCount);
    }
    public void MoveHand(int x)
    {
        //esto manda a la mano al centro durante los pagos y luego la devuelve
        defaultPos = new Vector3(x, transform.position.y, -4);

        transform.position = new Vector3(x, transform.position.y, transform.position.z);
        DeterminePosition();
    }
    public void HideHand(bool hide)
    {
        //Hide The hand below the screen at the start of the game
        for (int i = 0; i < cards.Length; i++)
        {        
            if (hide)
            {
                yPos = -8;
                cards[i].GetComponent<CardSlotHand>().inHand = true;
                DeterminePosition();
            }
            else
            {
                yPos = -5;
                cards[i].GetComponent<CardSlotHand>().startingPos.y = yPos;
            }
        }
    }
    public void PutCardInLimbo(GameObject Slot)
    {
        cardInLimbo = Slot;
        cardInLimbo.SetActive(false);
        cardInLimbo.transform.parent = cardStorage.transform;
        ActivateColliders(false);
        ActivateUndo();
    }
    public void Undo()
    {
        if(costumeOn) UndoCostumeMove();
        else UndoLimbo();

        zPrompt.SetActive(false);
        DeterminePosition();
    }
    public void UndoLimbo()
    {
        cardInLimbo.SetActive(true);
        cardInLimbo.transform.GetChild(0).GetComponent<CardObject>().myCard.UndoEffect();
        TurnCheck.instance.UndoCostumes();
        if (overlay == null) overlay = BoardOverlay.instance;
        overlay.DeactivatOverlay();
        MouseManager.instance.hasTreat = false;
        activateColliders = true;
        timer = 0.25f;

        cardInLimbo.transform.position = transform.position;
        cardInLimbo.transform.parent = transform;      
    }
    private void UndoCostumeMove()
    {
        MouseManager.instance.firstSelect = null;
        MouseManager.instance.hover2Pos = null;
        movimiento.UndoCostumeMove();
        costumeOn = false;
        if(!hasMoved) UndoLimbo();
    }
    public void DestroyLimbo()
    {
        if (cardInLimbo != null)
        {
            zPrompt.SetActive(false);
            Destroy(cardInLimbo);
            activateColliders = true;
            timer = 0.25f;
        }
    }
    public void ActivateUndo()
    {
        zPrompt.SetActive(true);

        //Change size of black cover
        float xSize = 1;
        switch(transform.childCount)
        {
            case <= 3:
                xSize = 1;
                break;

            case >= 4:
                xSize = 1.4f;
                break;
        }

        zPrompt.transform.GetChild(1).localScale = new Vector3(xSize, 1, 1);
    }
    public void NukeSelf()
    {
        //Called by buttons
        Destroy(cardStorage.gameObject);
        Destroy(this.gameObject);
    }
    public void ActivateColliders(bool state)
    {
        GetComponent<BoxCollider2D>().enabled = state;

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = state;
        }
    }
}
