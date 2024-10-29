using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    private bool resetSprite;
    public bool hasTreat, hasMoved, moveSelected, isMoving;

    public Vector3 destination, tempDestination;
    public Vector2 tempVector, myPos;

    private GameManager gameManager;
    private SpriteRenderer rendereador;
    private MouseManager mouseManager;
    private DisplayBigImage display;
    private TrickRadar trickRadar;
    private TurnCheck turnCounter;
    private BoardOverlay overlay;
    private Sprite startSprite;
    public Sprite tempSprite;
    [SerializeField] private GameObject highlight, cards;
    private Transform[] cardGrid;
    private List<GameObject> myHighlights;
    private Hand hand;

    public int turnsWithcostume;

    public string costumeName;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        rendereador = GetComponent<SpriteRenderer>();
        trickRadar = GetComponent<TrickRadar>();
        startSprite = rendereador.sprite;
        display = GetComponent<DisplayBigImage>();
        cardGrid = new Transform[15];
        for(int i = 0; i < 15; i++)
        {
            cardGrid[i] = cards.transform.GetChild(i);
        }
        myHighlights = new List<GameObject>();
    }

    private void Start()
    {
        overlay = BoardOverlay.instance;
        mouseManager = MouseManager.instance;
        hand = Hand.Instance;
        turnCounter = gameManager.turnCounter;
    }

    private void Update()
    {
        #region Change sprite
        if (turnsWithcostume > 0)
        {
            rendereador.sprite = tempSprite;
            display.playerText.text = turnsWithcostume.ToString();
            resetSprite = true;
        }
        else
        {
            if(resetSprite && gameManager.currentState == GameManager.turnState.ReplaceCard)
            {
                TakeOffCostume();
                resetSprite = false;
            }
        }
        #endregion

        if (isMoving)
        {
            Move();

            if(turnsWithcostume <= 0)
            {
                if (transform.position.x == destination.x && transform.position.y == destination.y)
                {
                    #region Check if reach Position
                    isMoving = false;
                    if (hasTreat)
                    {
                        gameManager.powerUpOn = false;
                        hasTreat = false;
                        DespawnHighlights(0);
                        overlay.DeactivatOverlay();
                    }

                    hasMoved = false;
                    gameManager.powerUpOn = false;
                    gameManager.ChangeState(GameManager.turnState.ReplaceCard);
                    #endregion
                }
            }
            else
            {
                #region Check if reach position costume
                    if (!hasMoved)
                    {
                        if (transform.position.x == tempDestination.x && transform.position.y == tempDestination.y)
                        {
                            hasMoved = true;
                        }
                    }
                    else if(transform.position.x == destination.x && transform.position.y == destination.y)
                    {
                        isMoving = false;
                        hand.zPrompt.SetActive(false);
                        
                        gameManager.ChangeState(GameManager.turnState.ReplaceCard);
                        turnsWithcostume--;
                        if(turnsWithcostume <= 0)
                        {
                            gameManager.powerUpOn = false;
                            overlay.DeactivatOverlay();
                            TakeOffCostume();
                            hand.hasMoved = false;
                        }

                        DespawnHighlights(0);
                    }
                    #endregion
            }
        }

        if(gameManager.trapTriggered)
        {
            overlay.ACtivateOverlay("Red");
        }
        else if(turnsWithcostume > 0)
        {
            overlay.ACtivateOverlay("Yellow");
        }
    }

    public void TryMove(Vector2 cardGridPos, Vector2 cardActualPos)
    {
        if (isMoving) return;

        #region Check if can Basic move
        bool canBasicMove = false;
        if (cardGridPos.x <= myPos.x + 1 && cardGridPos.x >= myPos.x - 1 && cardGridPos.y <= myPos.y + 1 && cardGridPos.y >= myPos.y - 1 && cardGridPos != myPos)
        {
            canBasicMove = true;
        }
        #endregion

        if (turnsWithcostume > 0)
        {
            #region Costume Movement
            if (!moveSelected)
            {
                if (canBasicMove)
                {
                    //Set first move and activate undo
                    hand.movimiento = this;
                    hand.costumeOn = true;
                    hand.zPrompt.SetActive(true);

                    tempVector = cardGridPos;
                    tempDestination = new Vector3(cardActualPos.x, cardActualPos.y, -0.15f);
                    moveSelected = true;
                    hasMoved = false;
                    trickRadar.ghostMoveOn = true;

                    #region Spawn and move highlights
                    SpawnHighlight(9);
                    MoveHighlights(0, tempDestination, "yellow");
                    MoveHighlights(1, SeekSlot(new Vector2(tempVector.x, tempVector.y - 1)), "brown");
                    MoveHighlights(2, SeekSlot(new Vector2(tempVector.x + 1, tempVector.y - 1)), "brown");
                    MoveHighlights(3, SeekSlot(new Vector2(tempVector.x + 1, tempVector.y)), "brown");
                    MoveHighlights(4, SeekSlot(new Vector2(tempVector.x + 1, tempVector.y + 1)), "brown");
                    MoveHighlights(5, SeekSlot(new Vector2(tempVector.x, tempVector.y + 1)), "brown");
                    MoveHighlights(6, SeekSlot(new Vector2(tempVector.x - 1, tempVector.y + 1)), "brown");
                    MoveHighlights(7, SeekSlot(new Vector2(tempVector.x - 1, tempVector.y)), "brown");
                    MoveHighlights(8, SeekSlot(new Vector2(tempVector.x - 1, tempVector.y - 1)), "brown");
                    #endregion
                }
            }
            else
            {
                if (cardGridPos.x <= tempVector.x + 1 && cardGridPos.x >= tempVector.x - 1 && cardGridPos.y <= tempVector.y + 1 && cardGridPos.y >= tempVector.y - 1 && cardGridPos != tempVector)
                {

                    destination = new Vector3(cardActualPos.x, cardActualPos.y, -0.15f);
                    myPos = cardGridPos;
                    moveSelected = false;
                    isMoving = true;
                    trickRadar.ghostMoveOn = false;

                    MoveHighlights(1, destination, "yellow");
                    DespawnHighlights(2);
                    hand.DestroyLimbo();
                    hand.hasMoved = true;
                }
            }
            #endregion
        }
        else
        {
            if (!hasTreat)
            {
                #region Normal movement
                if (canBasicMove)
                {
                    destination = new Vector3(cardActualPos.x, cardActualPos.y, -0.13f);
                    myPos = cardGridPos;
                    isMoving = true;
                }
                #endregion
            }
            else
            {
                if (canBasicMove)
                {
                    #region Change card grid pos
                    //This makes you move 2 squares when you use the treat

                    float x = myPos.x - cardGridPos.x;
                    float y = myPos.y - cardGridPos.y;
                    Vector2 originalPosGrid = cardGridPos;
                    Vector2 originalActualPos = cardActualPos;
                    bool cantMove = false;

                    //adjust x position
                    if (x == -1)
                    {
                        if (cardGridPos.x != 3)
                        {
                            cardGridPos.x += 1;
                            cardActualPos.y += 2.7f;
                        }
                        else cantMove = true;
                    }
                    else if (x != 0)
                    {
                        if (cardGridPos.x != 1)
                        {
                            cardGridPos.x -= 1;
                            cardActualPos.y -= 2.7f;
                        }
                        else cantMove = true;
                    }

                    //adjust y position
                    if (y == -1)
                    {
                        if (cardGridPos.y != 5)
                        {
                            cardGridPos.y += 1;
                            cardActualPos.x += 2;
                        }
                        else cantMove = true;
                    }
                    else if (y != 0)
                    {
                        if (cardGridPos.y != 1)
                        {
                            cardGridPos.y -= 1;
                            cardActualPos.x -= 2;
                        }
                        else cantMove = true;
                    }

                    if (!cantMove) cantMove = CanGoThere(cardActualPos);

                    if (cantMove)
                    {
                        cardGridPos = originalPosGrid;
                        cardActualPos = originalActualPos;
                    }
                    else
                    {
                        for (int i = 0; i < 15; i++)
                        { 
                            if (cardGrid[i].position.x == cardActualPos.x && cardGrid[i].position.y == cardActualPos.y)
                            {
                                if(cardGrid[i].childCount > 0) gameManager.selectedCardSlot = cardGrid[i].gameObject;
                                else
                                {
                                    cardGridPos = originalPosGrid;
                                    cardActualPos = originalActualPos;
                                }
                            }
                        }
                    }

                    #endregion
                }

                if ((cardGridPos.x == myPos.x + 2 || cardGridPos.x == myPos.x - 2 || cardGridPos.x == myPos.x) && (cardGridPos.y == myPos.y || cardGridPos.y == myPos.y + 2 || cardGridPos.y == myPos.y - 2))
                {
                    #region Calculate Enemy position
                    float xposition = cardGridPos.x;
                    float yposition = cardGridPos.y;
                    if (cardGridPos.x == myPos.x + 2)
                    {
                        xposition = cardGridPos.x - 1;
                    }
                    if (cardGridPos.x == myPos.x - 2)
                    {
                        xposition = cardGridPos.x + 1;
                    }
                    if (cardGridPos.y == myPos.y + 2)
                    {
                        yposition = cardGridPos.y - 1;
                    }
                    if (cardGridPos.y == myPos.y - 2)
                    {
                        yposition = cardGridPos.y + 1;
                    }

                    Vector2 middlePos = new Vector2(xposition, yposition);
                    #endregion

                    if (cardGridPos != myPos && gameManager.enemy.myPos != middlePos)
                    {
                        if(!CanGoThere(cardActualPos))
                        {
                            destination = new Vector3(cardActualPos.x, cardActualPos.y, -0.13f);
                            myPos = cardGridPos;
                            isMoving = true;
                            mouseManager.hasTreat = false;
                            mouseManager.hover2Pos = null;
                            hand.DestroyLimbo();
                        }
                    }
                }
            }
        }
    }
       
    private void Move()
    {
        if (turnsWithcostume > 0)
        {
            if (!hasMoved)
            {
                transform.position = Vector3.MoveTowards(transform.position, tempDestination, 4.5f * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, 4.5f * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 4.5f * Time.deltaTime);
        }

        gameManager.ChangeState(GameManager.turnState.Moving);
    }

    public void SpawnHighlight(int howMany)
    {
        if (howMany <= myHighlights.Count) return;

        for (int i = 0; i < howMany; i++)
        {
            if (i >= myHighlights.Count)
            {
                myHighlights.Add(GameObject.Instantiate(highlight, transform.position, Quaternion.identity));
                myHighlights[i].GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        }
    }

    public void DespawnHighlights(int exceptions)
    {
        for (int i = 0; i < myHighlights.Count; i++)
        {
            if(i >= exceptions)
            {
                myHighlights[i].SetActive(false);
            }
        }
    }
    public void MoveHighlights(int whichOne, Vector2 pos, string myColor)
    {
        if (pos == new Vector2(20, 20)) return;
        
        //If the position wanted does not exist, object is not set active
        myHighlights[whichOne].SetActive(true);
        myHighlights[whichOne].transform.position = pos;
        SpriteRenderer renderer = myHighlights[whichOne].GetComponent<SpriteRenderer>();

        if (myColor == "yellow") renderer.color = Color.yellow;
        if (myColor == "brown") renderer.color = new Color(1, 1, 1, 0.4f);
        if (myColor == "red") renderer.color = Color.red;
        if (myColor == "blue") renderer.color = Color.green;
    }
    public Vector2 SeekSlot(Vector2 slotPos)
    {
        //Get the transform of an element in the cardgrid
        Vector2 wantedpos = new Vector2(20,20);

        for (int i= 0; i < cardGrid.Length; i++)
        {
           CardSlot currentSlot = cardGrid[i].GetComponent<CardSlot>();

            if (currentSlot.Location.x == slotPos.x && currentSlot.Location.y == slotPos.y)
            {
                if(cardGrid[i].childCount > 0) wantedpos = new Vector2(cardGrid[i].position.x, cardGrid[i].position.y);
            }
        }
        return wantedpos;
    }
    public void DisplayTreatHighlight(Vector2 pos)
    {
        if (isMoving) return;

        float x = myPos.x - pos.x;
        float y = myPos.y - pos.y;
        bool cantMove = false;

        //adjust x position
        if (x == -1)
        {
            if (pos.x != 3)
            {
                pos.x += 1;
            }
            else cantMove = true;
        }
        else if (x != 0)
        {
            if (pos.x != 1)
            {
                pos.x -= 1;
            }
            else cantMove = true;
        }

        //adjust y position
        if (y == -1)
        {
            if (pos.y != 5)
            {
                pos.y += 1;
            }
            else cantMove = true;
        }
        else if (y != 0)
        {
            if (pos.y != 1)
            {
                pos.y -= 1;
            }
            else cantMove = true;
        }

        if (!cantMove)
        {
            SpawnHighlight(1);
            MoveHighlights(0, SeekSlot(pos), "yellow");
        }
        else
        {
            DespawnHighlights(0);
        }
    }
    public void TakeOffCostume()
    {
        costumeName = "None";
        rendereador.sprite = startSprite;
        display.ResetImage();
        turnCounter.DisplayCostumeTurns(true);
    }
    public void PutOnCostume(Sprite image, string name)
    {
        turnsWithcostume = 3;
        tempSprite = image;
        costumeName = name;
        turnCounter.DisplayCostumeTurns(false);
    }
    public void UndoCostumeMove()
    {
        moveSelected = false;
        trickRadar.ghostMoveOn = false;
        DespawnHighlights(0);
    }
    public void ActivateTreat()
    {
        hasTreat = true;
        mouseManager.hasTreat = true;
        mouseManager.DeactivateRadar();
    }

    private bool CanGoThere(Vector2 pos)
    {
        bool istrue = false;

        //Avoid steeping into a place you can't with a treat
        for (int o = 0; o < cards.transform.childCount; o++)
        {
            
            Transform cardPos = cards.transform.GetChild(o).transform;
            if (pos.x == cardPos.position.x && pos.y == cardPos.position.y)
            {
                    if (cardPos.GetComponent<CardSlot>().unavailable > 0) istrue = true;
            }
            
        }

        //Returns true when you can't go there
        return istrue;
    }
}
