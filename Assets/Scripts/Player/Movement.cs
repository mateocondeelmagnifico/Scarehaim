using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector2 myPos;

    private bool resetSprite;
    public bool hasTreat, hasMoved, moveSelected, isMoving;

    public Vector2 destination, tempDestination, tempVector;

    private GameManager gameManager;
    private SpriteRenderer rendereador;
    private DisplayBigImage display;
    private Sprite startSprite;
    public Sprite tempSprite;
    [SerializeField] private GameObject highlight, cards;
    private GameObject[] myHighlights;
    private Transform[] cardGrid;

    public int turnsWithcostume;

    public string costumeName;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        rendereador = GetComponent<SpriteRenderer>();
        startSprite = rendereador.sprite;
        display = GetComponent<DisplayBigImage>();
        myHighlights = new GameObject[2];
        cardGrid = new Transform[15];
        for(int i = 0; i < 15; i++)
        {
            cardGrid[i] = cards.transform.GetChild(i);
        }
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
                costumeName = "None";
                rendereador.sprite = startSprite;
                GetComponent<DisplayBigImage>().ResetImage();
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
                        
                        gameManager.ChangeState(GameManager.turnState.ReplaceCard);
                        turnsWithcostume--;
                        if(turnsWithcostume <= 0)
                        {
                            gameManager.powerUpOn = false;
                        }

                        myHighlights[0].SetActive(false);
                        myHighlights[1].SetActive(false);
                     }
                    #endregion
            }
        }

        //este codigo es muy cutre, luego habría que cambiarlo
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.13f);
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

                    tempVector = cardGridPos;
                    tempDestination = new Vector3(cardActualPos.x, cardActualPos.y, -0.13f);
                    moveSelected = true;
                    hasMoved = false;

                    SpawnHighlight();
                }
            }
            else
            {
                if (cardGridPos.x <= tempVector.x + 1 && cardGridPos.x >= tempVector.x - 1 && cardGridPos.y <= tempVector.y + 1 && cardGridPos.y >= tempVector.y - 1  && cardGridPos != tempVector)
                {

                    destination = new Vector3(cardActualPos.x, cardActualPos.y, -0.13f);
                    myPos = cardGridPos;
                    moveSelected = false;
                    isMoving = true;

                    SpawnHighlight();
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
                if(canBasicMove)
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
                        if(cardGridPos.x != 3)
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

                    if(cantMove)
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
                                gameManager.selectedCardSlot = cardGrid[i].gameObject;
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
                            destination = new Vector3(cardActualPos.x, cardActualPos.y, -0.13f);
                            myPos = cardGridPos;
                            isMoving = true;
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
                transform.position = Vector2.MoveTowards(transform.position, tempDestination, 4.5f * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, destination, 4.5f * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, 4.5f * Time.deltaTime);
        }

        gameManager.ChangeState(GameManager.turnState.Moving);
    }

    private void SpawnHighlight()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 myDestination = Vector3.zero;
            if (i == 0) myDestination = tempDestination;
            else myDestination = destination;

            if (myHighlights[i] == null)
            { 

                myHighlights[i] = GameObject.Instantiate(highlight, myDestination, Quaternion.identity);
                myHighlights[i].SetActive(true);
                myHighlights[i].GetComponent<SpriteRenderer>().sortingOrder = 0;
                i = 2;
            }
            else if (!myHighlights[i].activeInHierarchy)
            {
                myHighlights[i].transform.position = myDestination;
                myHighlights[i].SetActive(true);
                i = 2;
            }
        }
    }
}
