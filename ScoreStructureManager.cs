using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class ScoreStructureManager
{
    private int beatsPerMeasure = 0; //by default
    private RhythmValue beatUnit = null;

    private double tempo = 0;
    private float maxDurationPieceSeconds = 0.0f;
    private float initialDelayFirstNote = 0.0f;



    private List<Measure> listMeasures = new List<Measure>();

    public ScoreStructureManager(MidiFileLoader loader, List<MPTKEvent> mptkEvents)
    {
        tempo = loader.MPTK_InitialTempo;
        // Denominator: number of quarter notes in a beat.0=ronde, 1=blanche, 2=quarter, 3=eighth, etc. 
        beatUnit = RhythmValues.convertDenominatorFromMIDI(loader.MPTK_TimeSigDenominator);
        beatsPerMeasure = loader.MPTK_TimeSigNumerator;
        maxDurationPieceSeconds = loader.MPTK_DurationMS / 1000.0f; //saved in seconds
        CurrentBeatValuesTable.UpdateValues(beatUnit, beatsPerMeasure, loader.MPTK_MicrosecondsPerQuarterNote);

        readEventsAndCreateMusicStructure(loader, mptkEvents);

    }

    private void readEventsAndCreateMusicStructure(MidiFileLoader loader, List<MPTKEvent> mptkEvents)
    {

        int measureMS = CurrentBeatValuesTable.getValueMeasureMS();
        int beatValueMS = (int)CurrentBeatValuesTable.getBeatValueMS();
        Measure measure = new Measure(beatsPerMeasure, beatUnit, beatValueMS, 0, measureMS); //first measure
        bool delayIsSet = false;

        // Loop on each MIDI events
        int countEvent = 0;
        foreach (MPTKEvent mptkEvent in mptkEvents)
        {
            countEvent++; //number of events, starts in 1.

            //loader.MPTK_DeltaTicksPerQuarterNote
            if (mptkEvent.Command == MPTKCommand.NoteOn)
            {
                //Debug.Log($"Note On at {mptkEvent.RealTime} millisecond  Channel:{mptkEvent.Channel} Note:{mptkEvent.Value}  Duration:{mptkEvent.Duration} millisecond  Velocity:{mptkEvent.Velocity} Length:{loader.MPTK_NoteLength(mptkEvent)}");

                if (!delayIsSet)
                {
                    initialDelayFirstNote = mptkEvent.RealTime;
                    delayIsSet = true;
                }
                //--------- *** add logic to also detect rests -- create notes an insert them in measures
                //----rests can be added after all the notes are added to the measure
                bool noteAdded = false;

                while (!noteAdded)
                {
                    if (mptkEvent.Duration > measureMS)
                    {
                        //*** the note is bigger than the measure. Weird case, implement later
                        noteAdded = true;
                    }
                    else
                    {
                        if (measure.isEventStartingInsideMeasure(mptkEvent)) //the event starts in the measure
                        {
                            if (measure.isEventFinsihingInsideMeasure(mptkEvent)) //the event finishes inside the measure
                            {
                                measure.addNote(mptkEvent);
                                noteAdded = true;
                                if (countEvent == mptkEvents.Count)//if it is the last event, add to the measure list
                                {
                                    listMeasures.Add(measure);
                                }

                            }
                            else //***implement subdivition, the note starts in the measure but does not end in the same
                            {
                                noteAdded = true;
                            }

                        }
                        else //the event is in other measure, we have to create another (and save the previous one)
                        {
                            int endPrevMeasure = measure.getEndMeasureMS();
                            listMeasures.Add(measure);
                            measure = new Measure(beatsPerMeasure, beatUnit, beatValueMS, endPrevMeasure, endPrevMeasure + measureMS);
                        }
                    }

                }

                //----------
            }
            else if (mptkEvent.Command == MPTKCommand.PatchChange)
                Debug.Log($"Patch Change at {mptkEvent.RealTime} millisecond  Channel:{mptkEvent.Channel}  Preset:{mptkEvent.Value}");
            else if (mptkEvent.Command == MPTKCommand.ControlChange)
            {
                if (mptkEvent.Controller == MPTKController.BankSelectMsb)
                    Debug.Log($"Bank Change at {mptkEvent.RealTime} millisecond  Channel:{mptkEvent.Channel}  Bank:{mptkEvent.Value}");
            }
            //Set metronome values
            //*** SetMetronomeValues(loader, delayFirstNote);
        }

        printListMeasures();

    }

    public float getDurationOfThePiece_Seconds()
    {
        return this.maxDurationPieceSeconds;
    }

    public List<Measure> getlistMeasures()
    {
        return listMeasures;
    }

    public void printListMeasures()
    {
        for (int i = 0; i < listMeasures.Count; i++)
        {
            //Debug.Log("!!!MEASURE = " + (i + 1) + "------");
            listMeasures[i].printAllNotes();
        }
    }

    public List<MusicNote> getListAllNotes()
    {
        List<MusicNote> listAllNotes = new List<MusicNote>();
        for(int i = 0; i< this.listMeasures.Count; i++)
        {
            listAllNotes.AddRange(this.listMeasures[i].getAllNotes());
        }
        return listAllNotes;
    }

}
  