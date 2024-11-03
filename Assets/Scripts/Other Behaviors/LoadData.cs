using UnityEngine;

public class LoadData : MonoBehaviour
{
    //This script sets the resolution, fullscreen and volume to what the player has put them in previous games
    //If he has player before
    [SerializeField] private OptionsManager optionsManager;
    [SerializeField] private InfoKeeper infoKeeper;

    private void Awake()
    {
       if(PlayerPrefs.HasKey("Resolution")) infoKeeper.Resolution = PlayerPrefs.GetInt("Resolution");
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            if (PlayerPrefs.GetInt("Fullscreen") == 0) infoKeeper.Fullsreen = false;
            else infoKeeper.Fullsreen = true;
        }
        if (PlayerPrefs.HasKey("Volume")) infoKeeper.volume = PlayerPrefs.GetFloat("Volume");
       Destroy(gameObject);
    }
}
