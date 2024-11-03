using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI resText, FullScreenText;
    private InfoKeeper infoKeeper;
    private bool isFullScreen;
    private int currentRes;

    private void Start()
    {
        infoKeeper = InfoKeeper.instance;
        isFullScreen = infoKeeper.Fullsreen;
        if(isFullScreen) FullScreenText.text = "Full Screen";
        else FullScreenText.text = "Windowed";
        resText.text = Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString();
        SetMyResolution(infoKeeper.Resolution);
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

        Screen.SetResolution((int)values.x, (int)values.y, isFullScreen);
        PlayerPrefs.SetInt("Resolution", number);
        infoKeeper.Resolution = number;
        infoKeeper.Fullsreen = isFullScreen;
        Camera.main.pixelRect = new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height);
        resText.text = values.x.ToString() + "x" + values.y.ToString();
    }

    public void UpdateVolume()
    {
        InfoKeeper.instance.volume = SoundManager.Instance.volumeSetting;
        PlayerPrefs.SetFloat("Volume", SoundManager.Instance.volumeSetting);
    }
}
