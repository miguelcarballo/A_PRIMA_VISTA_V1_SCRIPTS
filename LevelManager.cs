using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelManager
{
    public static List<LevelGame> listOfLevels;

    public static void initializeAllLevels()
    {
        LevelGame level1 = new LevelGame(1, 0, "Etude 1 Whole Notes");
        LevelGame level2 = new LevelGame(2, 80, "Etude 2 Half Notes");
        LevelGame level3 = new LevelGame(3, 160, "Etude 3 Quarter Notes");
        LevelGame level4 = new LevelGame(4, 250, "Mary Had a Little Lamb");
        LevelGame level5 = new LevelGame(5, 350, "Twinkle Twinkle Little Star");
        LevelGame level6 = new LevelGame(6, 450, "Jingle Bells");

        listOfLevels = new List<LevelGame>();
        listOfLevels.Add(level1);
        listOfLevels.Add(level2);
        listOfLevels.Add(level3);
        listOfLevels.Add(level4);
        listOfLevels.Add(level5);
        listOfLevels.Add(level6);
    }

    public static LevelGame getlevelByName(string name)
    {
        foreach(LevelGame level in listOfLevels)
        {
            if (level.getName().Equals(name))
            {
                return level;
            }
        }
        return null;
    }

    public static List<LevelPlayer> putNewLevelRecordInList(List<LevelPlayer> listLevels, LevelPlayer newLevel)
    {
        bool wasAlreadyInList = false;
        for(int i = 0; i< listLevels.Count; i++)
        {
            if(listLevels[i].getNum() == newLevel.getNum()) //same level is being saved
            {
                listLevels[i] = newLevel;
                wasAlreadyInList = true;
                break;
            }
        }
        if (!wasAlreadyInList)
        {
            listLevels.Add(newLevel);
        }
        /*if (listLevels.Count > 1)
        {
            List<LevelPlayer> SortedList = listLevels.OrderBy(o => o.getNum()).ToList(); //put in order
            listLevels = SortedList;
        }*/
        return listLevels;
    }

    public static int getPointsOfLevelIfExists(List<LevelPlayer> listLevelPlayer, LevelGame levelGame)
    {
        int value = -1;
        foreach(LevelPlayer levelPlayer in listLevelPlayer)
        {
            if(levelPlayer.getNum() == levelGame.getNum())
            {
                value = levelPlayer.getPointsObtained();
                return value;
            }
        }
        return value;
    }

    public static int getTotalPointsOfLevels(List<LevelPlayer> listLevelPlayer)
    {
        int value = 0;
        foreach (LevelPlayer levelPlayer in listLevelPlayer)
        {
            value += levelPlayer.getPointsObtained();
        }
        return value;
    }
}


public class LevelGame { //represents the levels of the game, they never change
    private int num = 0;
    private int requiredPoints = 0;
    private string name = "";

    public LevelGame(int num, int requiredPoints, string name)
    {
        this.num = num;
        this.requiredPoints = requiredPoints;
        this.name = name;
    }

    public int getNum()
    {
        return this.num;
    }

    public string getName()
    {
        return this.name;
    }

    public int getRequiredPoints() {
        return this.requiredPoints;
    }

}

[Serializable]
public class LevelPlayer //represents the data of the level that the player gets each game
{
    public int num = 0;
    public int pointsObtained = 0;
    public string name = "";

    public LevelPlayer(LevelGame levelGame, int pointsObtained)
    {
        this.num = levelGame.getNum();
        this.pointsObtained = pointsObtained;
        this.name = levelGame.getName();
    }

    public int getNum() {
        return this.num;
    }

    public int getPointsObtained()
    {
        return this.pointsObtained;
    }

    public string toString()
    {
        return "LevelP {num="+this.num + ",p="+this.pointsObtained+",name="+this.name+"}";
    }
}


