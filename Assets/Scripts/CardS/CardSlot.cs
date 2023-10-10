using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot: MonoBehaviour
{
    public Card mycard;
    bool isActivated;
    public Vector2 Location;
    public bool isInHand;

    public float hoverTimer;

    private void Update()
    {
        if (hoverTimer > 0)
        {
            hoverTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<SpriteRenderer>().sprite = mycard.image;
            mycard.Effect(collision.gameObject);
        }
    }
}
