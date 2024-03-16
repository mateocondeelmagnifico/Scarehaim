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
    [SerializeField] private GameObject highlight;
    private GameObject[] myHighlights;

    public int turnsWithcostume;

    public string costumeName;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        rendereador = GetComponent<SpriteRenderer>();
        startSprite = rendereador.sprite;
        display = GetComponent<DisplayBigImage>();
        myHighlights = new GameObject[2];
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
                    gameManager.currentState = GameManager.turnState.ReplaceCard;
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
                        
                        gameManager.currentState = GameManager.turnState.ReplaceCard;
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

        if(turnsWithcostume > 0)
        {
            #region Costume Movement
            if (!moveSelected)
            {
                if (cardGridPos.x <= myPos.x + 1 && cardGridPos.x >= myPos.x - 1 && cardGridPos.y <= myPos.y + 1 && cardGridPos.y >= myPos.y - 1  && cardGridPos != myPos)
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
                #region treat Movement
                //Normal movement
                if (cardGridPos.x <= myPos.x + 1 && cardGridPos.x >= myPos.x - 1 && cardGridPos.y <= myPos.y + 1 && cardGridPos.y >= myPos.y - 1)
                {
                    if (cardGridPos != myPos)
                    {
                        destination = new Vector3(cardActualPos.x, cardActualPos.y, -0.13f);
                        myPos = cardGridPos;
                        isMoving = true;
                    }
                }
                #endregion
            }
            else
            {
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

        gameManager.currentState = GameManager.turnState.Moving;
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
