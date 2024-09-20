using System.Collections.Generic;
using UnityEngine;

public class TurnCheck : MonoBehaviour
{
    [SerializeField] private List<Transform> positions;
    [SerializeField] private Transform startPos;
    [SerializeField] private Sprite enemySprite, playerSprite, costumeSprite;

    [SerializeField] private int moveFrequency;
    public int turnsUntilEnemy;
    private int costumeTurns;

    public void MoveLeft()
    {
        for(int i = 4; i >= 0; i--)
        {
            #region Move images to the left and place the first to the right
            if (i == 0)
            {
                Transform firstIcon = positions[i];
                positions[i].position = startPos.position;
                positions.RemoveAt(0);
                positions.Add(firstIcon);         
            } 
            else positions[i].position = new Vector3(positions[i].position.x - 0.66f, positions[i].position.y, positions[i].position.z);
            #endregion

            #region Disable or enable images
            if (i == 0 || i == 5) positions[i].GetComponent<SpriteRenderer>().enabled = false;
            else positions[i].GetComponent<SpriteRenderer>().enabled = true;
            #endregion

            #region Decide if it's either player or enemy sprite
            if (i == 4)
            {
                if (turnsUntilEnemy > 0)
                {
                    if (costumeTurns <= 0) positions[i].GetComponent<SpriteRenderer>().sprite = playerSprite;
                    else
                    {
                        positions[i].GetComponent<SpriteRenderer>().sprite = costumeSprite;
                        costumeTurns--;
                    }
                    turnsUntilEnemy--;
                }
                else
                {
                    positions[i].GetComponent<SpriteRenderer>().sprite = enemySprite;
                    turnsUntilEnemy = moveFrequency;
                }
            }
            #endregion
        }
    }

    public void DisplayCostumeTurns(bool takeOff)
    {
        //Display or hide costume turns

        if(takeOff) costumeTurns = 0;
        else costumeTurns = 3;

        for (int i = 0; i < positions.Count; i++)
        {
            if(i >0 && i < 4 && positions[i].GetComponent<SpriteRenderer>().sprite != enemySprite)
            {
                if (!takeOff)
                {
                    positions[i].GetComponent<SpriteRenderer>().sprite = costumeSprite;
                    costumeTurns--;
                }
                else positions[i].GetComponent<SpriteRenderer>().sprite = playerSprite;
            }
        }
    }
}
