using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.U2D;

public class TurnCheck : MonoBehaviour
{
    [SerializeField] private List<Transform> positions;
    [SerializeField] private Transform startPos;
    [SerializeField] private Sprite enemySprite, playerSprite;
    [SerializeField] private int moveFrequency;
    public int turnsUntilEnemy;


    public void MoveLeft()
    {
        for(int i = 4; i >= 0; i--)
        {
            //Move images to the left and place the first to the right
            if (i == 0)
            {
                Transform firstIcon = positions[i];
                positions[i].position = startPos.position;
                positions.RemoveAt(0);
                positions.Add(firstIcon);         
            } 
            else positions[i].position = new Vector3(positions[i].position.x - 0.66f, positions[i].position.y, positions[i].position.z);

            //Disable or enable images
            if (i == 0 || i == 5) positions[i].GetComponent<SpriteRenderer>().enabled = false;
            else positions[i].GetComponent<SpriteRenderer>().enabled = true;

            //Decide if it's either player or enemy sprite
            if (i == 4)
            {
                if (turnsUntilEnemy > 0)
                {
                    positions[i].GetComponent<SpriteRenderer>().sprite = playerSprite;
                    turnsUntilEnemy--;
                }
                else
                {
                    positions[i].GetComponent<SpriteRenderer>().sprite = enemySprite;
                    turnsUntilEnemy = moveFrequency;
                }
            }
        }
    }
}
