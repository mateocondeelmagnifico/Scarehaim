using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{
    private GameManager manager;
    private Camera myCam;
    public Movement playerMove;
    public Image display;

    private bool cardGrabbed;
    public bool moveCard, cardInformed;

    public GameObject selectedCardSlot;
    private void Start()
    {
        manager = GameManager.Instance;
        myCam = Camera.main;
        display.enabled = false;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    private void Update()
    {
        Raycast();
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

        if(hit.collider != null)
        {
            if (hit.collider.gameObject.tag.Equals("Card Slot") || hit.collider.gameObject.tag.Equals("Player") || hit.collider.gameObject.tag.Equals("Enemy"))
            {
                

                if (hit.collider.gameObject.tag.Equals("Player") || hit.collider.gameObject.tag.Equals("Enemy"))
                {
                    //Check if it's hitting a player or enemy
                    hit.collider.gameObject.GetComponent<DisplayBigImage>().isHovered = true;
                    hit.collider.gameObject.GetComponent<DisplayBigImage>().otherTimer = 0.2f;

                    if (hit.collider.GetComponent<DisplayBigImage>().hoverTimer > 0.8f)
                    {
                        display.enabled = true;
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
                            if (manager.currentState == GameManager.turnState.CheckMovement)
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
                    else if (manager.currentState != GameManager.turnState.CheckCardEffect)
                    {
                        //this is to display the card on the left
                        if (currentCard.hoverTimer > 0.8f)
                        {
                            display.enabled = true;
                            if (hit.collider.transform.childCount > 0)
                            {
                                display.sprite = hit.collider.GetComponentInChildren<Card>().bigImage;
                            }
                        }
                    }
                }
            }
            else
            {
                display.enabled = false;
            }
        }
    }
}
