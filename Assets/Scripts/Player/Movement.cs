using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector2 myPos;

    private bool isMoving, resetSprite;
    public bool hasTreat, hasMoved;

    private Vector2 destination;

    private GameManager gameManager;
    private SpriteRenderer rendereador;
    private Sprite startSprite;
    public Sprite tempSprite;

    public int turnsWithcostume;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        rendereador = GetComponent<SpriteRenderer>();
        startSprite = rendereador.sprite;
    }

    private void Update()
    {
        #region Change sprite
        if (turnsWithcostume > 0)
        {
            rendereador.sprite = tempSprite;
            resetSprite = true;
        }
        else
        {
            if(!resetSprite)
            {
                rendereador.sprite = startSprite;
                GetComponent<DisplayBigImage>().ResetImage();
            }
        }
        #endregion

        if (isMoving)
        {
            Move();

            if (transform.position.x == destination.x && transform.position.y == destination.y)
            {
                isMoving = false;
                if(hasTreat)
                {
                    gameManager.powerUpOn = false;
                    hasTreat = false;
                }
                

                if(turnsWithcostume > 0)
                {
                    if(!hasMoved)
                    {
                        gameManager.currentState = GameManager.turnState.CheckMovement;
                        turnsWithcostume--;
                        hasMoved = true;
                    }
                    else
                    {
                        gameManager.currentState = GameManager.turnState.ReplaceCard;
                        hasMoved = false;
                    }
                }
                else
                {
                    gameManager.powerUpOn = false;
                    gameManager.currentState = GameManager.turnState.ReplaceCard;
                }
            }
        }

        //este codigo es muy cutre, luego habría que cambiarlo
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.13f);
    }

    public void TryMove(Vector2 cardGridPos, Vector2 cardActualPos)
    {
        if (!hasTreat && turnsWithcostume <= 0)
        {
            //Normal movement
            if (cardGridPos.x <= myPos.x + 1 && cardGridPos.x >= myPos.x - 1 && cardGridPos.y <= myPos.y + 1 && cardGridPos.y >= myPos.y - 1 && !isMoving)
            {
                if(cardGridPos != myPos)
                {
                    destination = new Vector3(cardActualPos.x, cardActualPos.y, -0.13f);
                    myPos = cardGridPos;
                    isMoving = true;
                }
            }
        }
        else
        {
            if (cardGridPos.x <= myPos.x + 2 && cardGridPos.x >= myPos.x - 2 && cardGridPos.y <= myPos.y + 2 && cardGridPos.y >= myPos.y - 2 && !isMoving)
            {
                if((cardGridPos.x == myPos.x + 2 || cardGridPos.x == myPos.x - 2) && (cardGridPos.y == myPos.y + 1 || cardGridPos.y == myPos.y - 1) || (cardGridPos.y == myPos.y + 2 || cardGridPos.y == myPos.y - 2) && (cardGridPos.x == myPos.x + 1 || cardGridPos.x == myPos.x - 1))
                {
                    //This is to check you cant move two spaces in one direction and one in another
                }
                else
                {
                    #region Calculate Enemy position
                    float xposition = cardGridPos.x;
                    float yposition = cardGridPos.y;
                    if(cardGridPos.x == myPos.x +2)
                    {
                        xposition = cardGridPos.x - 1;
                    }
                    if(cardGridPos.x == myPos.x - 2)
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

                    Vector2 middlePos = new Vector2(xposition,yposition);
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
        transform.position = Vector2.MoveTowards(transform.position, destination, 4.5f * Time.deltaTime);
        gameManager.currentState = GameManager.turnState.Moving;
    }
}
