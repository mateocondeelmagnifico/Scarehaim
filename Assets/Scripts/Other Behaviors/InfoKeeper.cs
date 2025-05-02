using UnityEngine;
using UnityEngine.SceneManagement;

public class InfoKeeper : MonoBehaviour
{
    //Esto mantiene el valor del volumen cuando la mano se destruye

    public static InfoKeeper instance { get; private set;}

    public float volume = 1;
    public int resolution;
    public int Lenguaje = 1;
    public bool Fullsreen = true;
    public bool hasRes;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        //OptionsManager.instance.LoadValues(this);
        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void SceneChanged(Scene current, Scene next)
    {
        //OptionsManager.instance.LoadValues(this);
    }
}
