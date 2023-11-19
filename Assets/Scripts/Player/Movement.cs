using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector2 myPos;

    [SerializeField] private bool isMoving;

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
                gameManager.currentState = GameManager.turnState.CheckCardEffect;
            }
        }
    }

    public void TryMove(Vector2 cardGridPos, Vector2 cardActualPos)
    {
        if (cardGridPos.x <= myPos.x + 1 && cardGridPos.x >= myPos.x - 1 && cardGridPos.y <= myPos.y + 1 && cardGridPos.y >= myPos.y - 1 && !isMoving)
        {
            destination = cardActualPos;
            myPos = cardGridPos;
            isMoving = true;
        }
    }
    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, 4.5f * Time.deltaTime);
        gameManager.currentState = GameManager.turnState.Moving;
    }
}
