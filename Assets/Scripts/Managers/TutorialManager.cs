using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private bool gamepaused;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (gamepaused)
            {
                Nextmenu();
            }
        }
    }

    private void StopGame()
    {
        Time.timeScale = 0;
        gamepaused = true;
    }

    private void Nextmenu()
    {

    }
}
