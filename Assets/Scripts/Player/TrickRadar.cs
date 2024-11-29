using System.Collections.Generic;
using UnityEngine;

public class TrickRadar : MonoBehaviour
{
    public int numberOfScans;
    public bool ghostMoveOn;
    private bool moveScans;
    public GameObject scanObject, scanObjectVert;
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
            if (Vector3.Distance(scans[scans.Count - 1].transform.position, desiredPoss[desiredPoss.Count - 1])  <= 0.1f)
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
        //Makes as much as you need
        GameObject myScanObject = scanObject;
        Vector3 offset = new Vector3(0.85f,0,0);
        Quaternion myRot = Quaternion.identity;
        if (whatDirection == Direction.up || whatDirection == Direction.down)
        {
            myRot = Quaternion.Euler(0,0,90);
            myScanObject = scanObjectVert;

            if(whatDirection == Direction.up) offset = new Vector3(0, 1.3f, 0);
            else offset = new Vector3(0, -1.3f, 0);
        }
        else
        {
            if (whatDirection == Direction.left) offset = new Vector3(-0.85f, 0, 0);
        }

        for(int i = 0; i < cardPos.Count; i++)
        {
            scans.Add(GameObject.Instantiate(myScanObject, cardPos[i] - offset, myRot));
            desiredPoss.Add(scans[i].transform.position + (offset * 2));
        }
        moveScans = true;
    }
}
