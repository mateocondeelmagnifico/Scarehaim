using UnityEngine;
using UnityEngine.SceneManagement;

public class Hand : MonoBehaviour
{
    private Transform[] cards;
    private Vector3 defaultPos;

    public static Hand Instance { get; set;}

    //la mano guarda el fear entre escenas
    public int fear;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        defaultPos = new Vector3(4, -5, -2);
    }

    private void Update()
    {
        //Check hand size
        if(transform.childCount != 0)
        if(transform.childCount != cards.Length) DeterminePosition();
    }

    public void AddCardToHand(Transform card)
    {
        card.parent = this.transform;
        DeterminePosition();
    }

    public void DeterminePosition()
    {
        cards = new Transform[transform.childCount];
        
        #region Reset position and rotation
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = transform.GetChild(i);

            cards[i].position = defaultPos;
            cards[i].rotation = Quaternion.identity;
            cards[i].GetComponent<CardSlotHand>().startingPos = cards[i].position;
            cards[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 20 - i;
        }
        #endregion

        #region Give new Position
        for (int i = 0; i < cards.Length; i++)
        {
            //Moves cads in hand
            //This if is so that the card in the middle stays put

            #region Rotation multiplier and offset
            float rotMultiplier;
            Vector3 positionOffset;

            if (cards.Length == 2 || cards.Length == 4)
            {
                rotMultiplier = 0.6f;
                positionOffset = new Vector3(0.7f, 0, 0);
            }
            else
            {
                rotMultiplier = 1;
                positionOffset = new Vector3(1, 0, 0);
            }
            #endregion

            if ((cards.Length == 3 && i == 1) || (cards.Length == 5 && i == 2) || cards.Length == 1)
            {
                //Do nothing
                cards[i].position = defaultPos;
                cards[i].rotation = Quaternion.identity;
                cards[i].GetComponent<CardSlotHand>().startingPos = cards[i].position;
            }
            else
            {
               if (cards.Length / (i + 1) < 2)
               {
                    //Upper half
                    MoveAndRot(i, positionOffset, -rotMultiplier);

                    if (i + 2 == cards.Length && cards.Length > 3)
                    {
                        //Cards in the extremes are more rotated
                        MoveAndRot(i + 1, positionOffset, -rotMultiplier);
                    }
               }
               else
               {
                    //Lower Half
                    MoveAndRot(i, -positionOffset, rotMultiplier);

                    if (i - 1 == 0 && cards.Length > 3)
                        {
                        //Cards in the extremes are more rotated

                        MoveAndRot(i - 1, -positionOffset, rotMultiplier);
                    }
               }
            }

        }
        #endregion
    }

    private void MoveAndRot(int whatcard, Vector3 offset, float multiplier)
    {
        cards[whatcard].position += offset;
        cards[whatcard].Rotate(0, 0, 10 * multiplier);
        cards[whatcard].GetComponent<CardSlotHand>().startingPos = cards[whatcard].position;
    }

    public void ResizeHand(bool makeBig)
    {
        Vector3 offset;

        if (makeBig)
        {
            offset = new Vector3(0,1,0);
        }
        else
        {
            offset = new Vector3(0, -1, 0);
        }

        if (transform.childCount > 0)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].GetComponent<CardSlotHand>().startingPos += offset;
            }
        } 
    }

    public void UpdateFear()
    {
        //llamado por botones
        fear = GameManager.Instance.player.GetComponent<Fear>().fear;
    }
}
