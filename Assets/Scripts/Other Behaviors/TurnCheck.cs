using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnCheck : MonoBehaviour
{
    public static TurnCheck instance { get; set; }

    [SerializeField] private List<Transform> positions;
    [SerializeField] private Transform startPos;
    [SerializeField] private Sprite enemySprite, playerSprite, costumeSprite;

    [SerializeField] private int moveFrequency, extraTurnsStart;
    private int turnsUntilEnemy;
    private int costumeTurns;

    private bool move, storeMove;
    private bool[] hasArrived = new bool[6];

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        turnsUntilEnemy += extraTurnsStart;
    }

    private void Update()
    {
        //Move when turn progresses
        if (move)
        {
            for (int i = 5; i >= 0; i--)
            {
               //Move towards objective and decelerate when near
               Vector3 currentDestination = positions[i].GetComponent<DestinationInfo>().destination;
                if (positions[i].position != currentDestination)
                {
                    positions[i].position = Vector3.MoveTowards(positions[i].position, currentDestination, 1 * Time.deltaTime);

                    if(i == 0)
                    {
                        if (Vector3.Distance(positions[i].position, currentDestination) < 0.32f) positions[i].GetComponent<SpriteRenderer>().enabled = false;
                    }

                    if (i == 5)
                    {
                        if (Vector3.Distance(positions[i].position, currentDestination) < 0.32f) positions[i].GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
                else 
                {
                    hasArrived[i] = true;
                }              
            }

            #region Check if all have arrived
            int count = 0;

            for(int i = 0; i < hasArrived.Length; i++)
            {
                if (hasArrived[i]) count++;
            }

            if (count == 6) move = false;
            #endregion
        }

        //This is so that moving left doesn't happen 2 times in a row
        if(storeMove && !move)
        {
            MoveLeft();
            storeMove = false;
        }
    }

    public void MoveLeft()
    {
        if (move)
        {
            storeMove = true;
            return;
        }

        for(int i = 5; i >= 0; i--)
        {
            #region Inform images where to go
            if (i == 0)
            {
                Transform firstIcon = positions[i];
                firstIcon.position = startPos.position;
                firstIcon.GetComponent<DestinationInfo>().destination = startPos.position - new Vector3(0.66f,0,0);
                positions.RemoveAt(0);
                positions.Add(firstIcon);

                #region Decide Sprite
                if (turnsUntilEnemy > 0)
                {
                    if (costumeTurns <= 0) positions[i].GetComponent<SpriteRenderer>().sprite = playerSprite;
                    else
                    {
                        if (positions[4].GetComponent<SpriteRenderer>().sprite != enemySprite)
                        {
                            positions[4].GetComponent<SpriteRenderer>().sprite = costumeSprite;
                        }
                        else positions[i].GetComponent<SpriteRenderer>().sprite = costumeSprite;

                        costumeTurns--;

                        if(costumeTurns <= 0) DeleteCostumes();
                    }
                    turnsUntilEnemy--;
                }
                else
                {
                    positions[i].GetComponent<SpriteRenderer>().sprite = enemySprite;
                    turnsUntilEnemy = moveFrequency;
                }
                #endregion            
            }
            else positions[i].GetComponent<DestinationInfo>().destination = new Vector3(positions[i].position.x - 0.66f, positions[i].position.y, positions[i].position.z);

            #endregion
        }

        #region Command Move
        for (int i = 0; i < hasArrived.Length; i++)
        {
            hasArrived[i] = false;
        }
        move = true;
        #endregion
    }

    public void DisplayCostumeTurns(bool takeOff)
    {
        //Display or hide costume turns

        if(takeOff) costumeTurns = 0;
        else costumeTurns = 3;


        for (int i = 0; i < positions.Count; i++)
        {
            if(i > 1 && i < 6 && positions[i].GetComponent<SpriteRenderer>().sprite != enemySprite && costumeTurns > 0)
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

    private void DeleteCostumes()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            if(i== 0 || i == 1 )
            {
                if (positions[i].GetComponent<SpriteRenderer>().sprite == costumeSprite) positions[i].GetComponent<SpriteRenderer>().sprite = playerSprite;
            }    
        }
    }

    public void UndoCostumes()
    {
        //This is in case you undo the costume
        for (int i = 0; i<positions.Count; i++)
        {
            if(positions[i].GetComponent<SpriteRenderer>().sprite == costumeSprite) positions[i].GetComponent<SpriteRenderer>().sprite = playerSprite;
        }
    }
}

