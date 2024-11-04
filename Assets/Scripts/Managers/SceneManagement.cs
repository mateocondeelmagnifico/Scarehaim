using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagement : MonoBehaviour
{
    //Este script es principalmente accedido por botones
    public static SceneManagement Instance { get; private set; }
    private AsyncOperation operation;
    [SerializeField] private GameObject skipTutorialMenu;

    public GameObject gameWonMenu, optionsMenu, pauseMenu, blackscreen, loadingIcon, loadingMenu;
    private GameObject currentMenu;

    public bool canPause;
    private bool wasActive;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            if(currentMenu == null)
            {
                DisplayMenu(pauseMenu);
            }
            else
            {
                ExitMenu(currentMenu);
            }
        }
    }
    public void ChangeScene(int whatScene)
    {
        SceneManager.LoadScene(whatScene);
    }

    public void SaveProgress(int stage)
    {
        PlayerPrefs.SetInt("CurrentStage", stage);
    }

    public void ReloadScene()
    {
        StartLoad(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextScene()
    {
        //Aqui va código que cambia la escena segun en la que estes
        //Lo ampliare cuando ya veamos como vamos a organizar las escenas
        gameWonMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void DisplayMenu(GameObject menu)
    {
        if (blackscreen.activeInHierarchy && currentMenu == null) wasActive = true;
        else blackscreen.SetActive(true);

        if (currentMenu != null) currentMenu.SetActive(false);
        currentMenu = menu;
        menu.SetActive(true);
        
        
        Time.timeScale = 0;
    }
    public void ExitMenu(GameObject menu)
    {
        currentMenu = null;
        menu.SetActive(false);

        if(wasActive) wasActive = false; 
        else blackscreen.SetActive(false);

        Time.timeScale = 1;
    }
    public void CallLoadScene()
    {
        //Called by buttons, start game where you left off
        //Only used in main menu
        int myStage = 1;
        if (PlayerPrefs.HasKey("CurrentStage")) myStage = PlayerPrefs.GetInt("CurrentStage");
        if(myStage == 1) StartLoad(myStage);
        else skipTutorialMenu.SetActive(true);
    }

    public void StartLoad(int stage)
    {
        loadingMenu.SetActive(true);
        StartCoroutine(LoadSceneAsync(stage));
    }
    IEnumerator LoadSceneAsync(int sceneId)
    {
        operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {

            loadingIcon.GetComponent<Image>().fillAmount = operation.progress / 0.9f;

            if (operation.progress >= 0.8f) operation.allowSceneActivation = true;

            yield return null;
        }
    }
}
