using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GAMESCORE_HANDLER 
{
    private static ScoreStructureManager scoreStructureManager = null;
    private static ScoreGraphicManager scoreGraphicManager = null;
    private static List<MusicNote> listAllMusicNotes = null;
    private static GameScoring gameScoring = null;
    private static MusicNote currentNote = null;
    private static MusicNote futureNote = null;
    private static float currentTime = 0.0f;

    private static float goodRatio = 1.0f / 6.0f;
    private static float acceptableRatio = 1.0f / 4.0f;
    private static float goodValue = 0.0f;
    private static float acceptableValue = 0.0f;

    private static float goodRatioRelease = 1.0f / 2.5f; //0.4 beat tolerance, just 0.6 required
    private static float goodValueRelease = 0.0f;
   
    private static int indexEvaluatorHit = 0;
    private static MusicNote noteBeingHold1 = null;
    private static MusicNote noteBeingHold2 = null;

    private static int idealGameScore = 0;
    private static int noRestQuantityNotes = 0;

    public static void setManagers(ScoreStructureManager scoreStructureManagerIn, ScoreGraphicManager scoreGraphicManagerIn)
    {
        indexEvaluatorHit = 0;
        currentTime = 0.0f;
        scoreStructureManager = scoreStructureManagerIn;
        scoreGraphicManager = scoreGraphicManagerIn;
        gameScoring = new GameScoring();
        listAllMusicNotes = scoreStructureManager.getListAllNotes();
        if(listAllMusicNotes.Count > 1)
        {
            currentNote = listAllMusicNotes[0];
            futureNote = listAllMusicNotes[1];
        }
        idealGameScore = calculateIdealGameScore(listAllMusicNotes);
        noRestQuantityNotes = calculateQuantityNoRestNotes(listAllMusicNotes);
        updateEvaluationValues();
    }

    public static void updateEvaluationValues()
    {
        goodValue = CurrentBeatValuesTable.getBeatValueSeconds() * goodRatio;
        acceptableValue = CurrentBeatValuesTable.getBeatValueSeconds() * acceptableRatio;
        goodValueRelease = CurrentBeatValuesTable.getBeatValueSeconds() * goodRatioRelease;
    }

    public static void setCurrentPieceTime(float currentPieceTime)
    {
        currentTime = currentPieceTime;
        checkAndUpdateCurrentNote();
    }

    private static void moveIndexEvaluator()
    {
        indexEvaluatorHit++;
        //Debug.Log("Moving INDEX = " + indexEvaluator);
        if (indexEvaluatorHit < listAllMusicNotes.Count)
        {
            currentNote = listAllMusicNotes[indexEvaluatorHit];
        }
        else
        {
            currentNote = null;
        }
        if(indexEvaluatorHit + 1 < listAllMusicNotes.Count)
        {
            futureNote = listAllMusicNotes[indexEvaluatorHit + 1];
        }
        else
        {
            futureNote = null;
        }
    }

    private static void checkAndUpdateCurrentNote()
    {
        if (currentNote != null)
        {
            float noteStartT = currentNote.getNoteStartsOnSecond();
            //if NOT in acceptable range
            if (currentTime > noteStartT + acceptableValue)
            {
                //update color MISS (mistake)
                // Debug.Log("Ct = " + currentTime + "   Nstart = " + noteStartT);
                string txtNoteSkipped = gameScoring.addNoteSkipped();
                scoreGraphicManager.setCommentAndColors(txtNoteSkipped);
                moveIndexEvaluator();
                if (currentNote != null)
                {
                    Debug.Log("should play => " + currentNote.getPitchName());
                }   
            }
        }
    }

    public static int getTotalScore()
    {
        return gameScoring.getTotalLevelScore();
    }

    public static int getQuantityEarlyReleasePenalty()
    {
        return gameScoring.getQuantityEarlyRelease();
    }

    public static int getQuantityLateReleasePenalty()
    {
        return gameScoring.getQuantityLateRelease();
    }

    public static void evaluateKeyHit(string nameNotePitch)
    {
        if (currentNote != null)
        {
            Debug.Log("HITnameNote = " + nameNotePitch + "  currPitch= " + currentNote.getPitchName() +" currTime=" + currentTime + " GV " + goodValue + " currTN");   
            if (nameNotePitch.Equals(currentNote.getPitchName()))//is playing right note
            {
                float noteStartT = currentNote.getNoteStartsOnSecond();
                if (currentTime >= noteStartT - goodValue && currentTime <= noteStartT + goodValue) //good range
                {
                    //update color good and add points to score
                    string textGoodHit = gameScoring.addGoodHit();
                    scoreGraphicManager.setCommentAndColors(textGoodHit);
                    addNoteBeingHold(currentNote);
                    moveIndexEvaluator();
                }
                else if (currentTime >= noteStartT - acceptableValue && currentTime <= noteStartT - goodValue) //acceptable range
                {

                    //update color acceptable early Hit and add points to score
                    string textEarlyHit = gameScoring.addEarlyHit();
                    scoreGraphicManager.setCommentAndColors(textEarlyHit);
                    addNoteBeingHold(currentNote);
                    moveIndexEvaluator();
                }else if (currentTime <= noteStartT + acceptableValue && currentTime >= noteStartT + goodValue)
                {
                    //update color acceptable late Hit and add points to score
                    string textLateHit = gameScoring.addLateHit();
                    scoreGraphicManager.setCommentAndColors(textLateHit);
                    addNoteBeingHold(currentNote);
                    moveIndexEvaluator();
                }
                else //play right note in wrong moment
                {
                    //update color mistake
                    string textNotOnTempo = gameScoring.addNotOnTempo();
                    scoreGraphicManager.setCommentAndColors(textNotOnTempo);
                }
            }
            else //not right note, verify the next future note. 
            {
                if (futureNote != null)
                {
                    if (nameNotePitch.Equals(futureNote.getPitchName()))
                    {
                        float noteStartT = futureNote.getNoteStartsOnSecond();
                        if (currentTime >= noteStartT - goodValue && currentTime <= noteStartT + goodValue) //good range
                        {

                            //update SKIP current note (user played the future note but not the current note)
                            //mistake
                            gameScoring.addNoteSkipped();
                            addNoteBeingHold(futureNote);
                            moveIndexEvaluator();
                            //update color good and add points to score
                            string textGoodHit = gameScoring.addGoodHit();
                            scoreGraphicManager.setCommentAndColors(textGoodHit);
                            addNoteBeingHold(futureNote);
                            moveIndexEvaluator();
                        }
                        else if (currentTime >= noteStartT - acceptableValue && currentTime <= noteStartT - goodValue) //acceptable range early
                        {
                            //update SKIP current note (user played the future note but not the current note)
                            //mistake
                            gameScoring.addNoteSkipped();
                            addNoteBeingHold(futureNote);
                            moveIndexEvaluator();
                            //update color acceptable early Hit and add points to score
                            string textEarlyHit = gameScoring.addEarlyHit();
                            scoreGraphicManager.setCommentAndColors(textEarlyHit);

                            moveIndexEvaluator();
                        }
                        else if (currentTime <= noteStartT + acceptableValue && currentTime >= noteStartT + goodValue) //acceptable range late
                        {
                            //update SKIP current note (user played the future note but not the current note)
                            //mistake
                            gameScoring.addNoteSkipped();
                            addNoteBeingHold(futureNote);
                            moveIndexEvaluator();
                            //update color acceptable late Hit and add points to score
                            string textLateHit = gameScoring.addLateHit();
                            scoreGraphicManager.setCommentAndColors(textLateHit);
                            moveIndexEvaluator();
                        }
                    }
                    else //no right note at all, obvious mistake
                    {
                        //mistake
                        string txtWrong = gameScoring.addWrongNote();
                        scoreGraphicManager.setCommentAndColors(txtWrong);
                    }
                }
                else //piece finished and user is still playing
                {
                    //neutral

                }
            }
        }
    }

    private static void addNoteBeingHold(MusicNote musicNote)
    {
        if(noteBeingHold1 == null)
        {
            noteBeingHold1 = musicNote;
        }
        else
        {
            noteBeingHold2 = musicNote;
        }
    }

    private static MusicNote getNoteIfBeingHold(string nameNotePitch)
    {
        if (noteBeingHold1 != null)
        {
            if (noteBeingHold1.getPitchName().Equals(nameNotePitch)){
                return noteBeingHold1;
            }
        }
        if(noteBeingHold2 != null)
        {
            if (noteBeingHold2.getPitchName().Equals(nameNotePitch))
            {
                return noteBeingHold2;
            }
        }
        return null;
    }

    private static void releaseNoteBeingHold(string nameNotePitch)
    {
        if (noteBeingHold1 != null)
        {
            if (noteBeingHold1.getPitchName().Equals(nameNotePitch))
            {
                noteBeingHold1 = noteBeingHold2; //move 2 to 1
                noteBeingHold2 = null;
            }
        }
        if (noteBeingHold2 != null)
        {
            if (noteBeingHold2.getPitchName().Equals(nameNotePitch))
            {
                noteBeingHold2 = null;
            }
        }
    }

    public static void evaluateKeyReleased(string nameNotePitch)
    {
        MusicNote noteBeingHold = getNoteIfBeingHold(nameNotePitch);
        if (noteBeingHold != null)
        {
            float noteEndT = noteBeingHold.getNoteFinishOnSecond();
            //see if the release is good
            if (currentTime >= noteEndT - goodValueRelease && currentTime <= noteEndT + goodValueRelease)
            {
                //if it is good, no penalty applied
                string txtGoodRelease = gameScoring.addGoodRelease();
                //scoreGraphicManager.setCommentAndColors(txtGoodRelease);
                releaseNoteBeingHold(nameNotePitch);

            }else if (currentTime < noteEndT - goodValueRelease)
            {
                //it released key too early, penalty applied
                string txtEarlyRelease = gameScoring.addEarlyRelease();
                //scoreGraphicManager.setCommentAndColors(txtEarlyRelease);
                scoreGraphicManager.updatePenaltyScore();
                releaseNoteBeingHold(nameNotePitch);

            }else if (currentTime > noteEndT + goodValueRelease)
            {
                //it released key too late, penalty applied
                string txtLateRelease = gameScoring.addLateRelease();
                //scoreGraphicManager.setCommentAndColors(txtLateRelease);
                scoreGraphicManager.updatePenaltyScore();
                releaseNoteBeingHold(nameNotePitch);
            }
        }
    }

    private static int calculateIdealGameScore(List<MusicNote> allMusicNotes)
    {
        int idealScore = 0;
        foreach(MusicNote note in allMusicNotes)
        {
            if (!note.isRest())
            {
                idealScore = idealScore + GameScoring.getGoodHitNoPenalties();

            }
        }
        return idealScore;
    }

    private static int calculateQuantityNoRestNotes(List<MusicNote> allMusicNotes)
    {
        int qNotes = 0;
        foreach (MusicNote note in allMusicNotes)
        {
            if (!note.isRest())
            {
                qNotes++;
            }
        }
        return qNotes;
    }

    public static ReportGame getReportOfGame()
    {
        //gameScoring.printData();
        //Debug.Log("GAMESCORE ideal = " + idealGameScore + " nRestQ="+noRestQuantityNotes);
        ReportGame newReport = new ReportGame(idealGameScore, noRestQuantityNotes, gameScoring);
        return newReport;
    }
}

