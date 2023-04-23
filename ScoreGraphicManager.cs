using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using VolumetricLines;

public class ScoreGraphicManager {
    public GameObject musicStaff;
    public GameObject[] prefabMusicSymbols;
    public ScoreStructureManager scoreStructureManager;
    private VolumetricLineBehavior readerPointerBehavior;
    PosYNotePitch[] arrayNotesPitchPosY;
    List<string> listAllNaturalNoteName = new List<string>();
    List<GraphicSymbolAPV> listOfSymbolsAPV = new List<GraphicSymbolAPV>(); //generated objects for score
   
    private TMPro.TextMeshPro txtScoreVal;
    private TMPro.TextMeshPro txtComment;
    private TMPro.TextMeshPro txtEarlyVal;
    private TMPro.TextMeshPro txtLateVal;

    Vector3 line1pos;
    Vector3 line2pos;
    Vector3 minDistanceNotes;

    //horizontal size of the staff
    float sizeXStaff = 0;
    Vector3 iniPositionStaff;
    //create the beat distance accordingly
    float ratioStaffTimeBeat = 1.0f / 12.0f; //12 beats inside the staff

    //set initial offset (for the line player)
    float ratioInitialOffsetX = 1.0f / 2.0f;

    float ratioLeftBorderX = 1.0f / 9.0f;

    //IMPORTANT
    float beatDistance = 0; //it is going to be modified once initialized
    float initialOffsetX = 0;
    float leftBorderX = 0;
    float rightBorderX = 0;


    public ScoreGraphicManager(GameObject musicStaff, GameObject[] prefabMusicSymbols, ScoreStructureManager scoreStructureManager)
    {
        this.musicStaff = musicStaff;
        this.prefabMusicSymbols = prefabMusicSymbols;
        this.scoreStructureManager = scoreStructureManager;
        analyzeAndSetPositions();
        createListOfNotes();
    }

    public void analyzeAndSetPositions()
    {
        //get script of reader pointer
        GameObject lineLight = musicStaff.transform.Find("ReaderPointer").gameObject; 
        readerPointerBehavior = lineLight.GetComponent<VolumetricLineBehavior>();

        //get textMeshPro----
        GameObject gameTxtScore = musicStaff.transform.Find("txtScoreVal").gameObject;
        Component[] components = gameTxtScore.GetComponents(typeof(Component));
        foreach (Component component in components)
        {
            Debug.Log("%%%%%%%%%%%%%%%%%%%%%%%%%%%" + component.ToString());
        }
        txtScoreVal = gameTxtScore.GetComponent<TMPro.TextMeshPro>();
        Debug.Log("%%%%%%%%%%%%%%%% + " + txtScoreVal.text);

        GameObject gameTxtComment = musicStaff.transform.Find("txtComment").gameObject;
        txtComment = gameTxtComment.GetComponent<TMPro.TextMeshPro>();
        //Debug.Log("%%%%%%%%%%%%%%%% + " + txtComment.text);

        GameObject gameTxtEarly = musicStaff.transform.Find("txtEarlyVal").gameObject;
        txtEarlyVal = gameTxtEarly.GetComponent<TMPro.TextMeshPro>();

        GameObject gameTxtLate = musicStaff.transform.Find("txtLateVal").gameObject;
        txtLateVal = gameTxtLate.GetComponent<TMPro.TextMeshPro>();

        //----Line1 and 2
        GameObject line1 = musicStaff.transform.Find("Line1").gameObject;
        line1pos = line1.transform.position;
        //Debug.Log("_____Line1 = " + line1pos.ToString());

        GameObject line2 = musicStaff.transform.Find("Line2").gameObject;
        line2pos = line2.transform.position;
        //Debug.Log("_____Line2 = " + line2pos.ToString());

        minDistanceNotes = (line2pos - line1pos)/2;

        sizeXStaff = line1.transform.lossyScale.x;
        Vector3 auxPos = line1.transform.position;
        auxPos.x = auxPos.x + sizeXStaff / 2;
        iniPositionStaff = auxPos; //set the referenced position of the staff in the left down corner (line1)
        beatDistance = sizeXStaff * ratioStaffTimeBeat;
        initialOffsetX = sizeXStaff * ratioInitialOffsetX + beatDistance*CurrentBeatValuesTable.getBeatsPerMeasure(); //initial offset + 1 measure distance before to start

        leftBorderX = iniPositionStaff.x - sizeXStaff * ratioLeftBorderX;
        rightBorderX = iniPositionStaff.x - sizeXStaff;

        //Debug.Log("-----sizeStaff = " + sizeXStaff);

    }

