using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector2 myPos;

    private bool isMoving;
    public bool hasTreat, hasCostume;

    private Vector2 destination;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (isMoving)
        {
            Move();

            if (transform.position.x == destination.x && transform.position.y == destination.y)
            {
                isMoving = false;
                gameManager.powerUpOn = false;
                hasTreat = false;

                if(gameManager.costumeOn)
                {
                    gameManager.currentState = GameManager.turnState.CheckMovement;
                    gameManager.costumeOn = false;
                }
                else
                {
                    gameManager.currentState = GameManager.turnState.CheckCardEffect;
                }
            }
        }

        //este codigo es muy cutre, luego habría que cambiarlo
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.13f);
    }

    public void TryMove(Vector2 cardGridPos, Vector2 cardActualPos)
    {
        if (!hasTreat && !hasCostume)
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
                    if (cardGridPos != myPos)
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
