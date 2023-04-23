
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class SoundManager {
    float totalTime = 0f;
    int countPrevBeats = 0;
    float midiTime = 0f;
    List<MPTKEvent> listNotesMidi;
    MidiStreamPlayer midiStreamPlayer;
    Metronome metronome;

    public SoundManager(MidiStreamPlayer midiStreamPlayer, List<MPTKEvent> listNotesMidi, MidiFileLoader loader, float delayFirstNote)
    {
        this.midiStreamPlayer = midiStreamPlayer;
        this.listNotesMidi = listNotesMidi;
        metronome = new Metronome(midiStreamPlayer);
        metronome.SetMetronomeValues(loader, delayFirstNote);

    }

    public void playSounds(float deltaTime)
    {
        if (metronome.isOn())
        {
            if (countPrevBeats <= metronome.beatsBeforeStart())
            {
                if (metronome.checkIfTimeToSound(totalTime))
                {
                    //play the sound
                    metronome.PlayBeatMetronome();
                    countPrevBeats++;
                }
            }
            else //anticipation beats are done
            {
                if (metronome.checkIfTimeToSound(totalTime))
                {
                    //play the sound
                    metronome.PlayBeatMetronome();
                }
                List<MPTKEvent> notesJustPlayed = new List<MPTKEvent>();
                foreach (MPTKEvent noteEvent in listNotesMidi)//play the piano
                {
                    if (midiTime * 1000 >= noteEvent.RealTime)
                    {
                        Debug.Log("midit = " + midiTime + " RealTime" + noteEvent.RealTime);
                        PlayOneNote(noteEvent);
                        notesJustPlayed.Add(noteEvent);
                    }
                }
                foreach (MPTKEvent noteEvent in notesJustPlayed)
                {
                    listNotesMidi.Remove(noteEvent);
                }
                midiTime += deltaTime;
            }
            totalTime += deltaTime;
        }
    }

    public void playJustMetronome(float deltaTime)
    {
        if (metronome.isOn())
        {
            if (countPrevBeats <= metronome.beatsBeforeStart())
            {
                if (metronome.checkIfTimeToSound(totalTime))
                {
                    //play the sound
                    metronome.PlayBeatMetronome();
                    countPrevBeats++;
                }
            }
            else //anticipation beats are done
            {
                if (metronome.checkIfTimeToSound(totalTime))
                {
                    //play the sound
                    metronome.PlayBeatMetronome();
                }
            }
            totalTime += deltaTime;
        }
    }

    private void PlayOneNote(MPTKEvent noteIn)
    {
        MPTKEvent NotePlaying;
        // Start playing a new note
        NotePlaying = new MPTKEvent()
        {
            Command = MPTKCommand.NoteOn,
            Value = noteIn.Value, // note to played, ex 60=C5. Use the method from class HelperNoteLabel to convert to string
            Channel = 1,
            Duration = noteIn.Duration, // millisecond, -1 to play indefinitely
            Velocity = noteIn.Velocity, // Sound can vary depending on the velocity
            Delay = Convert.ToInt64(0),
        };

        midiStreamPlayer.MPTK_PlayEvent(NotePlaying);
    }

    public Metronome getMetronome()
    {
        return this.metronome;
    }
}


public class Metronome //VERIFY THE UNITS
{
    MidiStreamPlayer midiStreamPlayer;
    List<float> listBeatsToPlay;

    double tempo = 60; //BPM 

    int channel = 2;
    int instrument = 115; //WoodBlock
    int timeSigNumerator = 4;
    int timeSigDenominator = 4; // 4/4 by default

    float initialDelay = 0.0f;
    int totalBeats = 64; //default 64 beats in total

    int pickupBeats = 0;
    float maxDurationAllMidi = 30f; //in seconds, 30 by default

    private float internalTimeCounterForBeat = 0f;
    private float totalTimeCounter = 0.0f;
    private float beatIntervalSeconds = 0.0f;
    private float beatIntervalMS = 0.0f;

    private bool on;

    public Metronome(MidiStreamPlayer midiStreamPlayer)
    {
        this.midiStreamPlayer = midiStreamPlayer;
        this.listBeatsToPlay = new List<float>();
        this.on = true; //to start
    }

    public void SetMetronomeValues(MidiFileLoader loader, float delayFirstNote)
    {
        tempo = loader.MPTK_InitialTempo;
        timeSigDenominator = loader.MPTK_TimeSigDenominator; //VERIFY THE UNITS
        timeSigNumerator = loader.MPTK_TimeSigNumerator;
        maxDurationAllMidi = loader.MPTK_DurationMS / 1000f; //saved in seconds
        initialDelay = delayFirstNote;
        // beatIntervalSeconds = 60 / tempo;
        beatIntervalSeconds = CurrentBeatValuesTable.getBeatValueMS()/1000;
        beatIntervalMS = CurrentBeatValuesTable.getBeatValueMS();
        //Debug.Log("%%%%%%%%%%% IntervalMS = " + beatIntervalMS);
        midiStreamPlayer.MPTK_ChannelForcedPresetSet(channel, instrument); //channel 2 set with WoodBlock 115

        float timeCounter = 0;
        float realDuration = beatsBeforeStart() * beatIntervalSeconds + maxDurationAllMidi;
        for(int i = 0; timeCounter <= realDuration; i++)
        {
            this.listBeatsToPlay.Add(timeCounter);
            timeCounter = timeCounter + beatIntervalSeconds;
           
        }
        /*for(int i =0; i < listBeatsToPlay.Count; i++)
        {
            Debug.Log("++++++++" + this.listBeatsToPlay[i] + "------");
        }*/
    }

    public int beatsBeforeStart()
    {
        return timeSigNumerator - pickupBeats; //add logic for compound measures
    }

    public float secondsBeforeStart()
    {
        return this.beatsBeforeStart() * this.beatIntervalSeconds;
    }

    public bool checkIfTimeToSound()
    {
        return internalTimeCounterForBeat >= beatIntervalSeconds;
    }

    public bool checkIfTimeToSound(float currentTime)
    {
        if (this.listBeatsToPlay.Count > 0)
        {
            if(currentTime >= this.listBeatsToPlay[0])
            {
                //Debug.Log(" -- [0] = " + listBeatsToPlay[0] + " CurrentTime = " + currentTime + "----------------------------------------");

                this.listBeatsToPlay.RemoveAt(0);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
       
    }

    public void addTimeToTimeCounterForBeat(float deltaTime)
    {
        internalTimeCounterForBeat += deltaTime;
    }

    public void addTimeToTotalTimeCounter(float deltaTime)
    {
        totalTimeCounter += deltaTime;
    }

    public void resetTimeCounterForBeat()
    {
        internalTimeCounterForBeat = 0;
    }

    public bool isTheEnd()
    {
        //Debug.Log("TTC = " + totalTimeCounter + " -- Goal =" + maxDurationAllMidi);
        return (totalTimeCounter >= maxDurationAllMidi);
    }

    public bool isOn()
    {
        return this.on;
    }

    public void PlayBeatMetronome()
    {
        MPTKEvent NotePlaying;
        // Start playing a new note
        NotePlaying = new MPTKEvent()
        {
            Command = MPTKCommand.NoteOn,
            Value = 76, //sound of woodBlock
            Channel = channel,
            Duration = Convert.ToInt64(beatIntervalMS), // millisecond, -1 to play indefinitely
            Velocity = 60, // Sound can vary depending on the velocity
            Delay = Convert.ToInt64(0),
        };

        midiStreamPlayer.MPTK_PlayEvent(NotePlaying);

    }
}