    private void createListOfNotes()
    {
        //string[] notes = { "A", "B", "C", "D", "E", "F", "G" };
        string[] notes = {  "C", "D", "E", "F", "G", "A", "B" };
        //midi has from 21 to 127, from A0 to G#9 -> we have to extract just the name of them
            
        listAllNaturalNoteName.Add("A0");
        listAllNaturalNoteName.Add("B0");
        for (int i = 1; i <= 9; i++)
        {
            for(int cNote = 0; cNote < notes.Length; cNote++)
            {
                listAllNaturalNoteName.Add(notes[cNote]+i);
            }
        }

        arrayNotesPitchPosY = new PosYNotePitch[listAllNaturalNoteName.Count]; //create the vector of positions

        //get the index of G4 (second line treble cleff)
        int indexG4 = findIndexNaturalNotes("G4");

        for(int i = indexG4; i< listAllNaturalNoteName.Count; i++)
        {
            int distToG4 = i - indexG4;
            //the position = G4posy + distanceToG4*(minDistance)

            PosYNotePitch newPosNotePitch = new PosYNotePitch(listAllNaturalNoteName[i], line2pos.y + distToG4 * (minDistanceNotes.y));
            arrayNotesPitchPosY[i] = newPosNotePitch;
        }
        for (int i = indexG4-1; i >= 0; i--)
        {
            int distToG4 = i - indexG4;
            //the position = G4posy + distanceToG4*(minDistance)
            PosYNotePitch newPosNotePitch = new PosYNotePitch(listAllNaturalNoteName[i], line2pos.y + distToG4 * (minDistanceNotes.y));
            arrayNotesPitchPosY[i] = newPosNotePitch;
        }


        for(int i = 0; i < arrayNotesPitchPosY.Length; i++)
        {
            //Debug.Log(i + " -> " + arrayNotesPitchPosY[i].getName() + " -- " + arrayNotesPitchPosY[i].getPosY());
           // Debug.Log(i + " * " + listAllNaturalNoteName[i] + " -- ");
        }

        //Debug.Log("Line2posY = " + line2pos.y + " -- minDistY =  " + minDistanceNotes.y);
    }

    private int findIndexNaturalNotes(string note)
    {
        int index = 0;
        string cleanNote = cleanNoteToNatural(note);
        for(int i = 0; i < listAllNaturalNoteName.Count; i++)
        {
            if (listAllNaturalNoteName[i].Equals(cleanNote))
            {
                index = i;
                break;
            }
        }
        return index;
    }

