using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject createPlayerMenu;
    public GameObject playersListMenu;
    public GameObject levelListMenu;
    public GameObject explanationMenu;
    public GameObject resultGameMenu;

    public GameObject gridWithElements;
    public GameObject btnModelPlayer;
    private List<GameObject> listButtonPlayer;

    public GameObject gridWithElementsLevel;
    public GameObject btnModelLevel;
    public GameObject txtHelloPlayer;
    public GameObject txtTotalPointsPlayer;

    private List<GameObject> listButtonLevel;

    public GameObject imageExplanationObject;

    public GameObject txtGood;
    public GameObject txtEarly;
    public GameObject txtLate;
    public GameObject txtWrong;
    public GameObject txtTOTAL;

    
    // Start is called before the first frame update
    void Start()
    {
        GENERAL_HANDLER.setMenuController(this);
        listButtonPlayer = new List<GameObject>();
        listButtonLevel= new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void disableMainMenu()
    {
        mainMenu.SetActive(false);
    }

    public void enableMainMenu()
    {
        mainMenu.SetActive(true);
    }

    //--------

    public void enableCreatePlayerMenu()
    {
        createPlayerMenu.SetActive(true);
    }

    public void disableCreatePlayerMenu()
    {
        createPlayerMenu.SetActive(false);
    }

    //--------

    public void disablePlayersListMenu()
    {
        playersListMenu.SetActive(false);
        if (listButtonPlayer.Count > 0)
        {
            foreach (GameObject btn in listButtonPlayer)
            {
                Destroy(btn);
            }
            listButtonPlayer = new List<GameObject>();
        }
    }

    public void enablePlayersListMenu()
    {
        playersListMenu.SetActive(true);
    }

    public void createButtonPlayer(List<string> listNamePlayers)
    {
        foreach (string name in listNamePlayers)
        {
            Debug.Log("player: " + name);
            GameObject playerBtn = GameObject.Instantiate(btnModelPlayer);
            playerBtn.SetActive(true);
            playerBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;
            playerBtn.transform.SetParent(gridWithElements.transform, false);
            //playerBtn.transform.parent = gridWithElements.transform;
            listButtonPlayer.Add(playerBtn);
        }

    }
    //-------

    public void enableLevelListMenu(List<LevelGame> listLevelsGame, string namePlayer, List<LevelPlayer> listOfGames_player)
    {
        levelListMenu.SetActive(true);
        Debug.Log("CREATING LEVELS FOR : " + namePlayer);
        txtHelloPlayer.GetComponent<TMPro.TextMeshProUGUI>().text = "Welcome " + namePlayer + "!";
        //---
        if(listOfGames_player == null)
        {
            listOfGames_player = new List<LevelPlayer>();
        }
        foreach(LevelPlayer level in listOfGames_player)
        {
            Debug.Log("--" + level.toString());
        }
        int totalPointsPlayer = LevelManager.getTotalPointsOfLevels(listOfGames_player);
        txtTotalPointsPlayer.GetComponent<TMPro.TextMeshProUGUI>().text = "TOTAL POINTS = "+ totalPointsPlayer;
        foreach (LevelGame level in listLevelsGame)
        {
            string message = "";
            bool isPlayable = true;

            int pointsRecorded = LevelManager.getPointsOfLevelIfExists(listOfGames_player, level); //return -1 if it does not exist any record
            if(pointsRecorded < 0) //no register of this level
            {
                if(totalPointsPlayer >= level.getRequiredPoints()) //player can play the level, it has not in the past
                {
                    message = "?/100";
                }
                else //player cannot play the level yet, not enough points
                {
                    message = "Min."+level.getRequiredPoints().ToString();
                    isPlayable = false;
                }
            }
            else //there is register of this level
            {
                message = pointsRecorded.ToString()+"/100";
            }
            GameObject levelBtn = GameObject.Instantiate(btnModelLevel);
            levelBtn.SetActive(true);
            //levelBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = level.getName() + "  " + message;
            levelBtn.transform.Find("txtShown").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = level.getName() + "  " + message;
            levelBtn.transform.Find("txtHidden").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = level.getName();
            levelBtn.transform.SetParent(gridWithElementsLevel.transform, false);
            levelBtn.GetComponent<Button>().interactable = isPlayable;
            listButtonLevel.Add(levelBtn);
        }    
    }

    public void disableLevelListMenu()
    {
        if (listButtonLevel.Count > 0)
        {
            foreach (GameObject btn in listButtonLevel)
            {
                Destroy(btn);
            }
            listButtonLevel = new List<GameObject>();
        }
        levelListMenu.SetActive(false);
    }

  

    //--------
    public void enableExplanationMenu(string levelName)
    {
        //levelName has to match with the name of the image
        explanationMenu.SetActive(true);
        Sprite sprite = Resources.Load<Sprite>("Images/"+levelName);
        if(sprite != null)
        {
            imageExplanationObject.GetComponent<Image>().sprite = sprite;
        }    
    }

    public void disableExplanationMenu()
    {
        explanationMenu.SetActive(false);
    }

    //------

    public void enableResultMenu(ReportGame reportG)
    {
        txtGood.GetComponent<TMPro.TextMeshProUGUI>().text = "Good Hit: " +reportG.goodHitP
            +"%  Good Release: "+ reportG.goodReleaseP + "%";
        txtEarly.GetComponent<TMPro.TextMeshProUGUI>().text = "Early Hit: " + reportG.earlyHitP
            + "%  Early Release: " + reportG.earlyReleaseP + "%";
        txtLate.GetComponent<TMPro.TextMeshProUGUI>().text = "Late Hit: " + reportG.lateHitP
            + "%  Late Release: " + reportG.lateReleaseP + "%";
        txtWrong.GetComponent<TMPro.TextMeshProUGUI>().text = "Out of beat: :  " + reportG.notOnTempoP
            + "%  Wrong Notes: " + reportG.wrongNoteP + "%      Notes Skipped: " + reportG.noteSkippedP + "%";
        txtTOTAL.GetComponent<TMPro.TextMeshProUGUI>().text = "EARNED POINTS: " + reportG.realGameScoreP + "/100";

        resultGameMenu.SetActive(true);
    }

    public void disableResultMenu()
    {
        resultGameMenu.SetActive(false);
    }
}
