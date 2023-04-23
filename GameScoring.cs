using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScoring 
{
    private int totalLevelScore = 0;
    private static int goodHitScore = 10;
    private static int earlyHitScore = 8;
    private static int lateHitScore = 8;
    private static int earlyReleaseScore = -1;
    private static int lateReleaseScore = -1;

    private int goodHitQuantity = 0;
    private int earlyHitQuantity = 0;
    private int lateHitQuantity = 0;
    private int notOnTempoQuantity = 0;
    private int noteSkippedQuantity = 0;
    private int wrongNoteQuantity = 0;
    private int goodReleaseQuantity = 0;
    private int earlyReleaseQuantity = 0;
    private int lateReleaseQuantity = 0;

    public static string goodHitMsg = "Good!";
    public static string earlyHitMsg = "Early";
    public static string lateHitMsg = "Late";

    public static string notOnTempoMsg = "Not on tempo";
    public static string noteSkippedMsg = "Note skipped";
    public static string wrongNoteMsg = "Wrong note!";

    public static string goodReleaseMsg = "Good Release";
    public static string earlyReleaseMsg = "Key released early!";
    public static string lateReleaseMsg = "Key released late!";

    public GameScoring()
    {

    }

    public static int getGoodHitNoPenalties()
    {
        return goodHitScore;
    }
    
    public int getTotalLevelScore()
    {
        return totalLevelScore;
    }

    public int getQuantityEarlyRelease()
    {
        return earlyReleaseQuantity;
    }

    public int getQuantityLateRelease()
    {
        return lateReleaseQuantity;
    }


    public string addGoodHit()
    {
        totalLevelScore += goodHitScore;
        goodHitQuantity++;
        return goodHitMsg;
    }

    public string addEarlyHit()
    {
        totalLevelScore += earlyHitScore;
        earlyHitQuantity++;
        return earlyHitMsg;
    }

    public string addLateHit()
    {
        totalLevelScore += lateHitScore;
        lateHitQuantity++;
        return lateHitMsg;
    }
    public string addNotOnTempo()
    {
        notOnTempoQuantity++;
        return notOnTempoMsg;
    }

    public string addNoteSkipped()
    {
        noteSkippedQuantity++;
        return noteSkippedMsg;
    }

    public string addWrongNote()
    {
        wrongNoteQuantity++;
        return wrongNoteMsg;
    }
    public string addGoodRelease()
    {
        goodReleaseQuantity++; //no penalty
        return goodReleaseMsg;
    }

    public string addEarlyRelease()
    {
        earlyReleaseQuantity++;
        totalLevelScore = totalLevelScore + earlyReleaseScore;
        return earlyReleaseMsg;
    }

    public string addLateRelease()
    {
        lateReleaseQuantity++;
        totalLevelScore = totalLevelScore + lateReleaseScore;
        return lateReleaseMsg;
    }
   
    public static int getEarlyHitScoreVal()
    {
        return goodHitScore;
    }
    public static int getGoodHitScoreVal()
    {
        return earlyHitScore;
    }
    public static int getLateHitScoreVal()
    {
        return lateHitScore;
    }

    //to fill the report----------
    public int getGoodHitQuantity()
    {
        return this.goodHitQuantity;
    }

    public int getEarlyHitQuantity()
    {
        return this.earlyHitQuantity;
    }

    public int getLateHitQuantity()
    {
        return this.lateHitQuantity;
    }

    public int getNotOnTempoQuantity()
    {
        return this.notOnTempoQuantity;
    }

    public int getNoteSkippedQuantity()
    {
        return this.noteSkippedQuantity;
    }

    public int getWrongNoteQuantity()
    {
        return this.wrongNoteQuantity;
    }

    public int getGoodReleaseQuantity()
    {
        return this.goodReleaseQuantity;
    }

    public int getEarlyReleaseQuantity()
    {
        return this.earlyReleaseQuantity;
    }

    public int getLateReleaseQuantity()
    {
        return this.lateReleaseQuantity;
    }

    public void printData()
    {
        Debug.Log("SCORING data G.E.L.= (" + this.goodHitQuantity + ", " + this.earlyHitQuantity + ", " + this.lateHitQuantity + ")");
    }
}



