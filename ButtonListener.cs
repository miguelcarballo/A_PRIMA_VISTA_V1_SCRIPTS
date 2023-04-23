using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonListener : MonoBehaviour
{
   public void clickMainCreatePlayerBtn()
    {
        GENERAL_HANDLER.showCreatePlayerMenu();
    }

    public void clickBackToMenu()
    {
        GENERAL_HANDLER.showMainMenu();
    }

    public void clickGoToPlayersList()
    {
        GENERAL_HANDLER.showPlayersListMenu();
    }

    public void clickCreateNewPlayer(GameObject txtNamePlayer)
    {
        string nameNewPlayer = txtNamePlayer.GetComponent<TMPro.TextMeshProUGUI>().text;
        if(nameNewPlayer.Length > 0)
        {
            GENERAL_HANDLER.createNewPlayer(nameNewPlayer);
            GENERAL_HANDLER.showLevelListMenu(nameNewPlayer); //go to list menu
            GENERAL_HANDLER.playerActive = nameNewPlayer;
        }
    }

    public void clickGoToLevelListMenu(GameObject btnNamePlayer)
    {
        string namePlayer = btnNamePlayer.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
        if (namePlayer.Length > 0)
        {
            GENERAL_HANDLER.showLevelListMenu(namePlayer);
            GENERAL_HANDLER.playerActive = namePlayer;
        }
    }

    public void clickGoToExplanationMenu(GameObject btnLevel)
    {
        GameObject txtChildShown = btnLevel.transform.Find("txtShown").gameObject;
        GameObject txtChildHidden = btnLevel.transform.Find("txtHidden").gameObject;
        //btnLevel.transform.Find("ChildGameObject").gameObject;
        //string nameLevel = btnLevel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
        string nameLevel = txtChildHidden.GetComponent<TMPro.TextMeshProUGUI>().text;
        if (nameLevel.Length > 0)
        {
            GENERAL_HANDLER.showExplanationMenu(nameLevel);
            GENERAL_HANDLER.pieceLevelActive = LevelManager.getlevelByName(nameLevel);
        }
    }

    public void clickBackToLevelListMenu()
    {
        GENERAL_HANDLER.deleteAllMusicSymbols();
        GENERAL_HANDLER.showLevelListMenu(GENERAL_HANDLER.playerActive);
    }

    public void clickStartPlayingThePiece()
    {
        GENERAL_HANDLER.startPlayingPiece();
    }

    public void clickSaveGameScore()
    {
        //save first!!!
        GENERAL_HANDLER.saveTheGameResult();
        clickBackToLevelListMenu();
    }

    public void clickDontSaveGameScore()
    {
        clickBackToLevelListMenu();
    }

    public void EXIT()
    {
        Application.Quit();
    }
}
