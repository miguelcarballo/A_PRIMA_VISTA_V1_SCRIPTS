using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmValue {
    protected string name = "";
    protected int valueRelativeToWholeNote = 0;
    protected bool isRest = false;

    public string getName()
    {
        return this.name;
    }

    public int getValueRelativeToWholeNote()
    {
        return this.valueRelativeToWholeNote;
    }

    public bool isItRest()
    {
        return isRest;
    }

}

public class WholeNote : RhythmValue
{
    public WholeNote()
    {
        this.name = "WholeNote";
        this.valueRelativeToWholeNote = 1;
        this.isRest = false;
    }     
}

public class HalfNote : RhythmValue
{
    public HalfNote()
    {
        this.name = "HalfNote";
        this.valueRelativeToWholeNote = 2;
        this.isRest = false;
    }
}


public class QuarterNote : RhythmValue
{
    public QuarterNote()
    {
        this.name = "QuarterNote";
        this.valueRelativeToWholeNote = 4;
        this.isRest = false;
    }
}

public static class RhythmValues
{
    public static WholeNote wholeNote = new WholeNote();
    public static HalfNote halfNote = new HalfNote();
    public static QuarterNote quarterNote = new QuarterNote();

    public static RhythmValue convertDenominatorFromMIDI(int denominatorVal)
    {
        // Denominator: number of quarter notes in a beat.0=ronde, 1=blanche, 2=quarter, 3=eighth, etc. 
        RhythmValue realValue = null;
        if(denominatorVal == 0)
        {
            return wholeNote;
        }else if(denominatorVal == 1)
        {
            return halfNote;
        }else if(denominatorVal == 2)
        {
            return quarterNote;
        }
        //add more later

        return realValue;
    }
}

public static class CurrentBeatValuesTable
{
    private static int toleranceError = 10;
    private static int quarterNoteMilis = 0;
    private static int halfNoteMilis = 0;
    private static int wholeNoteMilis = 0;
    private static int measureMilis = 0;
    private static float beatValueMilis = 0;
    private static int beatsPerMeasure = 0;
    private static float beatValueSeconds = 0;



    public static void UpdateValues(RhythmValue denominatorValue, int numeratorValue, int microsecondsEachQuarterNote )
    {
        int valueMilisQuarterNote = microsecondsEachQuarterNote / 1000;
        quarterNoteMilis = valueMilisQuarterNote;
        halfNoteMilis = quarterNoteMilis * 2;
        wholeNoteMilis = quarterNoteMilis * 4;

        //(quarterVal/denominatorVal)*numerator * milisQuarter
        measureMilis = RhythmValues.quarterNote.getValueRelativeToWholeNote() / denominatorValue.getValueRelativeToWholeNote() * numeratorValue * quarterNoteMilis;
        beatValueMilis = measureMilis / numeratorValue;
        beatsPerMeasure = numeratorValue;
        beatValueSeconds = beatValueMilis / 1000.0f;
    }
    public static int getToleranceErrorMilis()
    {
        return toleranceError;
    }
    public static RhythmValue getRhythmValue(int miliseconds, bool isRest)
    {
        RhythmValue value = null;
        if(miliseconds>quarterNoteMilis-toleranceError && miliseconds < quarterNoteMilis + toleranceError)
        {
            if (!isRest)
            {
                return RhythmValues.quarterNote;
            }
        }else if (miliseconds > halfNoteMilis - toleranceError && miliseconds < halfNoteMilis + toleranceError)
        {
            if (!isRest)
            {
                return RhythmValues.halfNote;
            }
        }
        else if (miliseconds > wholeNoteMilis - toleranceError && miliseconds < wholeNoteMilis + toleranceError)
        {
            if (!isRest)
            {
                return RhythmValues.wholeNote;
            }
        }
        //implement rests and other small notes - doted rythms
        return value;
    }

    public static int getValueQuarterMS()
    {
        return quarterNoteMilis;
    }

    public static int getValueHalfMS()
    {
        return halfNoteMilis;
    }

    public static int getValueWholeMS()
    {
        return wholeNoteMilis;
    }

    public static int getValueMeasureMS()
    {
        return measureMilis;
    }

    public static float getBeatValueMS()
    {
        return beatValueMilis;
    }

    public static float getBeatValueSeconds()
    {
        return beatValueSeconds;
    }

    public static int getBeatsPerMeasure()
    {
        return beatsPerMeasure;
    }

    public static bool isInsideErrorTolerance(int valToBeEvaluated, int centralValue)
    {
        if (valToBeEvaluated > centralValue - toleranceError && valToBeEvaluated < centralValue + toleranceError)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