    private bool isIndexInLine(int indexNote) //verify if the note is in the line or the space
    {
        //since A0 is Space and the index is 0 (even), all the odd numbers have lines
        if (indexNote % 2 == 1) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string cleanNoteToNatural(string note)
    {
        string cleanNote = "";

        char[] splitNote = note.ToCharArray();

        // If the length is two, then grab the symbol and number.
        // If the lenght is three, delete the sharp or flat
        if (splitNote.Length == 2)
        {
            cleanNote = note;
        }
        else if (splitNote.Length == 3)
        {
            cleanNote = char.ToString(splitNote[0]) + char.ToString(splitNote[2]);
        }

        return cleanNote;
    }

    private PosYNotePitch getPosYNotePitch(string note) //is going to give the posY of any note------
    {
        PosYNotePitch posNoteYPitch = null;
        note = cleanNoteToNatural(note);
        for(int i = 0; i < arrayNotesPitchPosY.Length; i++)
        {
            if (arrayNotesPitchPosY[i].getName().Equals(note))
            {
                posNoteYPitch = arrayNotesPitchPosY[i];
                break;
            }
        }
        return posNoteYPitch;
    }


    public class PosYNotePitch
    {
        string name;
        float posY;

        public PosYNotePitch(string name, float posY)
        {
            this.name = name;
            this.posY = posY;
        }

        public string getName()
        {
            return this.name;
        }
        public float getPosY()
        {
            return this.posY;
        }
    }
    //---------------------------------------------------------

    public List<GraphicSymbolAPV> drawAllSymbolsOnScore()
    {
        modifyReaderPointer(Color.green, 5.0f, 0.9f);
        List<Measure> listMeasures = scoreStructureManager.getlistMeasures();
        
        foreach (Measure measure in listMeasures)
        {
            int beatValueMS = measure.getBeatValueMS();
            List<MusicNote> musicNotes = measure.getAllNotes();
            foreach(MusicNote musicNote in musicNotes)
            {
                Debug.Log(musicNote.toStringToPrint());
                Debug.Log("beatVMs = " + beatValueMS);
                GraphicSymbolAPV newGraphic = graphicMusicNote(musicNote, beatValueMS);
                List<GraphicSymbolAPV> edgerLines = graphicLedgerLines(musicNote, beatValueMS);
                if (edgerLines!= null)
                {
                    listOfSymbolsAPV.AddRange(edgerLines);
                }
                
                listOfSymbolsAPV.Add(newGraphic);
            }
        }
        foreach (GraphicSymbolAPV symbol in listOfSymbolsAPV) //instantiate all symbols and replace the real inside list
        {
            GameObject newGO = GameObject.Instantiate(symbol.getGameObject(), symbol.getPosition(), symbol.getRotation());
            symbol.setInstantiatedGameObject(newGO);
        }

        return listOfSymbolsAPV;
    }

    public GraphicSymbolAPV graphicMusicNote(MusicNote musicNote, int beatValueMS)
    {
        //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!! " + musicNote.getPitchName());
        PosYNotePitch posYNote = getPosYNotePitch(musicNote.getPitchName());
        float posXNote = beatDistance * musicNote.getNoteStartsOnMS() / beatValueMS;

        Vector3 newPosNote = new Vector3();
        newPosNote = this.iniPositionStaff;
        newPosNote.x = this.iniPositionStaff.x - initialOffsetX - posXNote; //left is positive direction
        newPosNote.y = posYNote.getPosY();

        GameObject prefab = getGameObjectPrefabMusicSymbols(musicNote.GetRhythmValue().getName());
        GraphicSymbolAPV newGraphic = new GraphicSymbolAPV(prefab, newPosNote, this);
        bool itHasToRotate = evaluateRotation(musicNote);
        if (itHasToRotate)
        {
            newGraphic.rotate();
        }
        return newGraphic;
    }

    private List<GraphicSymbolAPV> graphicLedgerLines(MusicNote musicNote, int beatValueMS)
    {
        List<GraphicSymbolAPV> ledgerLines = new List<GraphicSymbolAPV>();
        //get index of the highest and lowest note without ledger lines
        int indexLowestNoteWithoutLL = findIndexNaturalNotes("D4");
        int indexHighestNoteWithoutLL = findIndexNaturalNotes("G5");
        int indexCurrentNote = findIndexNaturalNotes(musicNote.getPitchName());
        if (indexCurrentNote < indexLowestNoteWithoutLL) // if it is lower, we have to add ledge lines
        {
            for (int i = indexLowestNoteWithoutLL; i >= indexCurrentNote; i--) //create all the necessary ledge lines
            {
                bool noteInLine = isIndexInLine(i); //verify if the note has a line
                if (noteInLine)
                {
                    GraphicSymbolAPV newGraphic = getLedgerLineGraph(i, musicNote, beatValueMS);
                    ledgerLines.Add(newGraphic);
                }
            }

        } else if(indexCurrentNote > indexHighestNoteWithoutLL)
        {
            for (int i = indexHighestNoteWithoutLL; i <= indexCurrentNote; i++) //create all the necessary ledge lines
            {
                bool noteInLine = isIndexInLine(i); //verify if the note has a line
                if (noteInLine)
                {
                    GraphicSymbolAPV newGraphic = getLedgerLineGraph(i, musicNote, beatValueMS);
                    ledgerLines.Add(newGraphic);
                }
            }
        }
        return ledgerLines;
    }

    private GraphicSymbolAPV getLedgerLineGraph(int i, MusicNote musicNote, int beatValueMS)
    {
        string nameNoteOfLine = listAllNaturalNoteName[i];
        //Debug.Log("----------------------------------HAS a line : " + nameNoteOfLine);
        PosYNotePitch posYNote = getPosYNotePitch(nameNoteOfLine);
        float posXNote = beatDistance * musicNote.getNoteStartsOnMS() / beatValueMS;
        Vector3 newPosNote = new Vector3();
        newPosNote = this.iniPositionStaff;
        newPosNote.x = this.iniPositionStaff.x - initialOffsetX - posXNote; //left is positive direction
        newPosNote.y = posYNote.getPosY();

        GameObject prefab = getGameObjectPrefabMusicSymbols("LedgerLine");
        GraphicSymbolAPV newGraphic = new GraphicSymbolAPV(prefab, newPosNote, this);
        return newGraphic;
    }

    private bool evaluateRotation(MusicNote musicNote) //evaluate if note has steam down
    {
        if (!musicNote.GetRhythmValue().isItRest())
        {
            int indexCurrentNote = findIndexNaturalNotes(musicNote.getPitchName());
            int limitLineToFlip = findIndexNaturalNotes("B4");
            if (indexCurrentNote > limitLineToFlip)
            {
                return true;
            }
        }
        return false;
    }

    public void modifyReaderPointer(Color color, float lineWidth, float light) //setColor, lineWidth and lightsaber component
    {
        if (color != null)
        {
            readerPointerBehavior.LineColor = color;
        }
        if(lineWidth > 0.0f)
        {
            readerPointerBehavior.LineWidth = lineWidth;
        }
        if(light > 0.0f)
        {
            readerPointerBehavior.LightSaberFactor = light;
        }      
    }

    public GameObject getGameObjectPrefabMusicSymbols(string name)
    {
        GameObject gameObject = null;

        for (int i = 0; i<prefabMusicSymbols.Length; i++)
        {
            if (this.prefabMusicSymbols[i].name.Equals(name+"APV"))
            {
                gameObject = this.prefabMusicSymbols[i];
                break;
            }
        }
        return gameObject;
    }

    public void moveAllSymbols(float deltaTime)
    {
        //move according to delta time and the beat distance and time of beat
        //Debug.Log("dT = " + deltaTime + "  beatD = " + beatDistance + "  beatVInSeconds" + CurrentBeatValuesTable.getBeatValueSeconds());
        float deltaXMove = deltaTime * beatDistance / CurrentBeatValuesTable.getBeatValueSeconds();
        foreach(GraphicSymbolAPV symbol in listOfSymbolsAPV)
        {
            symbol.moveObjectInX(deltaXMove);
        }
    }

    public void deleteAllSymbols()
    {
        //--- rest values txt too
         txtScoreVal.text = "0";
         txtComment.text = "_";
         txtEarlyVal.text = "0";
         txtLateVal.text = "0";

        //--- delete symbols
        foreach (GraphicSymbolAPV symbol in listOfSymbolsAPV)
        {
            Object.Destroy(symbol.getGameObject());
        }
        listOfSymbolsAPV = new List<GraphicSymbolAPV>();
    }

    public float getRightBorderX()
    {
       return this.rightBorderX;
    }

    public float getLeftBorderX()
    {
        return this.leftBorderX;
    }

    //scoring point------
    public void setCommentAndColors(string typeEvaluation)
    {
        
        Color color = Color.grey;
        if (typeEvaluation.Equals(GameScoring.goodHitMsg))
        {
            Debug.Log("I DO GOOD");
            color = Color.green;
            modifyReaderPointer(color, -1f, -1f);
            
        }
        else if (typeEvaluation.Equals(GameScoring.earlyHitMsg))
        {
            //color = new Color(233f / 255f, 79f / 255f, 55f / 255f);
            Debug.Log("I DO Early");
            color = new Color(244f/255f, 187f/255f, 68f/255f);
            modifyReaderPointer(color, -1f, -1f);
        }
        else if (typeEvaluation.Equals(GameScoring.lateHitMsg))
        {
            //color = new Color(233f / 255f, 79f / 255f, 55f / 255f);
            Debug.Log("I DO Late");
            color = new Color(244f / 255f, 187f / 255f, 68f / 255f);
            modifyReaderPointer(color, -1f, -1f);
        }
        else if (typeEvaluation.Equals(GameScoring.notOnTempoMsg))
        {
            //Debug.Log("Not on tempo");
            color = Color.red;
            modifyReaderPointer(color, -1f, -1f);
        }
        else if (typeEvaluation.Equals(GameScoring.wrongNoteMsg))
        {
            //Debug.Log("THIS IS A MISTAKE");
            color = Color.red;
            modifyReaderPointer(color, -1f, -1f);
        }
        else if (typeEvaluation.Equals(GameScoring.noteSkippedMsg))
        {
            //Debug.Log("NOTE SKIPPED");
            color = Color.red;
            modifyReaderPointer(color, -1f, -1f);
        }
       
        else if (typeEvaluation.Equals("neutral"))
        {
            color = Color.grey;

        }
        txtComment.text = typeEvaluation;
        txtComment.color = color;
        txtScoreVal.text = GAMESCORE_HANDLER.getTotalScore().ToString();
    }

    public void updatePenaltyScore()
    {
        txtEarlyVal.text = GAMESCORE_HANDLER.getQuantityEarlyReleasePenalty().ToString();
        txtLateVal.text = GAMESCORE_HANDLER.getQuantityLateReleasePenalty().ToString();

    }

}

public class GraphicSymbolAPV
{
    private GameObject gameObject;
    private Vector3 position;
    private Quaternion rotation;
    private bool isRotated = false; //by default
    private ScoreGraphicManager scoreGraphicManager;


