using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private Transform[] cards;
     private Vector3 startingPos;

    public static Hand Instance { get; set;}

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this);
        }
        startingPos = new Vector3(0, -5f, -2);
    }

    public void AddCardToHand(Transform card)
    {
        card.parent = this.transform;
        DeterminePosition();
    }

    private void DeterminePosition()
    {
        cards = new Transform[transform.childCount];
        Vector3 positionOffset = new Vector3(0.5f, 0, 0);

        #region Reset position and rotation
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = transform.GetChild(i);

            cards[i].position = startingPos;
            cards[i].rotation = Quaternion.identity;
            cards[i].GetComponent<CardSlotHand>().startingPos = cards[i].position;
            cards[i].transform.GetChild(0).GetComponent<SpriteRenderer>().rendererPriority = 20 - i;
        }
        #endregion

        #region Give new Position
        for (int i = 0; i < cards.Length; i++)
        {
            //Moves cads in hand
            //This if is so that the card in the middle stays put

            if ((cards.Length == 3 && i == 1) || (cards.Length == 5 && i == 2) || cards.Length == 1)
            {
                //Do nothing
                cards[i].position = startingPos;
                cards[i].rotation = Quaternion.identity;
                cards[i].GetComponent<CardSlotHand>().startingPos = cards[i].position;
            }
            else
            {
                if (cards.Length - i + 1 < i || i + 1 == cards.Length)
                {
                    //Upper half
                    cards[i].position += positionOffset;
                    cards[i].Rotate(0, 0, -10);
                    cards[i].GetComponent<CardSlotHand>().startingPos = cards[i].position;

                    if (i + 2 == cards.Length && cards.Length > 3)
                    {
                        //Cards in the extremes are more rotated
                        cards[i + 1].position += positionOffset;
                        cards[i + 1].Rotate(0, 0, -10);
                        cards[i + 1].GetComponent<CardSlotHand>().startingPos = cards[i + 1].position;
                    }
                }
                else
                {
                    //Lower Half
                    cards[i].position -= positionOffset;
                    cards[i].Rotate(0, 0, 10);
                    cards[i].GetComponent<CardSlotHand>().startingPos = cards[i].position;

                    if (i - 1 == 0 && cards.Length > 3)
                    {
                        //Cards in the extremes are more rotated

                        cards[i - 1].position -= positionOffset;
                        cards[i - 1].Rotate(0, 0, 10);
                        cards[i - 1].GetComponent<CardSlotHand>().startingPos = cards[i - 1].position;
                    }
                }
            }

        }
        #endregion

    }
}
