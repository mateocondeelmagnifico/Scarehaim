using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoKeeper : MonoBehaviour
{
    //Esto mantiene el valor del volumen cuando la mano se destruye

    public static InfoKeeper instance { get; private set;}

    public float volume = 1;
    public int Resolution = 1;
    public bool Fullsreen = true;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
        DontDestroyOnLoad(this);
    }
}
