using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Note = Score.Note;
using Tone = Score.Tone;
using Value = Score.NoteLength;
using Shape = Wave.Shape;

public class Synth : MonoBehaviour {

    /* --- Components --- */

    public AudioSource audioSource;
    float sampleRate;
    int endPhase = 0;

    public Note root;

    [SerializeField] public Knob BPMKnob;
    [SerializeField] [ReadOnly] protected int BPM;
    [SerializeField] [ReadOnly] protected int maxBPM = 240;
    [HideInInspector] protected float secondsPerQuarterNote;

    [SerializeField] public Knob factorAKnob;
    [SerializeField] [ReadOnly] public float factorA;

    [SerializeField] public Knob factorBKnob;
    [SerializeField] [ReadOnly] public float factorB;

    [SerializeField] [ReadOnly] protected int overtones = 5;

    public Shape shapeA;
    public Shape shapeB;

    public float[] overtoneDistributionA;
    public float[] overtoneDistributionB;

    // the sheet
    List<Tone> notes = new List<Tone>();
    List<Value> lengths = new List<Value>();
    float startTime = 0;
    int index = 0;

    public bool isInstrument;

    void Awake() {
        sampleRate = AudioSettings.outputSampleRate;
        secondsPerQuarterNote = 60f / BPM;
    }

    void Update() {

        BPM = (int)(BPMKnob.value * maxBPM);
        factorA = factorAKnob.value;
        factorB = factorBKnob.value;

        if (isInstrument) {
            BPM = 0;

            bool keyIsBeingPressed = false;
            foreach (KeyValuePair<KeyCode, Tone> key in Score.MajorInstrument) {
                if (Input.GetKey(key.Key)) {
                    lengths = new List<Value> { Value.WHOLE };
                    notes = new List<Tone> { key.Value };
                    if (!audioSource.isPlaying) {
                        audioSource.Play();
                    }
                    keyIsBeingPressed = true;
                    break;
                }
            }

            if (!keyIsBeingPressed && audioSource.isPlaying) {
                audioSource.Stop();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !audioSource.isPlaying) {
            startTime = (float)AudioSettings.dspTime;
            var _sheet = Score.GetTone();
            notes = _sheet.Item1; lengths = _sheet.Item2;
            audioSource.Play();
            print("Playing");
        }
        else if (index >= notes.Count || (audioSource.isPlaying && Input.GetKeyDown(KeyCode.Space))) {
            index = 0;
            audioSource.Stop();
            print("Finished");
        }

    }

    void OnAudioFilterRead(float[] data, int channels) {

        // increment the time
        float time = (float)AudioSettings.dspTime - startTime;
        // print(time);

        // check if we need to move to the next note
        Value length = lengths[index];
        float noteLength = Score.LengthMultipliers[length];
        if (time >= noteLength * secondsPerQuarterNote) {
            // reset the end phase of the previous note
            // endPhase = 0;
            startTime = (float)AudioSettings.dspTime;
            index++;
        }

        if (index >= notes.Count) {
            return;
        }

        // play the current note
        float freq = Score.NoteFrequencies[Note.A] * Score.MajorScale[notes[index]];

        var _data = Wave.GetWave(shapeA, sampleRate, data, channels, endPhase, freq, overtones, overtoneDistributionA, factorA);
        _data.Item1 = Wave.AddWave(shapeB, sampleRate, _data.Item1, channels, endPhase, freq, overtones, overtoneDistributionB, factorB);


        data = _data.Item1;
        endPhase = _data.Item2;
    }
}