using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Score : MonoBehaviour {

    public Note root;

    public Clef treble;
    public Clef bass;
    public int bars;

    public void RandomScore() {
        print("Getting Score");
        treble = Score.GetRandomBar(bars);
        bass = Score.GetBasicBar(bars);
    }

    public GameObject noteNode;
    public NoteNode[] trebleNodes;
    public NoteNode[] bassNodes;

    public float scale;

    [Range(0.1f, 1f)] public float updateRate = 0.25f;

    public bool isEditing;

    public void Instantiate() {
        if (!isEditing) { return; }

        print("score");

        float subdivision = LengthMultipliers[NoteLength.EIGTH];
        trebleNodes = new NoteNode[(int)(bars * 4f / subdivision)];
        bassNodes = new NoteNode[(int)(bars * 4f / subdivision)];

        CreateNodes(trebleNodes, scale * 4f, subdivision);
        CreateNodes(bassNodes, 0f, subdivision);

        SetNodesFromScore(treble, trebleNodes);
        SetNodesFromScore(bass, bassNodes);

        StartCoroutine(IEUpdateScore(updateRate, treble, trebleNodes));
        StartCoroutine(IEUpdateScore(updateRate, bass, bassNodes));
    }

    void CreateNodes(NoteNode[] nodes, float yOffset, float subdivision) {
        for (int i = 0; i < bars * 4f / subdivision; i++) {
            NoteNode newNoteNode = Instantiate(noteNode, new Vector3(i * scale, yOffset, 0f) + noteNode.transform.position, Quaternion.identity, transform).GetComponent<NoteNode>();
            newNoteNode.gameObject.SetActive(true);
            nodes[i] = newNoteNode;
        }
    }

    IEnumerator IEUpdateScore(float delay, Clef clef, NoteNode[] nodes) {
        yield return new WaitForSeconds(delay);

        float subdivision = LengthMultipliers[NoteLength.EIGTH];
        clef.tones = new List<Tone>();
        clef.lengths = new List<NoteLength>();

        int j = 0;
        for (int i = 0; i < bars * 4f / subdivision; i++) {
            // Tone tone = noteNodes[i].isActive ? Tone.P1 : Tone.REST;
            if (j == 0) {
                nodes[i].gameObject.SetActive(true);
                clef.tones.Add(nodes[i].tone);
                clef.lengths.Add(nodes[i].length);
                j = nodes[i].skipCount;
            }
            else {
                nodes[i].gameObject.SetActive(false);
                j--;
            }
        }
        StartCoroutine(IEUpdateScore(updateRate, clef, nodes));
        yield return null;
    }

    public void SetNodesFromScore(Clef clef, NoteNode[] nodes) {

        float subdivision = LengthMultipliers[NoteLength.EIGTH];

        int skip = 0;
        int index = 0;

        for (int i = 0; i < bars * 4f / subdivision; i++) {

            if (skip == 0) {
                // find the tone in the major scale
                for (int j = 0; j < MajorScale.Length; j++) {
                    if ((int)clef.tones[index] == (int)MajorScale[j]) {
                        nodes[i].UpdateTone(j);
                        break;
                    }
                }
                skip = (int)(Score.LengthMultipliers[clef.lengths[index]] / subdivision) - 1;
                nodes[i].length = clef.lengths[index];
                index++;
            }
            else {
                nodes[i].UpdateTone(0);
                nodes[i].length = NoteLength.EIGTH;
                skip--;
            }

        }

    }

    public static string path = "DataFiles/Scores/";

    public void Save(string stream) {
        List<int[][]> channels = new List<int[][]>();

        int root = (int)this.root;
        int bars = this.bars;

        int[] settings = new int[] { root, bars };

        int[] trebleTones = new int[this.treble.tones.Count];
        int[] trebleLengths = new int[this.treble.lengths.Count];
        for (int i = 0; i < trebleTones.Length; i++) {
            trebleTones[i] = (int)this.treble.tones[i];
            trebleLengths[i] = (int)this.treble.lengths[i];
        }

        int[] bassTones = new int[this.bass.tones.Count];
        int[] bassLengths = new int[this.bass.lengths.Count];
        for (int i = 0; i < bassTones.Length; i++) {
            bassTones[i] = (int)this.bass.tones[i];
            bassLengths[i] = (int)this.bass.lengths[i];
        }

        int[][] saveData = new int[][] { settings, trebleTones, trebleLengths, bassTones, bassLengths };
        channels.Add(saveData);

        IO.SaveCSV(channels, path, stream);
    }

    //public GameObject noteNode;
    //public NoteNode[] trebleNodes;
    //public NoteNode[] bassNodes;

    public void Open(string stream, bool setNodes = true) {

        List<int[][]> channels = IO.OpenCSV(path, stream);

        int[][] saveData = channels[0];

        int[] settings = saveData[0]; 
        this.root = (Note)settings[0];
        this.bars = settings[1];

        List<Tone> trebleTones = new List<Tone>();
        List<NoteLength> trebleLengths = new List<NoteLength>();

        for (int i = 0; i < saveData[1].Length; i++) {
            trebleTones.Add((Tone)saveData[1][i]);
            trebleLengths.Add((NoteLength)saveData[2][i]);
        }

        this.treble = new Clef(trebleTones, trebleLengths);

        List<Tone> bassTones = new List<Tone>();
        List<NoteLength> bassLengths = new List<NoteLength>();

        for (int i = 0; i < saveData[3].Length; i++) {
            bassTones.Add((Tone)saveData[3][i]);
            bassLengths.Add((NoteLength)saveData[4][i]);
        }

        this.bass = new Clef(bassTones, bassLengths);

        if (setNodes) {
            SetNodesFromScore(treble, trebleNodes);
            SetNodesFromScore(bass, bassNodes);
        }

    }

    /* --- Hide this stuff --- */

    // class not struct because i want this to be by reference.
    public class Clef {
        public List<Tone> tones;
        public List<NoteLength> lengths;

        public Clef(List<Tone> _tones, List<NoteLength> _lengths) {
            tones = _tones;
            lengths = _lengths;
        }
    }

    public enum NoteLength {
        WHOLE, HALF, QUARTER, EIGTH, noteLengthCount, SIXTEENTH
    }

    public enum Tone {
        REST, P1, m2, M2, m3, M3, P4, b5, P5, m6, M6, m7, M7, P8, m9, M9, m10, M10, P11, b12, P12, toneCount
    }

    public enum Note { A, B, C, D, E, F, G, noteCount }


    public static Dictionary<NoteLength, float> LengthMultipliers = new Dictionary<NoteLength, float>() {
        {NoteLength.WHOLE, 4f },
        {NoteLength.HALF, 2f },
        {NoteLength.QUARTER, 1f },
        {NoteLength.EIGTH, 0.5f },
        {NoteLength.SIXTEENTH, 0.25f },
    };

    public static Dictionary<Tone, float> ToneMultipliers = new Dictionary<Tone, float>(){
        { Tone.REST, 0f },
        { Tone.P1 , 1f },
        { Tone.M2 , 9f/8f },
        { Tone.M3 , 5f/4f },
        { Tone.P4 , 4f/3f },
        { Tone.P5 , 3f/2f },
        { Tone.M6 , 5f/3f },
        { Tone.M7 , 15f/8f },
        { Tone.P8 , 2f },
        { Tone.M9 , 18f/8f },
        { Tone.M10 , 10f/4f },
        { Tone.P11 , 8f/3f },
        { Tone.P12 , 6f / 2f }
    };

    public static Tone[] MajorScale = new Tone[] { Tone.REST, Tone.P1, Tone.M2, Tone.M3, Tone.P4, Tone.P5, Tone.M6, Tone.M7, Tone.P8, Tone.M9, Tone.M10, Tone.P11, Tone.P12 };

    public static Dictionary<KeyCode, Tone> MajorInstrument = new Dictionary<KeyCode, Tone>(){
        // { KeyCode.Alpha0, Tone.REST },
        { KeyCode.Alpha1, Tone.P1 },
        { KeyCode.Alpha2, Tone.M2 },
        { KeyCode.Alpha3, Tone.M3 },
        { KeyCode.Alpha4, Tone.P4 },
        { KeyCode.Alpha5, Tone.P5 },
        { KeyCode.Alpha6, Tone.M6 },
        { KeyCode.Alpha7, Tone.M7 },
        { KeyCode.Alpha8, Tone.P8 },
        { KeyCode.Alpha9, Tone.M9 },
        { KeyCode.Alpha0, Tone.M10 }

    };


    public static Dictionary<Note, float> NoteFrequencies = new Dictionary<Note, float>() {
        { Note.A, 440f },
        { Note.B, 493.88f},
        { Note.C, 523.25f},
        { Note.D, 587.33f},
        { Note.E, 659.25f},
        { Note.F, 698.46f},
        { Note.G, 783.99f },
    };

    public static Clef GetTone() {

        List<Tone> tones = new List<Tone>();
        List<NoteLength> lengths = new List<NoteLength>();
        
        tones.Add(Tone.P1);
        lengths.Add(NoteLength.WHOLE);

        return new Clef(tones, lengths);

    }

    public static Clef GetRandomBar(int bars = 1, Clef clef = null) {

        float barLengthLeft = 4f * bars;
        List<Tone> tones = new List<Tone>();
        List<NoteLength> lengths = new List<NoteLength>();

        while (barLengthLeft > 0f) {

            //Tone tone = (Tone)Random.Range(0, (int)Tone.toneCount);
            //while (!MajorScale.ContainsKey(tone)) {
            //    tone = (Tone)Random.Range(0, (int)Tone.toneCount);
            //}
            Tone tone = MajorScale[Random.Range(0, 8)];
            tones.Add(tone);

            NoteLength noteLength = (NoteLength)Random.Range((int)NoteLength.HALF, (int)NoteLength.noteLengthCount);
            while (LengthMultipliers[noteLength] > barLengthLeft) {
                noteLength = (NoteLength)Random.Range((int)noteLength, (int)NoteLength.noteLengthCount);
            }

            lengths.Add(noteLength);
            barLengthLeft -= LengthMultipliers[noteLength];//
        }

        if (clef == null) {
            clef = new Clef(tones, lengths);
        }
        else {
            clef.tones = tones;
            clef.lengths = lengths;
        }
        PrintClef(clef);
        return clef;

    }

    public static Clef GetBasicBar(int bars = 1) {

        List<Tone> tones = new List<Tone>();
        List<NoteLength> lengths = new List<NoteLength>();

        for (int i = 0; i < 4 * bars; i++) {
            tones.Add(Tone.P1);
            lengths.Add(NoteLength.EIGTH);
            tones.Add(Tone.REST);
            lengths.Add(NoteLength.EIGTH);
        }
       
        Clef clef = new Clef(tones, lengths);
        PrintClef(clef);
        return clef;

    }

    public static Clef GetOffbeatBar() {

        List<Tone> tones = new List<Tone>();
        List<NoteLength> lengths = new List<NoteLength>();

        for (int i = 0; i < 4; i++) {
            tones.Add(Tone.REST);
            lengths.Add(NoteLength.EIGTH);
            tones.Add(Tone.P1);
            lengths.Add(NoteLength.EIGTH);
        }

        Clef clef = new Clef(tones, lengths);
        // PrintSheet(sheet);
        return clef;

    }

    //public static Sheet MarioTheme() {

    //    List<Tone> trebleTones = new List<Tone> { Tone.M10, Tone.M10, Tone.REST, Tone.P8, Tone.REST, Tone.M10, Tone.P12, Tone.REST, Tone.P5, Tone.REST, Tone.REST };
    //    List<Tone> bassTones = new List<Tone> { Tone.M9, Tone.M9, Tone.REST, Tone.M9, Tone.REST, Tone.M9, Tone.P12, Tone.REST, Tone.P5, Tone.REST , Tone.REST};
    //    List<NoteLength> rhythm = new List<NoteLength> { NoteLength.EIGTH, NoteLength.EIGTH, NoteLength.EIGTH, NoteLength.EIGTH,
    //                                            NoteLength.EIGTH,NoteLength.EIGTH,NoteLength.QUARTER,
    //                                            NoteLength.QUARTER, NoteLength.QUARTER, NoteLength.QUARTER, NoteLength.QUARTER,
    //                                            NoteLength.WHOLE };

    //    Clef treble = new Clef(trebleTones, rhythm);
    //    Clef bass = new Clef(bassTones, rhythm);

    //    Sheet sheet = new Sheet(treble, bass, 3);
    //    //3 + 7e 3 + 7e re 3 + 7e re 1 + 7e 3 + 7q / 5 + 7q rq 5q rq /

    //    //9 9 r 9 9 9q / 12 5
    //    // e e e e 

    //    return sheet;

    //}


    public static void PrintClef(Clef clef) {

        string _str = "";
        for (int i = 0; i < clef.tones.Count; i++) {
            _str += clef.tones[i].ToString() + " (" + clef.lengths[i] + ")" + ", ";
        }
        Debug.Log(_str);
    }

}