using System.Collections.Generic;
using UnityEngine;

public class TrickRadar : MonoBehaviour
{
    public int numberOfScans;
    public bool ghostMoveOn;
    private bool moveScans;
    public GameObject scanObject;
    [SerializeField] private TutorialManager tutorialManager;
    private List<GameObject> scans = new List<GameObject>();
    private List<Vector3> desiredPoss = new List<Vector3>();

    public enum Direction
    {
        left, right, down, up
    }

    private void Update()
    {
        if(moveScans)
        {
            for(int i = 0; i < scans.Count; i++)
            {
                scans[i].transform.position = Vector3.MoveTowards(scans[i].transform.position, desiredPoss[i], 6f * Time.deltaTime);
            }
           
            //If the last scan has arrived, destroy all
            if (scans[scans.Count -1].transform.position.x >= desiredPoss[desiredPoss.Count -1].x)
            {
                int listCount = scans.Count;
                for (int i = listCount - 1; i >= 0; i--)
                {
                    Destroy(scans[i]);
                    scans.RemoveAt(i);
                    desiredPoss.RemoveAt(i);
                }
                moveScans = false;
                if (tutorialManager != null)
                {
                   if(tutorialManager.currentTutorial == 6) tutorialManager.radarDone = true;
                }
            }
        }
    }

    public bool CanUseScan()
   {
        bool isTrue = false;

        if(numberOfScans > 0 && !ghostMoveOn)
        {
            numberOfScans--;
            isTrue = true;
        }

        return isTrue;
   }

    public void PlayScanAnim(List <Vector3> cardPos, Direction whatDirection)
    {
        //Make scanner objects
        //Makes as much as you want
        for(int i = 0; i < cardPos.Count; i++)
        {
            scans.Add(GameObject.Instantiate(scanObject, new Vector3(cardPos[i].x -0.85f, cardPos[i].y, cardPos[i].z), Quaternion.identity));
            desiredPoss.Add(new Vector3(scans[i].transform.position.x + 1.75f, scans[i].transform.position.y, scans[i].transform.position.z));
        }
        moveScans = true;
    }
}
