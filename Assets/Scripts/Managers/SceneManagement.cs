using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    //Este script es principalmente accedido por botones
    public static SceneManagement Instance { get; private set; }

    public GameObject gameWonMenu, optionsMenu, pauseMenu, blackscreen;
    private GameObject currentMenu;
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
        if(Input.GetKeyDown(KeyCode.Escape))
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
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        ChangeScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextScene()
    {
        //Aqui va c�digo que cambia la escena segun en la que estes
        //Lo ampliare cuando ya veamos como vamos a organizar las escenas
        gameWonMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void DisplayMenu(GameObject menu)
    {
        currentMenu = menu;
        menu.SetActive(true);
        blackscreen.SetActive(true);
        Time.timeScale = 0;
    }
    public void ExitMenu(GameObject menu)
    {
        currentMenu = null;
        menu.SetActive(false);
        blackscreen.SetActive(false);
        Time.timeScale = 1;
    }
}
