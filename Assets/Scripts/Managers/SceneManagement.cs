using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagement : MonoBehaviour
{
    //Este script es principalmente accedido por botones
    public static SceneManagement Instance { get; private set; }
    private AsyncOperation operation;
    [SerializeField] private GameObject skipTutorialMenu;
    [SerializeField] private TextMeshProUGUI finalTips;

    public GameObject gameWonMenu, optionsMenu, pauseMenu, blackscreen, loadingIcon, loadingMenu, currentMenu;
    private GameObject oldMenu;

    public bool canPause;
    private bool wasActive, activate;

    private float opacity;

    [TextArea]
    [SerializeField] private string[] tips;
    
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
        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
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

        //Check all children of a menu and increase their opacity
        if (activate && currentMenu != null)
        {
            ReduceMenuOpacity();

            opacity += 0.022f;

            if (opacity >= 1)
            {
                opacity = 0;
                activate = false;
            }
        }
    }

    private void ReduceMenuOpacity()
    {
        for (int i = 0; i < currentMenu.transform.childCount; i++)
        {
            Transform relevantObj = currentMenu.transform.GetChild(i);

            ChangeOpacity(relevantObj);

            if (relevantObj.childCount > 0)
            {
                for (int e = 0; e < relevantObj.transform.childCount; e++)
                {
                    Transform relevantObj2 = relevantObj.transform.GetChild(e);

                    ChangeOpacity(relevantObj2);

                    if (relevantObj2.childCount > 0)
                    {
                        for (int r = 0; r < relevantObj2.transform.childCount; r++)
                        {
                            ChangeOpacity(relevantObj2.transform.GetChild(r));
                        }
                    }
                }
            }
        }
    }
    private void ChangeOpacity(Transform myObject)
    {
        Color relevantColor = Color.white;

        if (myObject.GetComponent<Image>())
        {
            relevantColor = myObject.GetComponent<Image>().color;
            myObject.GetComponent<Image>().color = new Color(relevantColor.r, relevantColor.g, relevantColor.b, opacity);
        }

        if (myObject.GetComponent<TextMeshProUGUI>())
        {
            relevantColor = myObject.GetComponent<TextMeshProUGUI>().color;
            myObject.GetComponent<TextMeshProUGUI>().color = new Color(relevantColor.r, relevantColor.g, relevantColor.b, opacity);
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
        oldMenu = currentMenu;
        currentMenu = menu;
        ReduceMenuOpacity();
        menu.SetActive(true);
        activate = true;

        Time.timeScale = 0;
    }
    public void DisplayDeathMenu(GameObject menu)
    {
        finalTips.text = tips[Random.Range(0, tips.Length)];
        DisplayMenu(menu);
    }
    public void ReturnToMenu()
    {
        //Go back to the previous menu, activated by buttons
        if (oldMenu == null) return;
        DisplayMenu(oldMenu);
    }
    public void ExitMenu(GameObject menu)
    {
        #region Delete arrows from exit menu
        //If you hover a button and press esc in this menu the arrows are not deactivated normally
        //so this finds the arrows code deactivates them
        for (int i = 0; i < currentMenu.transform.childCount; i++)
        {
            if(currentMenu.transform.GetChild(i).childCount > 0 && currentMenu.transform.GetChild(i).GetComponent<Button>())
            {
                Transform myButton = currentMenu.transform.GetChild(i);
                for (int e = 0; e < myButton.childCount; e++)
                {
                     if(myButton.GetChild(e).GetComponent<Image>()) myButton.GetChild(e).gameObject.SetActive(false);
                }
            }
        }
        
        #endregion

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