public class ReportGame
{
    private int idealGameScore;
    private int realGameScore;

    public int realGameScoreP;
    public int goodHitP;
    public int earlyHitP;
    public int lateHitP;
    public int notOnTempoP;
    public int noteSkippedP;
    public int wrongNoteP;
    public int goodReleaseP;
    public int earlyReleaseP;
    public int lateReleaseP;

    public ReportGame(int idealGameScore, int totalNotes, GameScoring gameScoring)
    {
        this.idealGameScore = idealGameScore;
        this.realGameScore = gameScoring.getTotalLevelScore();
        this.realGameScoreP = gameScoring.getTotalLevelScore() * 100 / idealGameScore;
        this.goodHitP = gameScoring.getGoodHitQuantity()*100/ totalNotes ;
        this.earlyHitP = gameScoring.getEarlyHitQuantity()*100 / totalNotes;
        this.lateHitP = gameScoring.getLateHitQuantity() *100 / totalNotes;
        this.notOnTempoP = gameScoring.getNotOnTempoQuantity() * 100/ totalNotes;
        this.noteSkippedP = gameScoring.getNoteSkippedQuantity() *100/ totalNotes;
        this.wrongNoteP = gameScoring.getWrongNoteQuantity()*100/ totalNotes;
        this.goodReleaseP = gameScoring.getGoodReleaseQuantity()*100 / totalNotes;
        this.earlyReleaseP = gameScoring.getEarlyReleaseQuantity()*100 / totalNotes;
        this.lateReleaseP = gameScoring.getLateReleaseQuantity()*100/ totalNotes;
    }

}

