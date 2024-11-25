using System.Collections.Generic;
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
    private CardEffectManager effectManager;
    [SerializeField] private TMPro.TextMeshProUGUI radarText, hopeText, descriptionText;
    private SoundManager soundManager;
    private CardSlotHand currentCardHand;

    [SerializeField] private Transform board, tricks;

    private bool handDisplayed, highlightsSpawned, cardHandHovered, wantsToDisplay, playerDisplay, canFire, radarsOut;
    public bool moveCard, cardInformed,  isInTutorial, needsTreat, radarActive, cardGrabbed, hasTreat, canClick, dontDisplay;

    private float handtimer, displayTimer, displayTimer2, originalPos;

    private Vector2[] radarPositions;

    public GameObject firstSelect, selectedCardSlot, hoverAesthetics, hoverAesthetics2, trapIndicator, hover2Pos;
    private GameObject cardHit, tutHit;

    private Color startColor;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        manager = GameManager.Instance;
        effectManager = CardEffectManager.Instance;
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
        descriptionText = display.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        descriptionText.enabled = false;
        originalPos = descriptionText.transform.position.y;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    private void Update()
    {
        Raycast();

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

        //Display 0 radars
        if(radarsOut)
        {
            displayTimer2 -= Time.deltaTime;
            if (displayTimer2 <= 0)
            {
                radarsOut = false;
                radarText.enabled = false;
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

        if (radarActive || radarsOut) radarText.transform.position = new Vector3(hit.point.x + 0.8f, hit.point.y - 0.8f, -2);

        if (hit.collider != null)
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

                    if (manager.CheckIsInCheckMovement() || effectManager.effectActive) PlaceHighlight(0);

                    if (hit.collider.gameObject.tag.Equals("Enemy") && manager.CheckIsInCheckMovement() && playerMove.turnsWithcostume <= 0)
                    {
                        playerMove.DespawnHighlights(0);
                    }

                    #region Select Card in Hand

                    if (cardHit.GetComponent<CardSlotHand>())
                    {
                        currentCardHand = cardHit.GetComponent<CardSlotHand>();

                        currentCardHand.isHovered = true;

                        if (currentCardHand.hoverTimer >= 0.2f)
                        {
                            DisplayCard(currentCardHand.objectSprite, currentCardHand.objectDescription, CheckOffset(cardHit.transform.GetChild(0).gameObject));

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
                                effectManager.CheckCanAfford();
                            }
                        }

                        
                    }

                    #endregion
                }
                else
                {
                    cardHit = null;
                    if (cardHandHovered || hit.collider.gameObject.tag.Equals("Undo Button"))
                    {
                        DeactivateDisplay();
                        cardHandHovered = false;
                        if(currentCardHand != null)
                        {
                            currentCardHand.hoverTimer = 0;
                            currentCardHand.isHovered = false;
                        } 
                        hoverAesthetics.SetActive(false);
                    }
                }

                if (Input.GetMouseButtonUp(0) && cardGrabbed && currentCardHand != null)
                {
                    //Finish follow
                    currentCardHand.followMouse = false;
                    cardGrabbed = false;
                    currentCardHand.Relocate(hit.point);
                    hoverAesthetics.SetActive(false);
                    DeactivateDisplay();
                }

                if (hit.collider.gameObject.tag.Equals("BlackScreen"))
                {
                    DeactivateDisplay();
                    hoverAesthetics.SetActive(false);
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
                            DisplayCard(cardHit.GetComponent<DisplayBigImage>().bigImage, "", 0);

                            if (hit.collider.gameObject.tag.Equals("Player"))
                            {
                                playerDisplay = true;
                                hopeText.text = cardHit.GetComponent<Fear>().hope.ToString();
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

                                    if (tutorialManager != null) tutHit = cardHit;
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
                                    else if (tutorialManager.IsCorrectCard(tutHit) == true)
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
                                    DisplayCard(cardHit.GetComponent<CardSlot>().objectSprite, cardHit.GetComponent<CardSlot>().objectDescription, CheckOffset(cardHit.transform.GetChild(0).gameObject));
                                }
                                else DeactivateDisplay();
                            }

                        }
                        #endregion
                    }
                    else if(!hit.collider.gameObject.tag.Equals("Player") && !hit.collider.gameObject.tag.Equals("Enemy")) hoverAesthetics.SetActive(false);

                    if (hit.collider.gameObject.tag.Equals("Undo Button"))
                    {
                        //Press Undo Button
                        hand.Undo();
                    }

                    if (hit.collider.gameObject.tag.Equals("Untagged")) DeactivateDisplay();
                    
                }

                #region Display Hand
                if (cardHit != null || hit.collider.gameObject.CompareTag("Hand"))
                {
                    if (hit.collider.gameObject.CompareTag("Hand") || cardHit.GetComponent<CardSlotHand>())
                    {
                        if (handtimer < 1.5f) handtimer += Time.deltaTime;

                        if (handtimer > 0.2f && !handDisplayed && !manager.moveCardToHand)
                        {
                            hand.ResizeHand(true);
                            handDisplayed = true;
                        }
                    }
                    else ShrinkHand();
                }
                else ShrinkHand();
                #endregion

                #region Check Radar
                if(manager.CheckIsInCheckMovement())
                {
                    #region Toggle and Fire radar
                    if (Input.GetMouseButtonDown(0) && radarActive && canFire)
                    {
                        FireRadar();
                        boardOverlay.DeactivatOverlay();
                        radarText.text = "";
                        soundManager.PlaySound("Radar");
                        canFire = false;
                    }

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
                            radarText.transform.position = new Vector3(hit.point.x + 0.8f, hit.point.y - 0.8f, -1);
                            radarText.text = (trickRadar.numberOfScans + 1).ToString();
                            DeactivateDisplay();
                            hover2Pos = null;
                            hoverAesthetics.SetActive(false);
                            soundManager.PlaySound("Radar On");
                        }

                        //Display 0 radars
                        if (trickRadar.numberOfScans <= 0 && !radarActive)
                        {
                            radarsOut = true;
                            displayTimer2 = 0.6f;
                            radarText.color = Color.red;
                            radarText.enabled = true;
                            radarText.text = "0";
                            radarText.transform.position = new Vector3(hit.point.x + 0.8f, hit.point.y - 0.8f, -1);
                        }
                    }  
                    #endregion

                    //Check Direction of radar
                    if (radarActive)
                    {
                        #region Declare variables & check Direction
                        for(int i = 0; i < 3; i++)
                        {
                            radarPositions[i] = new Vector2(20,20);
                            //If they stay in this value, they won't appear
                        }
                        
                        Vector2 playerPos = new Vector2(playerMove.transform.position.x, playerMove.transform.position.y);
                        if (!highlightsSpawned)
                        {
                            pMovement.SpawnHighlight(3);
                            highlightsSpawned = true;
                        }
                        
                        //Place highlights at their respective place
                        if (Mathf.Abs(hit.point.x - playerPos.x) > Mathf.Abs(hit.point.y - playerPos.y))
                        {
                            if (hit.point.x > playerPos.x)
                            {
                                //Right
                                if(playerPos.x < 7.93)
                                {
                                    if(playerPos.y <  2.65f) radarPositions[0] = new Vector2(playerPos.x + 2, playerPos.y + 2.7f);
                                    radarPositions[1] = new Vector2(playerPos.x + 2, playerPos.y);
                                    if (playerPos.y > -2.69f) radarPositions[2] = new Vector2(playerPos.x + 2, playerPos.y - 2.7f);
                                }
                                else canFire = false;
                            }
                            else
                            {
                                //Left
                                if (playerPos.x > -0.05)
                                {
                                    if (playerPos.y < 2.65f) radarPositions[0] = new Vector2(playerPos.x - 2, playerPos.y + 2.7f);
                                    radarPositions[1] = new Vector2(playerPos.x - 2, playerPos.y);
                                    if (playerPos.y > -2.69f) radarPositions[2] = new Vector2(playerPos.x - 2, playerPos.y - 2.7f);
                                }
                                else canFire = false;
                            }
                        }
                        else
                        {
                            if (hit.point.y > playerPos.y)
                            {
                                //Up
                                if (playerPos.y < 2.65)
                                {
                                    if(playerPos.x > -0.05) radarPositions[0] = new Vector2(playerPos.x - 2, playerPos.y + 2.7f);
                                    radarPositions[1] = new Vector2(playerPos.x, playerPos.y + 2.7f);
                                    if (playerPos.x < 7.93) radarPositions[2] = new Vector2(playerPos.x + 2, playerPos.y + 2.7f);
                                }
                                else canFire = false;
                            }
                            else
                            {
                                //Down
                                if (playerPos.y > -2.5)
                                {
                                    if (playerPos.x > -0.05) radarPositions[0] = new Vector2(playerPos.x - 2, playerPos.y - 2.7f);
                                    radarPositions[1] = new Vector2(playerPos.x, playerPos.y - 2.7f);
                                    if (playerPos.x < 7.93) radarPositions[2] = new Vector2(playerPos.x + 2, playerPos.y - 2.7f);
                                }
                                else canFire = false;
                            }
                        }
                        #endregion

                        //Place highlights in the spots if they are valid
                        for (int i = 0; i < 3; i++)
                        {
                            pMovement.MoveHighlights(i, radarPositions[i], "blue");

                            if(radarPositions[i] != new Vector2(20,20)) canFire = true;
                        }

                        //Set radar text to reflect if you fire the radar
                        if(!canFire) radarText.text = "";
                        else radarText.text = (trickRadar.numberOfScans + 1).ToString();
                    }
                }
                #endregion
            }
            else Deactivation();
        }
        else Deactivation();
    }
    public void DeactivateRadar()
    {
        if(radarActive)
        {
            radarActive = false;
            trickRadar.numberOfScans++;
        }      
        pMovement.DespawnHighlights(0);
        boardOverlay.DeactivatOverlay();
        radarText.text = "";
        soundManager.PlaySound("Radar Off");
    }
    private void Deactivation()
    {
        hoverAesthetics.SetActive(false);
        ShrinkHand();
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
        if (slotScript.isInHand)
        {
            hoverRenderer.color = Color.green;
        }
        else
        {
            if (CanReach(slotScript.Location)) hoverRenderer.color = startColor;
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
    private void FireRadar()
    {
        List<Vector3> posList =new List<Vector3>();

        for(int i = 0;i < tricks.childCount;i++)
        {
            for(int e = 0;e < radarPositions.Length;e++)
            {
                if (radarPositions[e] != new Vector2(20,20))
                {
                    if (Vector2.Distance(radarPositions[e], tricks.GetChild(i).transform.position) <= 0.1f)
                    {
                        GameObject trapMarker = GameObject.Instantiate(trapIndicator, tricks.GetChild(i).transform.position, Quaternion.identity);
                        tricks.GetChild(i).GetComponent<TrickContainer>().myIndicator = trapMarker;
                        trapMarker.transform.parent = tricks.GetChild(i);
                    }

                    posList.Add(radarPositions[e]);
                }
            }
        }

        if (tutorialManager != null)
        {
            if(tutorialManager.currentTutorial == 6)
            tutorialManager.radarDone = true;
        }
        radarActive = false;
        pMovement.DespawnHighlights(0);

        trickRadar.PlayScanAnim(posList);
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
        descriptionText.text = "";
        hoverAesthetics2.SetActive(false);
        if(!playerMove.moveSelected)playerMove.DespawnHighlights(0);
    }
    private void DisplayCard(Sprite spriteToDisplay, string description, float offSet)
    {
        if (dontDisplay) return;

        //Move text
        Vector3 pos = descriptionText.transform.position;
        if (offSet != 0)
        {      
            if (pos.y == originalPos)
            {
                descriptionText.transform.position = new Vector3(pos.x, pos.y + offSet, pos.z);
            }
        }
        else descriptionText.transform.position = new Vector3(pos.x, originalPos, pos.z);

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
        if (radarActive) return;

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

                if (cardHit.CompareTag("Player")) hoverRenderer.color = Color.white;

                if (cardHit.CompareTag("Enemy"))
                {
                    hoverRenderer.color = Color.white;
                    hoverRenderer.sortingOrder = -2;
                }
            }
            else if(manager.CheckIsInCheckMovement())
            {
                hover2Pos = cardHit;
                hoverAesthetics.SetActive(false);
                hoverAesthetics2.SetActive(true);
                hoverAesthetics2.transform.position = cardHit.transform.position;
                hoverAesthetics2.transform.rotation = cardHit.transform.rotation;

                if(cardHit.transform.childCount > 0)
                if (cardHit.transform.GetChild(0).GetComponent<SpriteRenderer>()) hoverRenderer2.sortingOrder = cardHit.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder;

                //Change color depending on availability
                if (CanReach(currentCard.Location)) hoverRenderer2.color = Color.white;
                else hoverRenderer2.color = Color.red;              
            }
        }
        else
        {
            hoverAesthetics.SetActive(false);
        }
    }
    private float CheckOffset(GameObject card)
    {
        //Check if selected card is a treat to move text
        float offset = 0f;
        if (card.CompareTag("Treat")) offset = -0.1f;
        return offset;
    }
}
