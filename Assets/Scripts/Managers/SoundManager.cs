using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance{ get; private set; }
    public Sound[] Sonidos;
    [SerializeField] private AudioSource[] Sources;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //Called by other gameobjects, manages all sounds in the game
    public void PlaySound(string name)
    {
        for(int i = 0; i < Sonidos.Length; i++)
        {
            if(Sonidos[i].name == name)
            {
                Sound mySound = Sonidos[i];

                Sources[mySound.source].volume = mySound.volume;
                Sources[mySound.source].clip = mySound.clip;
                Sources[mySound.source].Play();
            }
        }
    }
}
