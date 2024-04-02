using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;


public class MouseManager : MonoBehaviour
{
    private GameManager manager;
    private Camera myCam;
    public Movement playerMove;
    public Image display, blackBox;
    private Hand hand;
    private TrickRadar trickRadar;
    private SpriteRenderer hoverRenderer;
    private Movement pMovement;
    private EnemyMovement enemyMove;
    private BoardOverlay boardOverlay;
    [SerializeField] private TMPro.TextMeshProUGUI radarText;

    [SerializeField] private Transform board, tricks;

    private bool cardGrabbed, handDisplayed, radarActive, highlightsSpawned;
    public bool moveCard, cardInformed, canClick;

    private float handtimer;

    private Vector2[] radarPositions;
    [SerializeField] private Vector2[] dummyPositions;

    public GameObject selectedCardSlot, hoverAesthetics, trapIndicator;

    private Color startColor;

    private void Start()
    {
        manager = GameManager.Instance;
        myCam = Camera.main;
        hand = Hand.Instance;
        boardOverlay = BoardOverlay.instance;
        pMovement = manager.playerMove;
        enemyMove = manager.enemy;
        trickRadar = pMovement.GetComponent<TrickRadar>();
        display.enabled = false;
        blackBox.enabled = false;
        hoverRenderer = hoverAesthetics.GetComponent<SpriteRenderer>();
        startColor = hoverRenderer.color;
        radarPositions = new Vector2[3];

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    private void Update()
    {
        Raycast();

        if(handtimer > 0.2f && !handDisplayed && !manager.moveCardToHand)
        {
            hand.ResizeHand(true);
            handDisplayed = true;
        }
    }

    private void Raycast()
    {
        #region Raycast Variables
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100;
        mousePos = myCam.ScreenToWorldPoint(mousePos);
        Ray rayo = myCam.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(myCam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        #endregion

        if(hit.collider != null && canClick)
        {
            if (hit.collider.gameObject.tag.Equals("Card Slot") || hit.collider.gameObject.tag.Equals("Player") || hit.collider.gameObject.tag.Equals("Enemy"))
            {
                GameObject cardHit = hit.collider.gameObject;

                #region Place Highlight
                if (cardHit.transform.childCount > 0)
                {
                    hoverAesthetics.SetActive(true);
                    hoverAesthetics.transform.position = cardHit.transform.position;
                    hoverAesthetics.transform.rotation = cardHit.transform.rotation;
                    if (cardHit.transform.GetChild(0).GetComponent<SpriteRenderer>()) hoverRenderer.sortingOrder = cardHit.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder;
                    if (cardHit.GetComponent<CardSlot>()) CheckDistanceToPlayer(cardHit.GetComponent<CardSlot>());
                }
                else
                {
                    hoverAesthetics.SetActive(false);
                }
                #endregion

                if (hit.collider.gameObject.tag.Equals("Player") || hit.collider.gameObject.tag.Equals("Enemy"))
                {
                    //Check if it's hitting a player or enemy
                    cardHit.GetComponent<DisplayBigImage>().isHovered = true;
                    cardHit.GetComponent<DisplayBigImage>().otherTimer = 0.2f;

                    if (cardHit.GetComponent<DisplayBigImage>().hoverTimer > 0.4f)
                    {
                        display.enabled = true;
                        blackBox.enabled = true;
                        display.sprite = cardHit.GetComponent<DisplayBigImage>().bigImage;
                    }

                    if (hit.collider.gameObject.tag.Equals("Enemy") && manager.CheckIsInCheckMovement() && playerMove.turnsWithcostume <= 0)
                    {
                        playerMove.DespawnHighlights(0);
                    }
                }
                else
                {
                    #region Set Variables
                    CardSlot currentCard = hit.collider.gameObject.GetComponent<CardSlot>();
                    currentCard.isHovered = true;
                    currentCard.otherTimer = 0.2f;
                    #endregion

                    if (Input.GetMouseButtonDown(0) && !radarActive)
                    {
                        if (!currentCard.isInHand)
                        {
                            SoundManager.Instance.PlaySound("Card Picked");
                            if (manager.CheckIsInCheckMovement() && currentCard.transform.childCount > 0)
                            {
                                if (playerMove.turnsWithcostume <= 0)
                                {
                                    manager.selectedCardSlot = cardHit;
                                }
                                else
                                {
                                    if (playerMove.moveSelected)
                                    {
                                        manager.selectedCardSlot = cardHit;
                                    }
                                }
                                cardInformed = false;
                                playerMove.TryMove(currentCard.Location, new Vector2(currentCard.transform.position.x, currentCard.transform.position.y));
                            }
                        }
                    }

                    //This code is for cards in your hand
                    if (currentCard.isInHand)
                    {
                        CardSlotHand currentCardHand = cardHit.GetComponent<CardSlotHand>();

                        if (handtimer < 1.5f) handtimer += Time.deltaTime;

                        if (Input.GetMouseButton(0) && !cardGrabbed && (manager.CheckIsInCheckMovement() || manager.currentState == GameManager.turnState.CheckCardEffect))
                        {
                            currentCard = currentCardHand;
                            currentCardHand.followMouse = true;
                            cardGrabbed = true;
                        }

                        if (Input.GetMouseButtonUp(0))
                        {
                            currentCardHand.followMouse = false;
                            cardGrabbed = false;
                        }

                        #region Display Big image
                        if (manager.currentState != GameManager.turnState.CheckCardEffect)
                        {
                            //this is to display the card on the left
                            if (currentCard.hoverTimer > 0.4f)
                            {
                                display.enabled = true;
                                blackBox.enabled = true;
                                if (cardHit.transform.childCount > 0)
                                {
                                    display.sprite = cardHit.GetComponentInChildren<Card>().bigImage;
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region Display Big image
                        if (manager.currentState != GameManager.turnState.CheckCardEffect)
                        {
                            //this is to display the card on the left
                            if (currentCard.hoverTimer > 0.4f)
                            {
                                display.enabled = true;
                                blackBox.enabled = true;
                                if (cardHit.transform.childCount > 0)
                                {
                                    display.sprite = cardHit.GetComponentInChildren<Card>().bigImage;
                                }
                            }
                        }
                        #endregion

                        ShrinkHand();
                    }
                }

                if(!hit.collider.gameObject.tag.Equals("Player") && manager.CheckIsInCheckMovement())
                {
                    #region Check Radar
                    if (Input.GetMouseButtonDown(1))
                    {
                        if (radarActive)
                        {
                            radarActive = false;
                            trickRadar.numberOfScans++;
                            pMovement.DespawnHighlights(0);
                            boardOverlay.DeactivatOverlay();
                            radarText.text = "";
                        }
                        else if (trickRadar.CanUseScan())
                        {
                            radarActive = true;
                            boardOverlay.ACtivateOverlay("Green");
                            radarText.text = "Radars left: " + (trickRadar.numberOfScans + 1);
                        }
                    }

                    if (radarActive)
                    {
                        if (!highlightsSpawned)
                        {
                            pMovement.SpawnHighlight(3);
                            highlightsSpawned = true;
                        }

                        #region Define Variables
                        radarPositions[0] = Vector2.zero;
                        radarPositions[1] = Vector2.zero;
                        radarPositions[2] = Vector2.zero;
                        Vector2 cardVector = new Vector2(cardHit.transform.position.x, cardHit.transform.position.y);
                        float playerY = pMovement.transform.position.y;
                        float playerX = pMovement.transform.position.x;
                        #endregion

                        if (Mathf.Abs(playerX - cardHit.transform.position.x) <= 2 && Mathf.Abs(playerY - cardHit.transform.position.y) <= 2.7f)
                        {
                            
                            //if scanner is in range
                            if (cardHit.transform.position.x != playerX)
                            {
                                pMovement.MoveHighlights(0, new Vector2(cardVector.x, playerY), "blue");
                                radarPositions[0] = new Vector2(cardVector.x, playerY);

                                if (playerY < -2f) { }
                                else
                                {
                                    pMovement.MoveHighlights(1, new Vector2(cardVector.x, playerY - 2.7f), "blue");
                                    radarPositions[1] = new Vector2(cardVector.x, playerY - 2.7f);
                                }

                                if (playerY! > 0.2f) { }
                                else
                                {
                                    pMovement.MoveHighlights(2, new Vector2(cardVector.x, playerY + 2.7f), "blue");
                                    radarPositions[2] = new Vector2(cardVector.x, playerY + 2.7f);
                                }
                            }
                            else
                            {
                                pMovement.MoveHighlights(0, cardVector, "blue");
                                radarPositions[0] = cardVector;

                                if (FindBoardPos(cardVector + new Vector2(-2, 0)))
                                {
                                    pMovement.MoveHighlights(1, cardVector + new Vector2(-2, 0), "blue");
                                    radarPositions[1] = cardVector + new Vector2(-2, 0);
                                }

                                if (FindBoardPos(cardVector + new Vector2(2, 0)))
                                {
                                    pMovement.MoveHighlights(2, cardVector + new Vector2(2, 0), "blue");
                                    radarPositions[2] = cardVector + new Vector2(2, 0);
                                }
                            }
                        }
                    }

                    if (Input.GetMouseButtonDown(0) && radarActive)
                    {
                        FireRadar();
                        boardOverlay.DeactivatOverlay();
                        radarText.text = "";
                    }
                    #endregion
                }
            }
            else
            {
                if(hit.collider.gameObject.tag.Equals("Hand")) handtimer += Time.deltaTime;
                else ShrinkHand();

                if(manager.CheckIsInCheckMovement() && playerMove.turnsWithcostume <= 0) playerMove.DespawnHighlights(0);
                display.enabled = false;
                blackBox.enabled = false;

                hoverAesthetics.SetActive(false);
            }
        }
    }

    private void ShrinkHand()
    {
        if (handtimer > 0 && handDisplayed)
        {
            handtimer = 0;
            handDisplayed = false;
            hand.ResizeHand(false);
        }
    }

    private void CheckDistanceToPlayer(CardSlot slotScript)
    {
        float playerX = pMovement.myPos.x;
        float playerY = pMovement.myPos.y;

        float cardX = slotScript.Location.x;
        float cardY = slotScript.Location.y;

        bool canBasicMove = false;

        if (slotScript.isInHand) hoverRenderer.color = Color.green;
        else
        {
            if(pMovement.turnsWithcostume > 0 && pMovement.moveSelected)
            {
                #region Second turn costume check
                if (pMovement.tempVector.x <= cardX + 1 && pMovement.tempVector.x >= cardX - 1 && pMovement.tempVector.y <= cardY + 1 && pMovement.tempVector.y >= cardY - 1)
                {
                    hoverRenderer.color = startColor;
                }
                else
                {
                    hoverRenderer.color = Color.red;
                }
                #endregion
            }
            else
            {
                if (playerX <= cardX + 1 && playerX >= cardX - 1 && playerY <= cardY + 1 && playerY >= cardY - 1)
                {
                    canBasicMove = true;
                }

                if (pMovement.hasTreat && manager.CheckIsInCheckMovement())
                {
                    #region Horrible treat math
                    if (!canBasicMove && playerX != cardX - 1 && playerX != cardX + 1 && playerY != cardY - 1 && playerY != cardY + 1)
                    {
                        if(playerX <= cardX + 2 && playerX >= cardX - 2 && playerY <= cardY + 2 && playerY >= cardY - 2)
                        {
                            #region Calculate Enemy position
                            float xposition = cardX;
                            float yposition = cardY;
                            if (cardX == playerX + 2)
                            {
                                xposition = cardX - 1;
                            }
                            if (cardX == playerX - 2)
                            {
                                xposition = cardX + 1;
                            }
                            if (cardY == playerY + 2)
                            {
                                yposition = cardY - 1;
                            }
                            if (cardY == playerY - 2)
                            {
                                yposition = cardY + 1;
                            }
                            Vector2 middlePos = new Vector2(xposition, yposition);
                            #endregion

                            if(dummyPositions == null)
                            {
                                if (enemyMove.myPos != middlePos) hoverRenderer.color = startColor;
                                else hoverRenderer.color = Color.red;
                            }
                            else
                            {
                                #region Check for Dummies
                                bool canplace = true;
                                for (int i = 0; i < dummyPositions.Length; i++)
                                {
                                    if (dummyPositions[i] == middlePos)
                                    {
                                        canplace = false;
                                        break;
                                    }
                                }

                                if (enemyMove.myPos != middlePos && canplace) hoverRenderer.color = startColor;
                                else hoverRenderer.color = Color.red;
                                #endregion
                            }
                        }
                        else hoverRenderer.color = Color.red;
                    }
                    else hoverRenderer.color = Color.red;

                    if(dummyPositions == null)
                    {
                        if (canBasicMove) playerMove.DisplayTreatHighlight(slotScript.Location);
                        else playerMove.DespawnHighlights(0);
                    }
                    else
                    {
                        #region Check Dummy Position
                        bool canplace = true;
                        for (int i = 0; i < dummyPositions.Length; i++)
                        {
                            if (dummyPositions[i].x == cardX && dummyPositions[i].y == cardY)
                            {
                                canplace = false;
                                break;
                            }
                        }

                        if(canBasicMove && canplace) playerMove.DisplayTreatHighlight(slotScript.Location);
                        else playerMove.DespawnHighlights(0);
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    if (canBasicMove)
                    {
                        if(dummyPositions != null)
                        {
                            bool canplace = true;
                            for (int i = 0; i < dummyPositions.Length; i++)
                            {
                                if (dummyPositions[i] == new Vector2(cardX,cardY))
                                {
                                    canplace = false;
                                    break;
                                }
                            }

                            if(canplace) hoverRenderer.color = startColor;
                            else hoverRenderer.color = Color.red;
                        }
                        else hoverRenderer.color = startColor;
                    }
                    else
                    {
                        hoverRenderer.color = Color.red;
                    }
                }
            }
           
        }  
    }

    private bool FindBoardPos(Vector2 pos)
    {
        bool isTrue = false;

        for(int i = 0; i < board.childCount; i++)
        {
            if(board.GetChild(i).transform.position.x == pos.x && board.GetChild(i).transform.position.y == pos.y)
            {
                isTrue = true;
                i = board.childCount;
            }
        }

        return isTrue;
    }

    private void FireRadar()
    {
        for(int i = 0;i < tricks.childCount;i++)
        {
            for(int e = 0;e < radarPositions.Length;e++)
            {
                if (radarPositions[e] != Vector2.zero)
                {
                    if (radarPositions[e].x == tricks.GetChild(i).transform.position.x && radarPositions[e].y == tricks.GetChild(i).transform.position.y)
                    {
                        tricks.GetChild(i).GetComponent<Trick>().myIndicator = GameObject.Instantiate(trapIndicator, tricks.GetChild(i).transform.position, Quaternion.identity);
                    }
                }
            }
        }
        radarActive = false;
        pMovement.DespawnHighlights(0);
    }
}
