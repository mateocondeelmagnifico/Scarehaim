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
        if (currentTutorial == 2 && trapDone) tutorialPlayed = false;
        if (currentTutorial == 7)
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
                    }
                    break;

                case 1:
                    if (manager.CheckIsInCheckMovement())
                    {
                        //go to trick tutorial
                        DisplayTutorial();
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
                        DisplayTutorial();
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

        if (currentTutorial == 9)
        {
            //Destroy tutorial manager
            RemoveTutorial();
            nextTutorialButton.SetActive(false);
            mouseManager.canClick = true;
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

    public void OptionsPressed()
    {
        if (currentTutorial == 8) Nextmenu();
    }
}
