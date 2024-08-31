using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickRadar : MonoBehaviour
{
    public int numberOfScans;
    public bool ghostMoveOn;

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
}
