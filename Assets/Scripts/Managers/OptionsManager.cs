using UnityEngine;
using UnityEngine.Localization.Settings;

public class OptionsManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI resText, FullScreenText, LenguajeText;
    private InfoKeeper infoKeeper;
    private bool isFullScreen;
    private int currentRes, selectedLenguaje;
    private Vector2 trueRes;
    
    public static OptionsManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        gameObject.SetActive(false);
    }

    public void SetResolutionAuto() 
    { 
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,true);
    }

    public void ChangeRes(int amount)
    {
        currentRes += amount;
        if(currentRes < 0) currentRes = 8;
        if(currentRes > 8) currentRes = 0;

        SetMyResolution(currentRes);
    }

    public void ChangeFullScreen()
    {
        if(isFullScreen) isFullScreen = false;
        else isFullScreen = true;   

        SetMyResolution(currentRes);

        if (isFullScreen)
        {
            FullScreenText.text = "Full Screen";
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else
        {
            FullScreenText.text = "Windowed";
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
        Debug.Log(LocalizationSettings.AvailableLocales.Locales.Count);
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[selectedLenguaje];
        infoKeeper.Lenguaje = selectedLenguaje;
        PlayerPrefs.SetInt("Lenguaje", selectedLenguaje);
    }

    public void SetMyResolution(int number)
    {
        Vector2 values = Vector2.zero;

        switch(number)
        {
            case 0:
                values = new Vector2(1920,1080);
                break;

            case 1:
                values = new Vector2(1366, 768);
                break;

            case 2:
                values = new Vector2(1280, 1024);
                break;

            case 3:
                values = new Vector2(1440, 900);
                break;

            case 4:
                values = new Vector2(1600, 900);
                break;

            case 5:
                values = new Vector2(1680, 1050);
                break;

            case 6:
                values = new Vector2(1280, 800);
                break;

            case 7:
                values = new Vector2(1024, 768);
                break;

            case 8:
                values = new Vector2(3840, 2160);
                break;
        }


        trueRes = values;
        resText.text = values.x.ToString() + "x" + values.y.ToString();
    }
    public void UpdateValues()
    {
        //Tambien updatea la resolucion, lo llama un botón
        InfoKeeper.instance.volume = SoundManager.Instance.volumeSetting;
        PlayerPrefs.SetFloat("Volume", SoundManager.Instance.volumeSetting);
        UpdateRes(trueRes);
        SetLenguaje();
    }

    public void LoadValues(InfoKeeper keeper)
    {
        infoKeeper = keeper;
        isFullScreen = infoKeeper.Fullsreen;
        selectedLenguaje = infoKeeper.Lenguaje;
        if (isFullScreen) FullScreenText.text = "Full Screen";
        else FullScreenText.text = "Windowed";
        resText.text = Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString();
        SetMyResolution(infoKeeper.Resolution);
        UpdateValues();
    }

    private void UpdateRes(Vector2 value)
    {
        Screen.SetResolution((int)value.x, (int)value.y, isFullScreen);
        PlayerPrefs.SetInt("Resolution", currentRes);
        infoKeeper.Resolution = currentRes;
        infoKeeper.Fullsreen = isFullScreen;
        Camera.main.pixelRect = new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height);
    }
}
