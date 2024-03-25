using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickRadar : MonoBehaviour
{
    public int numberOfScans;

   public bool CanUseScan()
   {
        bool isTrue = false;
        if(numberOfScans > 0 )
        {
            numberOfScans--;
            isTrue = true;
        }

        return isTrue;
   }
}
