using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance{ get; private set; }
    public Sound[] Sonidos;
    [SerializeField] private AudioSource[] Sources;
    [SerializeField]private Slider volumeSlider;

    private float volumeSetting;
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

        volumeSetting = volumeSlider.value;
        PlaySound("Music");
    }

    private void Update()
    {
        volumeSetting = volumeSlider.value;
        Sources[2].volume = volumeSetting;
    }

    //Called by other gameobjects, manages all sounds in the game
    public void PlaySound(string name)
    {
        for(int i = 0; i < Sonidos.Length; i++)
        {
            if(Sonidos[i].name == name)
            {
                Sound mySound = Sonidos[i];

                Sources[mySound.source].volume = mySound.volume * volumeSetting;
                Sources[mySound.source].clip = mySound.clip;
                Sources[mySound.source].Play();
            }
        }
    }
}
