using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score {

    public struct Sheet {
        public List<Tone> tones;
        public List<NoteLength> lengths;

        public Sheet(List<Tone> _tones, List<NoteLength> _lengths) {
            tones = _tones;
            lengths = _lengths;
        }
    }

    public enum NoteLength {
        WHOLE, HALF, QUARTER, EIGTH, noteLengthCount, SIXTEENTH
    }

    public enum Tone {
        REST, P1, m2, M2, m3, M3, P4, P5, m6, M6, m7, M7, P8, m9, M9, toneCount
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
        { Tone.P8 , 2f }
    };

    public static Dictionary<KeyCode, Tone> MajorInstrument = new Dictionary<KeyCode, Tone>(){
        { KeyCode.Alpha0, Tone.REST },
        { KeyCode.Alpha1, Tone.P1 },
        { KeyCode.Alpha2, Tone.M2 },
        { KeyCode.Alpha3, Tone.M3 },
        { KeyCode.Alpha4, Tone.P4 },
        { KeyCode.Alpha5, Tone.P5 },
        { KeyCode.Alpha6, Tone.M6 },
        { KeyCode.Alpha7, Tone.M7 },
        { KeyCode.Alpha8, Tone.P8 }
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

    public static Sheet GetTone() {

        List<Tone> tones = new List<Tone>();
        List<NoteLength> lengths = new List<NoteLength>();
        
        tones.Add(Tone.P1);
        lengths.Add(NoteLength.WHOLE);

        return new Sheet(tones, lengths);

    }

    public static Sheet GetRandomBar() {

        float barLengthLeft = 4f;
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

        Sheet sheet = new Sheet(tones, lengths);
        PrintSheet(sheet);
        return sheet;

    }

    public static Sheet GetBasicBar() {

        List<Tone> tones = new List<Tone>();
        List<NoteLength> lengths = new List<NoteLength>();

        for (int i = 0; i < 4; i++) {
            tones.Add(Tone.P1);
            lengths.Add(NoteLength.EIGTH);
            tones.Add(Tone.REST);
            lengths.Add(NoteLength.EIGTH);
        }
       
        Sheet sheet = new Sheet(tones, lengths);
        PrintSheet(sheet);
        return sheet;

    }

    public static Sheet GetOffbeatBar() {

        List<Tone> tones = new List<Tone>();
        List<NoteLength> lengths = new List<NoteLength>();

        for (int i = 0; i < 4; i++) {
            tones.Add(Tone.REST);
            lengths.Add(NoteLength.EIGTH);
            tones.Add(Tone.P1);
            lengths.Add(NoteLength.EIGTH);
        }

        Sheet sheet = new Sheet(tones, lengths);
        // PrintSheet(sheet);
        return sheet;

    }


    public static void PrintSheet(Sheet sheet) {

        string _str = "";
        for (int i = 0; i < sheet.tones.Count; i++) {
            _str += sheet.tones[i].ToString() + " (" + sheet.lengths[i] + ")" + ", ";
        }
        Debug.Log(_str);
    }

}