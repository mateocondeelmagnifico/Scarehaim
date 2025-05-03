using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI resText, FullScreenText, LenguajeText;
    private InfoKeeper infoKeeper;
    private bool isFullScreen;
    private int currentRes, selectedLenguaje;
    [SerializeField] private LocalizedString fullscreen, windowed;
    private float timer;
    private bool initialCheck, resCheck;

    private Resolution[] resolutions;
    private Resolution tempRes;
    private List<Resolution> filteredResolutions = new List<Resolution>();
    private float currentRefreshRate; 
    private List<string> resolutionNames = new List<string>();

    [SerializeField] private Slider volumeSlider;
    
    public static OptionsManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);    
        timer = 0.5f;
    }

    private void Start()
    {
        resolutions = Screen.resolutions;
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        for(int i = 0; i < resolutions.Length; i++)
        {
            if ((float)resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        for(int i = 0;i < filteredResolutions.Count;i++)
        {
            resolutionNames.Add(filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRateRatio + "Hz");
            
            if (filteredResolutions[i].width == 1920 && filteredResolutions[i].height == 1080)
            {
                currentRes = i;
                resCheck = true;
            }
            /*
            else if(filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height && !resCheck)
            {
                currentRes = i;
                resCheck = true;
            }
            */
        } 
    }

    private void Update()
    {
        //Esto es una gilipollez pero el paquete de localizacion no se carga en el start por alguna razón
        if (timer > 0) timer -= Time.deltaTime;
        else if (!initialCheck)
        {
            LoadValues(InfoKeeper.instance);
            selectedLenguaje = 0;
            initialCheck = true;
        }
    }

    public void ChangeRes(int amount)
    {
        currentRes += amount;
        if(currentRes < 0) currentRes = filteredResolutions.Count - 1;
        if(currentRes >= filteredResolutions.Count) currentRes = 0;

        resCheck = true;

        SetMyResolution(currentRes);
    }

    public void ChangeFullScreen()
    {
        if(isFullScreen) isFullScreen = false;
        else isFullScreen = true;   

        SetMyResolution(currentRes);

        if (isFullScreen)
        {
            FullScreenText.text = fullscreen.GetLocalizedString();
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else
        {
            FullScreenText.text = windowed.GetLocalizedString();
            PlayerPrefs.SetInt("Fullscreen", 0);
        }    
    }

    public void ChangeLenguaje(int value)
    {
        selectedLenguaje += value;
        if (selectedLenguaje < 0) selectedLenguaje = 1;
        if (selectedLenguaje > 1) selectedLenguaje = 0;

        switch(selectedLenguaje)
        {
            case 0:
                LenguajeText.text = "English";
                break;

            case 1:
                LenguajeText.text = "Español";
                break;
        }
    }

    public void SetLenguaje()
    {
        if (LocalizationSettings.AvailableLocales.Locales.Count == 0) return;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[selectedLenguaje];
        infoKeeper.Lenguaje = selectedLenguaje;
        PlayerPrefs.SetInt("Lenguaje", selectedLenguaje);

        switch (selectedLenguaje)
        {
            case 0:
                LenguajeText.text = "English";
                break;

            case 1:
                LenguajeText.text = "Español";
                break;
        }
    }

    public void SetMyResolution(int number)
    {    
        tempRes = filteredResolutions[number];

        resText.text = tempRes.width.ToString() + "x" + tempRes.height.ToString();
    }
    public void UpdateValues()
    {
        //Tambien updatea la resolucion, lo llama un botón
        InfoKeeper.instance.volume = SoundManager.Instance.volumeSetting;
        PlayerPrefs.SetFloat("Volume", SoundManager.Instance.volumeSetting);
        SetMyResolution(currentRes);
        UpdateRes();
        SetLenguaje();
    }

    public void LoadValues(InfoKeeper keeper)
    {        
        infoKeeper = keeper;
        isFullScreen = infoKeeper.Fullsreen;
        selectedLenguaje = infoKeeper.Lenguaje;
        if (isFullScreen) FullScreenText.text = "Full Screen";
        else FullScreenText.text = "Windowed";
        if (infoKeeper.hasRes)
        {
            currentRes = infoKeeper.resolution;
            resCheck = true;
        }
        volumeSlider.value = infoKeeper.volume;
        UpdateValues();
    }

    private void UpdateRes()
    {
        if (!resCheck) return;    
        
        resCheck = false;

        Screen.SetResolution(tempRes.width, tempRes.height, isFullScreen);
        PlayerPrefs.SetInt("Resolution", currentRes);
        infoKeeper.resolution = currentRes;
        infoKeeper.hasRes = true;
        infoKeeper.Fullsreen = isFullScreen;
        //Camera.main.pixelRect = new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height);
    }
}
