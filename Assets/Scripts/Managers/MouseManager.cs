using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{
    public static MouseManager instance { get; private set; }
    private GameManager manager;
    [HideInInspector] public TutorialManager tutorialManager;
    private Camera myCam;
    public Movement playerMove;
    public Image display, blackBox;
    private Hand hand;
    private CardSlot currentCard;
    private TrickRadar trickRadar;
    private SpriteRenderer hoverRenderer, hoverRenderer2;
    private Movement pMovement;
    private EnemyMovement enemyMove;
    private BoardOverlay boardOverlay;
    [SerializeField] private TMPro.TextMeshProUGUI radarText, hopeText, costumeTurnsText, descriptionText;
    private SoundManager soundManager;
    private CardSlotHand currentCardHand;

    [SerializeField] private Transform board, tricks;

    private bool handDisplayed, highlightsSpawned, cardHandHovered, wantsToDisplay, playerDisplay;
    public bool moveCard, cardInformed, canClick, isInTutorial, needsTreat, radarActive, cardGrabbed, hasTreat;

    private float handtimer, displayTimer;

    private Vector2[] radarPositions;

    public GameObject firstSelect, selectedCardSlot, hoverAesthetics, hoverAesthetics2, trapIndicator, hover2Pos;
    private GameObject cardHit;

    private Color startColor;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        manager = GameManager.Instance;
        myCam = Camera.main;
        hand = Hand.Instance;
        soundManager = SoundManager.Instance;
        boardOverlay = BoardOverlay.instance;
        pMovement = manager.playerMove;
        enemyMove = manager.enemy;
        trickRadar = pMovement.GetComponent<TrickRadar>();
        display.enabled = false;
        blackBox.enabled = false;
        hopeText.enabled = false;
        hoverRenderer = hoverAesthetics.GetComponent<SpriteRenderer>();
        hoverRenderer2 = hoverAesthetics2.GetComponent<SpriteRenderer>();
        startColor = hoverRenderer.color;
        radarPositions = new Vector2[3];
        descriptionText = display.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        descriptionText.enabled = false;

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

        if(!manager.CheckIsInCheckMovement())
        {
            DeactivateDisplay();
            
            if(manager.currentState != GameManager.turnState.CheckCardEffect) hoverAesthetics.SetActive(false);
        }

        if (wantsToDisplay)
        {
            displayTimer -= Time.deltaTime;
            if(displayTimer <= 0)
            {
                if (playerDisplay)
                {
                    hopeText.enabled = true; 
                    playerDisplay = false;
                }
                else descriptionText.enabled = true;

                display.enabled = true;
                blackBox.enabled = true;
            }
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

        if (radarActive) radarText.transform.position = new Vector3(hit.point.x + 0.8f, hit.point.y - 0.8f, 0);

        if (hit.collider != null )
        {
            if (canClick)
            {
                if (hit.collider.gameObject.tag.Equals("Card Slot") || hit.collider.gameObject.tag.Equals("Player") || hit.collider.gameObject.tag.Equals("Enemy"))
                {
                    //Put highlights when hovering over cards
                    cardHit = hit.collider.gameObject;

                    if (hit.collider.gameObject.tag.Equals("Card Slot"))
                    {
                        currentCard = cardHit.GetComponent<CardSlot>();
                        if (cardHandHovered && !cardHit.GetComponent<CardSlotHand>())
                        {
                            DeactivateDisplay();
                            cardHandHovered = false;
                            currentCardHand.hoverTimer = 0;
                            currentCardHand.isHovered = false;
                            hoverAesthetics.SetActive(false);
                        }
                    }

                    PlaceHighlight(0);

                    if (hit.collider.gameObject.tag.Equals("Enemy") && manager.CheckIsInCheckMovement() && playerMove.turnsWithcostume <= 0)
                    {
                        playerMove.DespawnHighlights(0);
                    }

                    #region Select Card in Hand

                    if (cardHit.GetComponent<CardSlotHand>())
                    {
                        currentCardHand = cardHit.GetComponent<CardSlotHand>();

                        currentCardHand.isHovered = true;
                        handtimer += Time.deltaTime;

                        if (currentCardHand.hoverTimer >= 0.2f)
                        {
                            DisplayCard(currentCardHand.objectSprite, currentCardHand.objectDescription);

                            cardHandHovered = true;
                        }

                        if (Input.GetMouseButton(0) && !cardGrabbed && (manager.CheckIsInCheckMovement() || manager.currentState == GameManager.turnState.CheckCardEffect))
                        {
                            //follow mouse
                            currentCard = currentCardHand;
                            currentCardHand.followMouse = true;
                            currentCardHand.Disown();
                            cardGrabbed = true;

                            if (currentCardHand.isPayment)
                            {
                                currentCardHand.isPayment = false;
                                currentCardHand.hasArrived = false;
                                CardEffectManager.Instance.CheckCanAfford();
                            }
                        }

                        if (Input.GetMouseButtonUp(0))
                        {
                            //Finish follow
                            currentCardHand.followMouse = false;
                            cardGrabbed = false;
                            hoverAesthetics.SetActive(false);
                            DeactivateDisplay();
                        }
                    }

                    
                    #endregion
                }
                else
                {

                    if (cardHandHovered || hit.collider.gameObject.tag.Equals("Undo Button"))
                    {
                        DeactivateDisplay();
                        cardHandHovered = false;
                        currentCardHand.hoverTimer = 0;
                        currentCardHand.isHovered = false;
                        hoverAesthetics.SetActive(false);
                    }

                    if (hit.collider.gameObject.tag.Equals("Hand") && handtimer < 1.5f)
                    {
                        handtimer += Time.deltaTime;
                    }
                    else ShrinkHand();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.gameObject.tag.Equals("Player") || hit.collider.gameObject.tag.Equals("Enemy"))
                    {
                        DisplayBigImage display = cardHit.GetComponent<DisplayBigImage>();
                        //Display player or enemy card
                        if (firstSelect != cardHit)
                        {
                            hoverAesthetics.SetActive(false);
                            firstSelect = cardHit;
                            DisplayCard(cardHit.GetComponent<DisplayBigImage>().bigImage, "");

                            if (hit.collider.gameObject.tag.Equals("Player"))
                            {
                                playerDisplay = true;
                                hopeText.text = cardHit.GetComponent<Fear>().hope.ToString();
                                if (cardHit.GetComponent<Movement>().turnsWithcostume > 0) costumeTurnsText.text = cardHit.GetComponent<Movement>().turnsWithcostume.ToString();
                            }
                            else hopeText.enabled = false;
                            PlaceHighlight(1);
                        }
                        else
                        {
                            firstSelect = null;
                            DeactivateDisplay();
                        }
                    }

                    if (hit.collider.gameObject.tag.Equals("Card Slot"))
                    {
                        #region Select card in Board

                        hopeText.enabled = false;
                        if (!radarActive && !currentCard.isInHand)
                        {
                            PlaceHighlight(1);

                            if (firstSelect == cardHit)
                            {
                                if (manager.CheckIsInCheckMovement() && currentCard.transform.childCount > 0)
                                {
                                    if (CanReach(currentCard.Location) && cardHit.GetComponent<CardSlot>().unavailable > 0) SoundManager.Instance.PlaySound("Card Picked");
                                    else SoundManager.Instance.PlaySound("Cant go there");

                                    if (playerMove.turnsWithcostume <= 0)
                                    {
                                        selectedCardSlot = cardHit;
                                        manager.selectedCardSlot = cardHit;
                                    }
                                    else
                                    {
                                        if (playerMove.moveSelected)
                                        {
                                            selectedCardSlot = cardHit;
                                            manager.selectedCardSlot = cardHit;
                                        }
                                    }
                                    cardInformed = false;

                                    #region Inform Player to move and tutorial to progress
                                    if (tutorialManager == null)
                                    {
                                        if (cardHit.GetComponent<CardSlot>().unavailable <= 0 || playerMove.hasTreat)
                                        {
                                            playerMove.TryMove(currentCard.Location, new Vector2(currentCard.transform.position.x, currentCard.transform.position.y));
                                        }

                                        DeactivateDisplay();
                                    }
                                    else if (tutorialManager.IsCorrectCard(selectedCardSlot) == true)
                                    {
                                        if (!needsTreat)
                                        {
                                            tutorialManager.Nextmenu();
                                            playerMove.TryMove(currentCard.Location, new Vector2(currentCard.transform.position.x, currentCard.transform.position.y));
                                        }
                                        else if (pMovement.hasTreat)
                                        {
                                            tutorialManager.Nextmenu();
                                            playerMove.TryMove(currentCard.Location, new Vector2(currentCard.transform.position.x, currentCard.transform.position.y));
                                            needsTreat = false;
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                if (cardHit.transform.childCount > 0)
                                {
                                    soundManager.PlaySound("Card Hovered");
                                    firstSelect = cardHit;
                                    DisplayCard(cardHit.GetComponent<CardSlot>().objectSprite, cardHit.GetComponent<CardSlot>().objectDescription);
                                }
                                else DeactivateDisplay();
                            }

                        }
                        #endregion
                    }

                    if (hit.collider.gameObject.tag.Equals("Undo Button"))
                    {
                        //Press Undo Button
                        hand.Undo();
                    }

                    if (hit.collider.gameObject.tag.Equals("Untagged")) DeactivateDisplay();
                }

                #region Check Radar
                if (!hit.collider.gameObject.tag.Equals("Player") && manager.CheckIsInCheckMovement())
                {
                    if (Input.GetMouseButtonDown(1) && !hasTreat)
                    {
                        if (radarActive)
                        {
                            DeactivateRadar();
                        }
                        else if (trickRadar.CanUseScan())
                        {
                            radarActive = true;
                            boardOverlay.ACtivateOverlay("Green");
                            radarText.text = (trickRadar.numberOfScans + 1).ToString();
                            soundManager.PlaySound("Radar On");
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
                        soundManager.PlaySound("Radar");
                    }

                }
                #endregion
            }
            else if (hit.collider.gameObject.tag.Equals("Tutorial Button"))
            {
                //Hit tutorial button
                hit.collider.gameObject.GetComponent<Animator>().SetBool("Jump", false);
            }
        }
        
    }
    public void DeactivateRadar()
    {
        radarActive = false;
        trickRadar.numberOfScans++;
        pMovement.DespawnHighlights(0);
        boardOverlay.DeactivatOverlay();
        radarText.text = "";
        soundManager.PlaySound("Radar Off");
    }
    private void ShrinkHand()
    {
        if (cardGrabbed) return;

        if (handtimer > 0 && handDisplayed)
        {
            handtimer = 0;
            handDisplayed = false;
            hand.ResizeHand(false);
        }
    }
    private void CheckDistanceToPlayer(CardSlot slotScript)
    {
        if (slotScript.isInHand) hoverRenderer.color = Color.green;
        else
        {
           if(CanReach(slotScript.Location)) hoverRenderer.color = startColor;
           else hoverRenderer.color = Color.red;
        }
    }
    private bool CanReach(Vector2 slot)
    {
        bool iReach = false;
        bool canBasicMove = false;

        float playerX = pMovement.myPos.x;
        float playerY = pMovement.myPos.y;
        float cardX = slot.x;
        float cardY = slot.y;

        if (pMovement.turnsWithcostume > 0 && pMovement.moveSelected)
        {
            #region Second turn costume check
            if (pMovement.tempVector.x <= cardX + 1 && pMovement.tempVector.x >= cardX - 1 && pMovement.tempVector.y <= cardY + 1 && pMovement.tempVector.y >= cardY - 1)
            {
                iReach = true;
            }
            else
            {
                iReach = false;
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
                float xposition = cardX;
                float yposition = cardY;

                if (!canBasicMove && playerX != cardX - 1 && playerX != cardX + 1 && playerY != cardY - 1 && playerY != cardY + 1)
                {
                    if (playerX <= cardX + 2 && playerX >= cardX - 2 && playerY <= cardY + 2 && playerY >= cardY - 2)
                    {
                        #region Calculate Enemy position
                       
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

                        if (enemyMove.myPos != middlePos)
                        {
                            iReach = true;
                        }
                        else
                        {
                            iReach = false;
                        }
                    }
                    else
                    {
                        iReach = false;
                    }
                }
                else
                {
                    iReach = false;
                }

                if (canBasicMove) playerMove.DisplayTreatHighlight(slot);
                else playerMove.DespawnHighlights(0);

                #endregion
            }
            else
            {
                if (canBasicMove)
                {
                    iReach = true;
                }
                else
                {
                    iReach = false;
                }
            }
        }
        return iReach;
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
                        GameObject trapMarker = GameObject.Instantiate(trapIndicator, tricks.GetChild(i).transform.position, Quaternion.identity);
                        tricks.GetChild(i).GetComponent<TrickContainer>().myIndicator = trapMarker;
                        trapMarker.transform.parent = tricks.GetChild(i);
                    }
                }
            }
        }

        if(tutorialManager != null) tutorialManager.radarDone = true;
        radarActive = false;
        pMovement.DespawnHighlights(0);
    }
    public void DeactivateDisplay()
    {
        display.enabled = false;
        descriptionText.enabled = false;
        blackBox.enabled = false;
        hopeText.enabled = false;
        wantsToDisplay = false;
        firstSelect = null;
        hopeText.text = "";
        costumeTurnsText.text = "";
        descriptionText.text = "";
        hoverAesthetics2.SetActive(false);
        if(!playerMove.moveSelected)playerMove.DespawnHighlights(0);
    }
    private void DisplayCard(Sprite spriteToDisplay, string description)
    {
        display.sprite = spriteToDisplay;
        descriptionText.text = description;

        if (!wantsToDisplay)
        {
            displayTimer = 0.4f;
            wantsToDisplay = true;
        }
    }
    private void PlaceHighlight(int whichOne)
    {
        
        if (cardHit.transform.childCount > 0 || cardHit.CompareTag("Enemy"))
        {
            if(whichOne == 0)
            {      

                if (cardHit == hover2Pos)
                {
                    hoverAesthetics.SetActive(false);
                    return;
                }

                hoverAesthetics.SetActive(true);
                hoverAesthetics.transform.position = cardHit.transform.position;
                hoverAesthetics.transform.rotation = cardHit.transform.rotation;
                if(cardHit.transform.childCount > 0)
                if (cardHit.transform.GetChild(0).GetComponent<SpriteRenderer>()) hoverRenderer.sortingOrder = cardHit.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder;

                if (cardHit.GetComponent<CardSlot>()) CheckDistanceToPlayer(cardHit.GetComponent<CardSlot>());
            }
            else
            {

                hover2Pos = cardHit;
                hoverAesthetics.SetActive(false);
                hoverAesthetics2.SetActive(true);
                hoverAesthetics2.transform.position = cardHit.transform.position;
                hoverAesthetics2.transform.rotation = cardHit.transform.rotation;

                if(cardHit.transform.childCount > 0)
                if (cardHit.transform.GetChild(0).GetComponent<SpriteRenderer>()) hoverRenderer2.sortingOrder = cardHit.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder;

                //Change color depending on availability
                if(CanReach(currentCard.Location)) hoverRenderer2.color = Color.white;
                else hoverRenderer2.color = Color.red;
                
            }
        }
        else
        {
            hoverAesthetics.SetActive(false);
        }
    }
}
