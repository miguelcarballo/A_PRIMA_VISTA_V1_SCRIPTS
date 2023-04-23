using System;
using System.Collections;
using System.Collections.Generic;
using MidiPlayerTK;
using UnityEngine;

public class MIDI_READER_APV : MonoBehaviour
{
   
    //String NameOfMIDIFile = "OdeToJoy";
    //String NameOfMIDIFile = "EdgerLinesTest";
    public GameObject musicStaff;
    public GameObject[] prefabMusicSymbols;

    MidiFileLoader loader; //MPTK fileLoader
    MidiStreamPlayer midiStreamPlayer;
    ScoreStructureManager scoreStructureManager;
    ScoreGraphicManager scoreGraphicManager;
    SoundManager soundManager;
   

    float counterTime = 0.0f;
    float counterPieceTime = 0.0f;
    float maxValueTimer = 0.0f;
 

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.initializeAllLevels();
        GENERAL_HANDLER.setMidiReader_APV(this);
        midiStreamPlayer = FindObjectOfType<MidiStreamPlayer>();
        //LoadMIDIFile();//includes create music structure and initialize the graphic/audio manager
       

    }

    // Update is called once per frame
    void Update()
    {
        if (GENERAL_HANDLER.isPieceAllowedToPlay())
        {
            if (counterTime <= maxValueTimer)
            {
                if (counterTime < soundManager.getMetronome().secondsBeforeStart())
                {

                }
                else
                {
                    GAMESCORE_HANDLER.setCurrentPieceTime(counterPieceTime);
                    counterPieceTime = counterPieceTime + Time.deltaTime;
                }
                //soundManager.playSounds(Time.deltaTime);
                soundManager.playJustMetronome(Time.deltaTime);
                scoreGraphicManager.moveAllSymbols(Time.deltaTime);
                counterTime = counterTime + Time.deltaTime;
            }
            else //the piece has finished
            {
                GENERAL_HANDLER.showResultMenu();
                GENERAL_HANDLER.resetValues();
            }
        }


    }

    public void LoadMIDIFile(string NameOfMIDIFile)
    {
        //String NameOfMIDIFile = "Twinkle Twinkle Little Star";
        //----Just for the sound
        List<MPTKEvent> listNotesMidi = new List<MPTKEvent>();
        bool delayIsSet = false;
        float delayFirstNote = 0;
        //-----

        // A MidiFileLoader prefab must be added to the hierarchy with the editor (see menu MPTK)
        loader = FindObjectOfType<MidiFileLoader>();

        if (loader == null)
        {
            Debug.LogWarning("Can't find a MidiFileLoader Prefab in the current Scene Hierarchy. Add it with the MPTK menu.");
            return;
        }

        // Index of the midi in the MidiDB (find it with 'Midi File Setup' from the menu MPTK)
        //loader.MPTK_MidiIndex = MidiIndex;
        loader.MPTK_MidiName = NameOfMIDIFile;
        //loader.MPTK_MidiName = "TestRests";

        // Open and load the Midi
        loader.MPTK_Load();

        // Read midi event to a List<>
        List<MPTKEvent> mptkEvents = loader.MPTK_ReadMidiEvents();

        //--------------Just for the sound (for the moment)
        // Loop on each MIDI events
        foreach (MPTKEvent mptkEvent in mptkEvents)
        {

            // Log if event is a note on

            //loader.MPTK_DeltaTicksPerQuarterNote
            if (mptkEvent.Command == MPTKCommand.NoteOn)
            {
                //Debug.Log($"Note On at {mptkEvent.RealTime} millisecond  Channel:{mptkEvent.Channel} Note:{mptkEvent.Value}  Duration:{mptkEvent.Duration} millisecond  Velocity:{mptkEvent.Velocity} Length:{loader.MPTK_NoteLength(mptkEvent)}");
                listNotesMidi.Add(mptkEvent);
                if (!delayIsSet)
                {
                    delayFirstNote = mptkEvent.RealTime;
                    delayIsSet = true;
                }
            }
            else if (mptkEvent.Command == MPTKCommand.PatchChange)
                Debug.Log($"Patch Change at {mptkEvent.RealTime} millisecond  Channel:{mptkEvent.Channel}  Preset:{mptkEvent.Value}");
            else if (mptkEvent.Command == MPTKCommand.ControlChange)
            {
                if (mptkEvent.Controller == MPTKController.BankSelectMsb)
                    Debug.Log($"Bank Change at {mptkEvent.RealTime} millisecond  Channel:{mptkEvent.Channel}  Bank:{mptkEvent.Value}");
            }
            // Uncomment to display all MIDI events
            //Debug.Log(mptkEvent.ToString());
        }
       
        //--------------

        //Using own structure
        scoreStructureManager = new ScoreStructureManager(loader, mptkEvents);
        //scoreStructureManager.printListMeasures();

        soundManager = new SoundManager(midiStreamPlayer, listNotesMidi, loader, delayFirstNote); //this is for metronome too

        //Graphic part
        scoreGraphicManager = new ScoreGraphicManager(musicStaff, prefabMusicSymbols, scoreStructureManager);
        scoreGraphicManager.drawAllSymbolsOnScore();

        //game handler - score game
        GAMESCORE_HANDLER.setManagers(scoreStructureManager, scoreGraphicManager);

        //set the timers
        maxValueTimer = soundManager.getMetronome().secondsBeforeStart() + scoreStructureManager.getDurationOfThePiece_Seconds();

        //set/reset
        counterTime = 0.0f;
        counterPieceTime = 0.0f;
        
    }


    public void deleteAllMusicSymbols()
    {
        this.scoreGraphicManager.deleteAllSymbols();
    }
}
