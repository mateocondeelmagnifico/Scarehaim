using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutManager2 : TutorialManager
{
    public bool radarOn, trapDone;
    private bool doOnce;
    [SerializeField] private GameObject arrow;

    public override void Update()
    {
        if (manager.currentState == GameManager.turnState.Endturn) tutorialPlayed = false;
        if(manager.trapTriggered) trapDone = true;
        if (currentTutorial == 2 && trapDone) tutorialPlayed = false;

        if (!tutorialPlayed)
        {
            #region Tutorial Triggers

            switch (currentTutorial)
            {
                case 0:
                    if (cardManager.cardsDealt)
                    {
                        //Move right tutorial
                        StopGame();
                    }
                    break;

                case 1:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //go to trick tutorial
                        StopGame();
                    }
                    break;

                case 2:
                    //Radar tutorial

                    if(mouseManager.radarActive) radarOn = true;

                    if (radarOn && radarDone && !doOnce)
                    {
                        textManager.displayButton = true;
                        Nextmenu();
                        doOnce = true;
                    }
                    break;

                case 5:
                    if (manager.CheckIsInCheckMovement() && doOnce)
                    {
                        //Go to Costume tutorial
                        StopGame();
                        doOnce = false;
                    }
                    break;

                case 7:
                    if (manager.CheckIsInCheckMovement() && manager.playerMove.turnsWithcostume > 0 && !doOnce)
                    {
                        //Go to Costume tutorial
                        Nextmenu();
                        doOnce = true;
                    }
                    break;

                    case 8:
                    arrow.SetActive(true);
                    break;
            }
            #endregion
        }
    }

    public override void Nextmenu()
    {
        //Called by buttons

        currentTutorial++;

        DisplayNextBlackScreen();

        if (currentTutorial == 9)
        {
            //Destroy tutorial manager
            RemoveTutorial();
            nextTutorialButton.SetActive(false);
            mouseManager.canClick = true;
            Destroy(arrow);
            Destroy(this.gameObject);
            return;
        }

        nextTutorialButton.SetActive(false);

        if (currentTutorial != 6 && currentTutorial != 8) mouseManager.canClick = true;
        else textManager.displayButton = true;

        switch (currentTutorial)
        {
            case 1:
                RemoveTutorial();
                break;

            case 5:
                RemoveTutorial();
                break;
        }

        if (textBox.gameObject.activeInHierarchy) textManager.TutorialTalk(tutorialTexts[currentTutorial]);
    }
}
