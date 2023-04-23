using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using System.Linq;

public class KeyNewBehaviour : MonoBehaviour
{
    public MidiStreamPlayer midiStreamPlayer;
    MPTKEvent NotePlaying;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        originalColor = gameObject.GetComponent<Renderer>().material.color;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayOneNote(string namePitchNote)
    {
        // Start playing a new note
        NotePlaying = new MPTKEvent()
        {
            Command = MPTKCommand.NoteOn,
            Value = Util.convertToPitch(gameObject.name), // note to played, ex 60=C5. 
            Channel = 1,
            Duration = Convert.ToInt64(4 * 1000f), // millisecond, -1 to play indefinitely
            Velocity = 60, // Sound can vary depending on the velocity
            Delay = Convert.ToInt64(0),
        };
        midiStreamPlayer.MPTK_PlayEvent(NotePlaying);
        if (GENERAL_HANDLER.isPieceAllowedToPlay())
        {
            GAMESCORE_HANDLER.evaluateKeyHit(namePitchNote);
        }
       
    }

    private void StopPlayingOneNote()
    {
        if (NotePlaying != null)
        {
            midiStreamPlayer.MPTK_StopEvent(NotePlaying);
            if (GENERAL_HANDLER.isPieceAllowedToPlay())
            {
                GAMESCORE_HANDLER.evaluateKeyReleased(gameObject.name);
            }              
            NotePlaying = null;
        }
        
    }
    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("OnTriggerEnter NAME:  " + collision.gameObject.name);
        //Debug.Log("MY NAME IS " + gameObject.name);
        if (collision.gameObject.name == "SphereRH" && !Data.rightIsPlaying)
        {
          
            //Color randomlySelectedColor = GetRandomColor();
            //---GameObject ChildGameObject1 = gameObject.transform.GetChild(0).gameObject;
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            PlayOneNote(gameObject.name);
            Data.rightIsPlaying = true;
        } else if (collision.gameObject.name == "SphereLH" && !Data.leftIsPlaying)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            Data.leftIsPlaying = true;
            string namePitchNote = gameObject.name;
            string namePitchNoteNS = String.Concat(namePitchNote.Where(c => !Char.IsWhiteSpace(c)));
            PlayOneNote(namePitchNoteNS);
        }

    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name == "SphereRH")
        {

            StopPlayingOneNote();
            Data.rightIsPlaying = false;
            gameObject.GetComponent<Renderer>().material.color = originalColor;

        }
        else if (collider.gameObject.name == "SphereLH")
        {
            StopPlayingOneNote();
            Data.leftIsPlaying = false;
            gameObject.GetComponent<Renderer>().material.color = originalColor;
        }
    }

    private Color GetRandomColor()
    {
        return new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f));
    }
}
