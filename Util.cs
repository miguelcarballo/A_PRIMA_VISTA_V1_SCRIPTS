using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{

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
                    return ((int)char.GetNumericValue(oct)  + 1) * 12 + i;
                    //+1 is to fix the calculation according to the International Pitch
                }
            }

        // If nothing was found, we return -1.
        return -1;
    }
}
