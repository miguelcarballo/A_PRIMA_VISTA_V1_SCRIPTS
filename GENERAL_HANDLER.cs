using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GENERAL_HANDLER 
{
    private static MenuController menuController;
    private static MIDI_READER_APV midiReader_apv;

    private static bool pieceIsAllowedToPlay = false;
    public static bool loadMidi = false;

    public static bool mainMenu = true;
    public static bool createPlayerMenu = false;
    public static bool playersListMenu = false;
    public static bool levelListMenu = false;
    public static bool explanationMenu = false;
    public static bool resultMenu = false;

    public static string filePlayersList = "test1players.json";
    private static string endOfFileNameLevel = "Test.json"; //for example, Miguel's file will be MIGUELTest.Json
    public static string fileListLevelsPlayer = ""; //this is using the previous attribute

    public static List<string> listPlayers = null;

    public static string playerActive = ""; //the file containing the game has the name + .json
    public static LevelGame pieceLevelActive = null; 

    public static void resetValues()
    {
         pieceIsAllowedToPlay = false;
         loadMidi = false;
    }

    public static void showCreatePlayerMenu()
    {
        mainMenu = false;
        createPlayerMenu = true;
        playersListMenu = false;
        levelListMenu = false;
        explanationMenu = false;
        resultMenu = false;

        menuController.disableMainMenu();
        menuController.enableCreatePlayerMenu();
        menuController.disablePlayersListMenu();
        menuController.disableLevelListMenu();
        menuController.disableExplanationMenu();
        menuController.disableResultMenu();
    }

    public static void showMainMenu()
    {
        mainMenu = true;
        createPlayerMenu = false;
        playersListMenu = false;
        levelListMenu = false;
        explanationMenu = false;
        resultMenu = false;
        menuController.enableMainMenu();
        menuController.disableCreatePlayerMenu();
        menuController.disablePlayersListMenu();
        menuController.disableLevelListMenu();
        menuController.disableExplanationMenu();
        menuController.disableResultMenu();
    }

    public static void showPlayersListMenu()
    {
        mainMenu = false;
        createPlayerMenu = false;
        playersListMenu = true;
        levelListMenu = false;
        explanationMenu = false;
        resultMenu = false;
        listPlayers = FileHandler.ReadListFromJSON<string>(filePlayersList);
        menuController.disableMainMenu();
        menuController.disableCreatePlayerMenu();
        menuController.disableLevelListMenu();
        menuController.disableExplanationMenu();
        menuController.disableResultMenu();

        menuController.enablePlayersListMenu();
        menuController.createButtonPlayer(listPlayers);
        
    }

    public static void createNewPlayer(string newPlayerName)
    {
        listPlayers = FileHandler.ReadListFromJSON<string>(filePlayersList);
        
        if(listPlayers == null)
        {
            listPlayers = new List<string>();
            Debug.Log("READ listPlayers : " + listPlayers.Count);
        }
        listPlayers.Add(newPlayerName);
        FileHandler.SaveToJSON<string>(listPlayers, filePlayersList);
        Debug.Log("WRITE listPlayers : " + listPlayers.Count);
    }

    public static void showLevelListMenu(string namePlayer)
    {

        playerActive = namePlayer;

        mainMenu = false;
        createPlayerMenu = false;
        playersListMenu = false;
        levelListMenu = true;
        explanationMenu = false;
        resultMenu = false;

        menuController.disableMainMenu();
        menuController.disableCreatePlayerMenu();
        menuController.disablePlayersListMenu();       
        menuController.disableExplanationMenu();
        menuController.disableResultMenu();

        //verify if there is any record of previous games
        fileListLevelsPlayer = playerActive + endOfFileNameLevel;
        List<LevelPlayer> listOfGames_player = FileHandler.ReadListFromJSON<LevelPlayer>(fileListLevelsPlayer);
        if (listOfGames_player != null)
        {
            Debug.Log("levelData for Player: " + listOfGames_player.Count);
        }
        else
        {
            Debug.Log("NO DATA FOR PLAYER");
        }
        menuController.enableLevelListMenu(LevelManager.listOfLevels, namePlayer, listOfGames_player); //Add current points!!!
       
    }

    public static void showExplanationMenu(string nameLevel)
    {
        mainMenu = false;
        createPlayerMenu = false;
        playersListMenu = false;
        levelListMenu = false;
        explanationMenu = true;
        resultMenu = false;
        menuController.disableMainMenu();
        menuController.disableCreatePlayerMenu();
        menuController.disablePlayersListMenu();
        menuController.disableLevelListMenu();
        menuController.disableResultMenu();

        pieceLevelActive = LevelManager.getlevelByName(nameLevel);
        midiReader_apv.LoadMIDIFile(nameLevel);
        menuController.enableExplanationMenu(nameLevel);
    }

    //----------

    public static void startPlayingPiece()
    {
        mainMenu = false;
        createPlayerMenu = false;
        playersListMenu = false;
        levelListMenu = false;
        explanationMenu = false;

        menuController.disableMainMenu();
        menuController.disableCreatePlayerMenu();
        menuController.disablePlayersListMenu();
        menuController.disableLevelListMenu();
        menuController.disableExplanationMenu();
        menuController.disableResultMenu();
        pieceIsAllowedToPlay = true;
    }

    public static void deleteAllMusicSymbols()
    {

        midiReader_apv.deleteAllMusicSymbols();
    }



    public static void setMenuController(MenuController menuControllerI)
    {
        menuController = menuControllerI;
                        
    }
    public static void setMidiReader_APV(MIDI_READER_APV midiReader)
    {
        midiReader_apv = midiReader;

    }

    public static bool isPieceAllowedToPlay()
    {
        return pieceIsAllowedToPlay;
    }

    public static void showResultMenu()
    {
        menuController.enableResultMenu(GAMESCORE_HANDLER.getReportOfGame());
    }

    public static void saveTheGameResult()
    {
        fileListLevelsPlayer = playerActive + endOfFileNameLevel;
        List<LevelPlayer> listOfGames_player= FileHandler.ReadListFromJSON<LevelPlayer>(fileListLevelsPlayer);

        if (listOfGames_player == null)
        {
            listOfGames_player = new List<LevelPlayer>();
        }
        Debug.Log("READ " + playerActive + "listGames : " + listOfGames_player.Count);
        Debug.Log("score tbsave = " + GAMESCORE_HANDLER.getReportOfGame().realGameScoreP);
        LevelPlayer levelDataToBeSaved = new LevelPlayer(pieceLevelActive, GAMESCORE_HANDLER.getReportOfGame().realGameScoreP);
        Debug.Log("BEF. Save->" + levelDataToBeSaved.toString());
        listOfGames_player = LevelManager.putNewLevelRecordInList(listOfGames_player, levelDataToBeSaved); //puts in the existent list
        FileHandler.SaveToJSON<LevelPlayer>(listOfGames_player, fileListLevelsPlayer);
        Debug.Log("WRITE levels for "+ playerActive + " : " + listOfGames_player.Count);
        foreach (LevelPlayer level in listOfGames_player)
        {
            Debug.Log("--" + level.toString());
        }
    }

}