    public GraphicSymbolAPV(GameObject gameObject, Vector3 position, ScoreGraphicManager scoreGraphicManager)
    {
        this.gameObject = gameObject;
        this.position = position;
        this.rotation = Quaternion.identity;
        this.scoreGraphicManager = scoreGraphicManager;
    }

    public void rotate()
    {
        this.rotation = Quaternion.AngleAxis(180.0f, new Vector3(0, 0, 1));
        this.isRotated = true;
    }

    public GameObject getGameObject()
    {
        return this.gameObject;
    }

    public Vector3 getPosition()
    {
        return this.position;
    }

    public Quaternion getRotation()
    {
        return this.rotation;
    }

    public void setInstantiatedGameObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
        scalateObject(gameObject);
        checkIfHasToDisable();
    }

    private void scalateObject(GameObject gameObject) {
        var prefabGameObject = gameObject.name;
        
        //Debug.Log("THE NAME IS " + prefabGameObject);
        if (gameObject.name.Equals("LedgerLineAPV(Clone)")) //special case with LedgerLines
        {
            Vector3 staffScale = this.scoreGraphicManager.musicStaff.transform.localScale;
            float scaleX = staffScale.x * 11.83f;
            float scaleY = staffScale.y * 0.3f;
            float scaleZ = staffScale.z;
            Vector3 scale = new Vector3(scaleX, scaleY, scaleZ);
            gameObject.transform.localScale = scale;
        }
        else
        {
            //scalate all the symbols according to staff
            gameObject.transform.localScale = this.scoreGraphicManager.musicStaff.transform.localScale;
        }
    }

    public void moveObjectInX(float deltaX)
    {
        int direction = 1;
        if (this.isRotated)
        {
            direction = -1;
        }
        this.gameObject.transform.position = this.gameObject.transform.position + this.gameObject.transform.right * deltaX*direction;
        this.position = this.gameObject.transform.position;
        checkIfHasToDisable();
    }

    public void checkIfHasToDisable()
    {
        float leftBorderX = this.scoreGraphicManager.getLeftBorderX();
        float rightBorderX = this.scoreGraphicManager.getRightBorderX();

        if(this.gameObject.transform.position.x > leftBorderX || this.gameObject.transform.position.x < rightBorderX)
        {
            this.gameObject.SetActive(false);
            //this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }

    
}






