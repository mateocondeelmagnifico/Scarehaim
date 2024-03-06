using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{
    private GameManager manager;
    private Camera myCam;
    public Movement playerMove;
    public Image display, blackBox;
    private Hand hand;

    private bool cardGrabbed, handDisplayed;
    public bool moveCard, cardInformed, canClick;

    private float handtimer;

    public GameObject selectedCardSlot, hoverAesthetics;

    private void Start()
    {
        manager = GameManager.Instance;
        myCam = Camera.main;
        hand = Hand.Instance;
        display.enabled = false;
        blackBox.enabled = false;

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
                #region Place Highlight
                hoverAesthetics.SetActive(true);
                hoverAesthetics.transform.position = hit.collider.transform.position;
                hoverAesthetics.transform.rotation = hit.collider.transform.rotation;
                #endregion

                if (hit.collider.gameObject.tag.Equals("Player") || hit.collider.gameObject.tag.Equals("Enemy"))
                {
                    //Check if it's hitting a player or enemy
                    hit.collider.gameObject.GetComponent<DisplayBigImage>().isHovered = true;
                    hit.collider.gameObject.GetComponent<DisplayBigImage>().otherTimer = 0.2f;

                    if (hit.collider.GetComponent<DisplayBigImage>().hoverTimer > 0.8f)
                    {
                        display.enabled = true;
                        blackBox.enabled = true;
                        display.sprite = hit.collider.GetComponent<DisplayBigImage>().bigImage;
                    }
                }
                else
                {
                    #region Set Variables
                    CardSlot currentCard = hit.collider.gameObject.GetComponent<CardSlot>();
                    currentCard.isHovered = true;
                    currentCard.otherTimer = 0.2f;
                    #endregion

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!currentCard.isInHand)
                        {
                            SoundManager.Instance.PlaySound("Card Picked");
                            if (manager.currentState == GameManager.turnState.CheckMovement && currentCard.transform.childCount > 0)
                            {
                                manager.selectedCardSlot = hit.collider.gameObject;
                                cardInformed = false;
                                playerMove.TryMove(currentCard.Location, new Vector2(currentCard.transform.position.x, currentCard.transform.position.y));
                            }
                        }
                    }

                    //This code is for cards in your hand
                    if (currentCard.isInHand)
                    {
                        CardSlotHand currentCardHand = hit.collider.gameObject.GetComponent<CardSlotHand>();

                        if (handtimer < 1.5f) handtimer += Time.deltaTime; ;

                        if (Input.GetMouseButton(0) && !cardGrabbed)
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
                    }
                    else
                    {
                        #region Display Big image
                        if (manager.currentState != GameManager.turnState.CheckCardEffect)
                        {
                            //this is to display the card on the left
                            if (currentCard.hoverTimer > 0.8f)
                            {
                                display.enabled = true;
                                blackBox.enabled = true;
                                if (hit.collider.transform.childCount > 0)
                                {
                                    display.sprite = hit.collider.GetComponentInChildren<Card>().bigImage;
                                }
                            }
                        }
                        #endregion

                        ShrinkHand();
                    }
                }
            }
            else
            {
                ShrinkHand();
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
}
