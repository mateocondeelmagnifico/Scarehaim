using UnityEngine;

public class TutManager2 : TutorialManager
{
    public bool radarOn, trapDone;
    private bool doOnce;

    public override void Start()
    {
        base.Start();

        //Give player more hope so that the tutorial doesn't kill him
        Fear playerFear = manager.player.GetComponent<Fear>();
        if (playerFear.hope < 5) playerFear.UpdateFear(5 - playerFear.hope);
    }

    public override void Update()
    {
        if (manager.currentState == GameManager.turnState.Endturn) tutorialPlayed = false;
        if(manager.trapTriggered) trapDone = true;
        if (currentTutorial == 6 && trapDone) tutorialPlayed = false;
        if (currentTutorial == 12)
        {
            if(manager.powerUpOn || mouseManager.cardGrabbed) screenImage.enabled = false;
            else screenImage.enabled = true;
        }

        if (!tutorialPlayed)
        {
            #region Tutorial Triggers

            switch (currentTutorial)
            {
                case 0:
                    if (cardManager.cardsDealt)
                    {
                        //Move right tutorial
                        DisplayTutorial();
                        textManager.displayButton = true;
                    }
                    break;

                case 3:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //go to trick tutorial
                        DisplayTutorial();
                    }
                    break;

                case 6:
                    //Radar tutorial

                    if(mouseManager.radarActive) radarOn = true;

                    if (radarOn && radarDone && !doOnce)
                    {
                        textManager.displayButton = true;
                        Nextmenu();
                        doOnce = true;
                    }
                    break;

                case 11:
                    if (manager.CheckIsInCheckMovement() && doOnce)
                    {
                        //Go to Costume tutorial
                        DisplayTutorial();
                        doOnce = false;
                    }
                    break;

                case 12:
                    if (manager.CheckIsInCheckMovement() && manager.playerMove.turnsWithcostume > 0 && !doOnce)
                    {
                        //Go to Costume tutorial
                        Nextmenu();
                        doOnce = true;
                    }
                    break;

                    case 20:
                    screenImage.enabled = true;
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


        if (currentTutorial == 21)
        {
            //Destroy tutorial manager
            RemoveTutorial();
            nextTutorialButton.SetActive(false);
            mouseManager.canClick = true;
            Destroy(this.gameObject);
            return;
        }

        nextTutorialButton.SetActive(false);      

        if (currentTutorial != 2 && currentTutorial != 3 && currentTutorial != 6 && currentTutorial != 10 && currentTutorial != 12 && currentTutorial != 16 && currentTutorial != 17)
        {
            mouseManager.canClick = false;           
            textManager.displayButton = true;
        }
        else
        {
            mouseManager.canClick = true;
        }

        mouseManager.display.enabled = false;

        switch (currentTutorial)
        {
            case 2:
                hand.ActivateColliders(false);
                break;

            case 3:
                hand.ActivateColliders(true);
                RemoveTutorial();
                break;

            case 4:
                hand.ActivateColliders(false);
                break;

            case 12:
                hand.ActivateColliders(true);
                mouseManager.dontDisplay = true;
                break;

            case 13:
                mouseManager.dontDisplay = false;
                break;
        }

        if (textBox.gameObject.activeInHierarchy) textManager.TutorialTalk(tutorialTexts[currentTutorial]);
    }

    public void OptionsPressed()
    {
        if (currentTutorial == 19) Nextmenu();
    }
}
