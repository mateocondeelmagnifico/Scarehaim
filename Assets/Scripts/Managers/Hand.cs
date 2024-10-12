using UnityEngine;
using UnityEngine.SceneManagement;

public class Hand : MonoBehaviour
{
    private Transform[] cards;
    private Vector3 defaultPos;
    public GameObject[] cardsStart;
    public GameObject cardStorage, zPrompt;
    private GameObject cardInLimbo;
    private BoardOverlay overlay;
    public Movement movimiento;

    public static Hand Instance { get; set;}

    //la mano guarda el fear entre escenas y sabe si has hecho el tutorial
    public int hope;
    public bool firstGame, costumeOn, activateColliders;

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
            Destroy(this.gameObject);
        }

        defaultPos = new Vector3(transform.position.x, transform.position.y, -2);

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

                cards[i].position = new Vector3(defaultPos.x, cards[i].transform.position.y, defaultPos.z);
                cards[i].rotation = Quaternion.identity;
                cards[i].GetComponent<CardSlotHand>().startingPos = cards[i].position;
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
                    positionOffset = new Vector3(0.7f, 0, 0);
                }
                else
                {
                    rotMultiplier = 1;
                    positionOffset = new Vector3(1, 0, 0);
                }
                #endregion

                if ((cards.Length == 3 && i == 1) || (cards.Length == 5 && i == 2) || cards.Length == 1)
                {
                    //Do nothing
                    cards[i].position = new Vector3(defaultPos.x, cards[i].transform.position.y, -3);
                    cards[i].rotation = Quaternion.identity;
                    cards[i].GetComponent<CardSlotHand>().startingPos = cards[i].position;
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
        cards[whatcard].position += offset;
        cards[whatcard].position = new Vector3(cards[whatcard].position.x, cards[whatcard].position.y, -3);
        cards[whatcard].Rotate(0, 0, 10 * multiplier);
        cards[whatcard].GetComponent<CardSlotHand>().startingPos = cards[whatcard].position;
    }

    public void ResizeHand(bool makeBig)
    {
        bool goUp = false;
        bool goDown = false;
        if (makeBig)
        {
            if(cards[0].transform.position.y <= -4) goUp = true;
        }
        else
        {
            switch(cards[0].transform.position.y)
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
                if(goUp) cards[i].GetComponent<CardSlotHand>().startingPos.y = -4;
                
                if(goDown) cards[i].GetComponent<CardSlotHand>().startingPos.y = -5;
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

        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < cardsStart.Length; i++)
        {
           GameObject currentCard =  GameObject.Instantiate(cardsStart[i].gameObject);
           currentCard.transform.parent = this.transform;
           currentCard.SetActive(true);
        }

        DeterminePosition();
    }

    private void DetermineStartCards()
    {
        cardsStart = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            cardsStart[i] = GameObject.Instantiate(transform.GetChild(i).gameObject);
            cardsStart[i].SetActive(false);
            cardsStart[i].transform.parent = cardStorage.transform;
        }
    }

    public void MoveHand(int x)
    {
        //esto manda a la mano al centro durante los pagos y luego la devuelve
        defaultPos = new Vector3(x, transform.position.y, -2);

        transform.position = new Vector3(x, transform.position.y, transform.position.z);
        DeterminePosition();
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
    }
    public void NukeSelf()
    {
        //Called by buttons
        Destroy(cardStorage.gameObject);
        Destroy(this.gameObject);
    }
    private void ActivateColliders(bool state)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = state;
        }
    }
}
