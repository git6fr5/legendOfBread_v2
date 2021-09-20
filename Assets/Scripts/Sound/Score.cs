using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Score : MonoBehaviour {

    public Note root;

    public Clef treble;
    public Clef bass;
    public int bars;

    public void RandomScore() {
        treble = Score.GetBasicBar(bars);
        bass = Score.GetRandomBar(bars);
    }

    /* --- Hide this stuff --- */


    public struct Clef {
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

    public static Dictionary<Tone, float> MajorScale = new Dictionary<Tone, float>(){
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

    public static Clef GetRandomBar(int bars = 1) {

        float barLengthLeft = 4f * bars;
        List<Tone> tones = new List<Tone>();
        List<NoteLength> lengths = new List<NoteLength>();

        while (barLengthLeft > 0f) {

            Tone tone = (Tone)Random.Range(0, (int)Tone.toneCount);
            while (!MajorScale.ContainsKey(tone)) {
                tone = (Tone)Random.Range(0, (int)Tone.toneCount);
            }
            tones.Add(tone);

            NoteLength noteLength = (NoteLength)Random.Range((int)NoteLength.HALF, (int)NoteLength.noteLengthCount);
            while (LengthMultipliers[noteLength] > barLengthLeft) {
                noteLength = (NoteLength)Random.Range((int)noteLength, (int)NoteLength.noteLengthCount);
            }

            lengths.Add(noteLength);
            barLengthLeft -= LengthMultipliers[noteLength];//
        }

        Clef clef = new Clef(tones, lengths);
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