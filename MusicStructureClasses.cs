using System;
using System.Linq;
using System.Collections.Generic;
using MidiPlayerTK;
using UnityEngine;

public class Measure
{
    private float beatsPerMeasure = 0; //by default
    private RhythmValue beatUnit = null;

    private float pointerTotalBeats = 0.0f;

    private int timeStartMS = 0;
    private int timeEndMS = 0;
    private int beatValueMS = 0;

    private List<MusicNote> listMusicNotes = new List<MusicNote>();
    

    public Measure(int beatsPerMeasure, RhythmValue beatUnit, int beatValueMilis, int timeStart, int timeEnd)
    {
       // Debug.Log("MEASURE --> bpm=" + beatsPerMeasure + " RV=" + beatUnit.getName() + " ts=" + timeStart + "te=" + timeEnd);
        this.beatsPerMeasure = (float)beatsPerMeasure;
        this.beatUnit = beatUnit;
        this.timeEndMS = timeEnd;
        this.timeStartMS = timeStart;
        this.beatValueMS = beatValueMilis;
    }

    public bool isEventStartingInsideMeasure(MPTKEvent noteEvent)
    {
        float starts = noteEvent.RealTime;
        float ends = starts + noteEvent.Duration;
       // Debug.Log("EVENT start " + noteEvent.RealTime + " timeStart=" + timeStartMS + " ends=" + timeEndMS);
        if(starts >= timeStartMS && starts < timeEndMS)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isEventFinsihingInsideMeasure(MPTKEvent noteEvent)
    {
        float starts = noteEvent.RealTime;
        float ends = starts + noteEvent.Duration;

        if (ends > timeStartMS && starts <= timeEndMS+ CurrentBeatValuesTable.getToleranceErrorMilis())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int getEndMeasureMS()
    {
        return this.timeEndMS;
    }

    public int getBeatValueMS()
    {
        return this.beatValueMS;
    }
    public void addNote(MPTKEvent noteEvent)
    {
        MusicNote newNote = new MusicNote(noteEvent);
        this.listMusicNotes.Add(newNote);
    }

    public void printAllNotes()
    {
        foreach(MusicNote note in listMusicNotes)
        {
            Debug.Log("-" + note.toStringToPrint());
        }
    }

    public List<MusicNote> getAllNotes()
    {
        return this.listMusicNotes;
    }
}

public class MusicNote
{
    private RhythmValue rhythmValue;
    private string pitchName = "";
    private int midiPitch = 0;
    private bool rest = false;
    private float noteStartsOn = 0.0f;
    private float noteFinishesOn = 0.0f;

    public MusicNote(MPTKEvent noteEvent)
    {
        //Debug.Log($"Note On at {mptkEvent.RealTime} millisecond
        //Channel:{mptkEvent.Channel} Note:{mptkEvent.Value}  Duration:{mptkEvent.Duration} millisecond
        //Velocity:{mptkEvent.Velocity} Length:{loader.MPTK_NoteLength(mptkEvent)}");

        this.pitchName = UtilMidiConv.getNoteName(noteEvent.Value);
        this.midiPitch = noteEvent.Value;
        this.rest = false;
        this.noteStartsOn = noteEvent.RealTime;
        this.noteFinishesOn = noteEvent.RealTime + noteEvent.Duration;
        this.rhythmValue = CurrentBeatValuesTable.getRhythmValue((int)noteEvent.Duration, false);
    }

    public string toStringToPrint()
    {
        //return "";
        return "MusicNote name= " + this.pitchName + "|" + this.rhythmValue.getName()
           + "|" + this.midiPitch + "|start=" + this.noteStartsOn + "|ends=" + this.noteFinishesOn;
    }

    public string getPitchName()
    {
        return this.pitchName;
    }

    public RhythmValue GetRhythmValue()
    {
        return this.rhythmValue;
    }

    public float getNoteStartsOnMS()
    {
        return this.noteStartsOn;
    }
    public float getNoteStartsOnSecond()
    {
        return this.noteStartsOn/1000.0f;
    }
    public float getNoteFinishOnSecond()
    {
        return this.noteFinishesOn / 1000.0f;
    }

    public bool isRest()
    {
        return this.rest;
    }
}

public static class UtilMidiConv
{
    public static string getNoteName(int noteNum)
    {
     
        var notes = "C C#D D#E F F#G G#A A#B ";
        int octv;
        string nt;
        octv = (int)(noteNum / 12) - 1;
        nt = notes.Substring((noteNum % 12) * 2, (noteNum % 12) * 2 + 2 - (noteNum % 12) * 2);
        //Console.WriteLine("Note # " + noteNum.ToString() + " = octave " + octv.ToString() + ", note " + nt);
        string nameNote = nt + "" + octv;
  
        string nameNoteNS = String.Concat(nameNote.Where(c => !Char.IsWhiteSpace(c)));
        return nameNoteNS;
    }

    public static int convertToPitch(string note)
    {
        string sym = "";
        char oct = ' ';
        string[][] notes = new string[][] {
           new string[]{"C"},
           new string[]{"Db", "C#"},
           new string[] {"D"},
           new string[] {"Eb", "D#"},
           new string[] {"E"},
           new string[] {"F"},
           new string[] {"Gb", "F#"},
           new string[] {"G"},
           new string[] {"Ab", "G#"},
           new string[] {"A"},
           new string[] {"Bb", "A#"},
           new string[]{"B"} };

        char[] splitNote = note.ToCharArray();

        // If the length is two, then grab the symbol and number.
        // Otherwise, it must be a two-char note.
        if (splitNote.Length == 2)
        {
            sym += splitNote[0];
            oct = splitNote[1];
        }
        else if (splitNote.Length == 3)
        {
            sym += char.ToString(splitNote[0]);
            sym += char.ToString(splitNote[1]);
            oct = splitNote[2];
        }

        // Find the corresponding note in the array.
        for (int i = 0; i < notes.Length; i++)
            for (int j = 0; j < notes[i].Length; j++)
            {
                if (notes[i][j].Equals(sym))
                {
                    return ((int)char.GetNumericValue(oct) + 1) * 12 + i;
                    //+1 is to fix the calculation according to the International Pitch
                }
            }

        // If nothing was found, we return -1.
        return -1;
    }

}