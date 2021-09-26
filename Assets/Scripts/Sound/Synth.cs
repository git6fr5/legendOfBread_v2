using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Note = Score.Note;
using Tone = Score.Tone;
using Value = Score.NoteLength;
using Shape = Wave.Shape;
using Parameters = Wave.Parameters;

public class Synth : MonoBehaviour {

    /* --- Components --- */
    public AudioSource audioSource;

    /* --- Settings --- */
    [Space(5)] [Header("Settings")]
    [SerializeField] [ReadOnly] public static float sampleRate; // The amount of times per second that the value of the wave is queried.
    [SerializeField] [ReadOnly] public int overtones = 5;

    [SerializeField] public Note root; // The root note of this

    /* --- Volume --- */
    [Space(5)] [Header("Volume")]
    [SerializeField] public Knob volumeKnob;
    [SerializeField] [ReadOnly] protected float volume; // The volume that this synth outputs at.
    [SerializeField] [ReadOnly] protected float maxVolume = 0.5f;

    // Wave Adders.
    [Space(5)] [Header("Wave Adders")]
    [SerializeField] public Knob factorAKnob;
    [SerializeField] [ReadOnly] public float factorA;

    [SerializeField] public Knob factorBKnob;
    [SerializeField] [ReadOnly] public float factorB;


    public Shape shapeA;
    public Shape shapeB;

    [SerializeField] public Distribution distributionA; 
    [SerializeField] [ReadOnly] public float[] overtoneDistributionA;
    [SerializeField] public Distribution distributionB;
    [SerializeField] [ReadOnly] public float[] overtoneDistributionB;

    // Modifiers.
    [Space(5)] [Header("Modifiers")]
    [SerializeField] public Knob attackKnob;
    [SerializeField] [ReadOnly] public float attack;
    [SerializeField] [ReadOnly] public float maxAttack = 1f;

    [SerializeField] public Knob sustainKnob;
    [SerializeField] [ReadOnly] public float sustain;
    [SerializeField] [ReadOnly] public float maxSustain = 1f;

    [SerializeField] public Knob decayKnob;
    [SerializeField] [ReadOnly] public float decay;
    [SerializeField] [ReadOnly] public float maxDecay = 10f;

    public int octaveShift;

    // Playback.
    [Space(5)] [Header("Playback")]
    [SerializeField] public bool isPlayable; // Determines whether the synth can be directly played or not.
    [SerializeField] [ReadOnly] protected int timeOffset = 0; // The time since at which the last note was played.
    [SerializeField] [ReadOnly] public Tone tone = Tone.REST; // The current note being played.
    [SerializeField] [ReadOnly] protected KeyCode currKey = KeyCode.None; // The current key being pressed.
    [HideInInspector] public bool newKey; // Triggers if a new key is pressed.

    void Awake() {
        sampleRate = AudioSettings.outputSampleRate;
    }

    void Update() {

        volume = volumeKnob.value * maxVolume;

        factorA = factorAKnob.value;
        factorB = factorBKnob.value;

        attack = attackKnob.value * maxAttack;
        sustain = sustainKnob.value * maxSustain;
        decay = decayKnob.value * maxDecay;

        overtoneDistributionA = distributionA.GetValues();
        overtoneDistributionB = distributionB.GetValues();

        if (isPlayable) {
            bool keyIsBeingPressed = false;
            foreach (KeyValuePair<KeyCode, Tone> key in Score.MajorInstrument) {
                if (Input.GetKey(key.Key)) {

                    tone = key.Value;

                    if (!audioSource.isPlaying) {
                        audioSource.Play();
                    }
                    keyIsBeingPressed = true;
                    if (currKey != key.Key) {
                        currKey = key.Key;
                        newKey = true;
                    }

                    break;
                }
            }

            if (!keyIsBeingPressed) {
                currKey = KeyCode.None;
                if (audioSource.isPlaying) {
                    audioSource.Stop();
                }

            }
        }
    }


    void OnAudioFilterRead(float[] data, int channels) {

        // Increment the time.
        if (newKey) {
            timeOffset = 0;
            newKey = false;
        }

        // if (octave == 0) { octaveFactor = 1; }
        // else if (octave > 0) { octaveFactor = octave + 1; }
        // else { octaveFactor = 1 / Mathf.Abs(octave - 1); }

        float octaveFactor = (octaveShift == 0) ? 1 : ((octaveShift > 0) ? Mathf.Pow(2, octaveShift) : 1f / Mathf.Pow(2, Mathf.Abs(octaveShift)));
        print(octaveFactor);

        // Play the current note.
        float fundamental = Score.NoteFrequencies[root] * Score.ToneMultipliers[tone] * octaveFactor; // Mathf.Max(1, octave + 1) / Mathf.Min(1, octave - 1);

        for (int i = 0; i < data.Length; i++) { data[i] = 0f; }
        Parameters waveA = new Parameters(shapeA, fundamental, overtones, overtoneDistributionA);
        Parameters waveB = new Parameters(shapeB, fundamental, overtones, overtoneDistributionB);

        data = AddWave(data, channels, waveA, factorA);
        data = AddWave(data, channels, waveB, factorB);
        data = AddModifiers(data, channels, timeOffset, attack, sustain, decay);

        timeOffset += (int)((float)data.Length); // / channels ??? I don't get it.

    }

    /* --- IO --- */
    public static string path = "DataFiles/Synths/";
    public static float savePrecision = 1e6f;

    public void Save(string stream) {
        List<int[][]> channels = new List<int[][]>();
        int volumeInt = (int)(volume * savePrecision);
        float[] modifierFloats = new float[] { volume, attack, sustain, decay };
        float[] factorFloats = new float[] { factorA, factorB };

        int[] octave = new int[] { octaveShift };
        int[] modifiers = ConvertToIntArray(modifierFloats);
        int[] factors = ConvertToIntArray(factorFloats);
        int[] distributionAInt = ConvertToIntArray(overtoneDistributionA);
        int[] distributionBInt = ConvertToIntArray(overtoneDistributionB);

        int[][] saveData = new int[][] { octave, modifiers, factors, distributionAInt, distributionBInt };
        channels.Add(saveData);
        IO.SaveCSV(channels, path, stream);
    }

    public void Open(string stream) {

        List<int[][]> channels = IO.OpenCSV(path, stream);

        int[][] saveData = channels[0];

        int octave = saveData[0][0];
        float[] modifiers = ConvertToFloatArray(saveData[1]);
        float[] factors = ConvertToFloatArray(saveData[2]);
        float[] distributionAFloat = ConvertToFloatArray(saveData[3]);
        float[] distributionBFloat = ConvertToFloatArray(saveData[4]);

        octaveShift = octave;

        volumeKnob.value = modifiers[0] / maxVolume;
        attackKnob.value = modifiers[1] / maxAttack;
        sustainKnob.value = modifiers[2] / maxSustain;
        decayKnob.value = modifiers[3] / maxDecay;

        factorAKnob.value = factors[0];
        factorBKnob.value = factors[1];

        distributionA.SetValues(distributionAFloat);
        distributionB.SetValues(distributionBFloat);


    }

    int[] ConvertToIntArray(float[] floatArray) {
        int[] intArray = new int[floatArray.Length];
        for (int i = 0; i < intArray.Length; i++) {
            int newInt = (int)(floatArray[i] * savePrecision);
            intArray[i] = newInt;
        }
        return intArray;
    }

    float[] ConvertToFloatArray(int[] intArray) {
        float[] floatArray = new float[intArray.Length];
        for (int i = 0; i < floatArray.Length; i++) {
            float newFloat = ((float)intArray[i]) / savePrecision;
            floatArray[i] = newFloat;
        }
        return floatArray;
    }

    public float[] AddWave(
        float[] dataPacket,
        int channels,
        Parameters parameters,
        float factor ) {

        float[] wavePacket = Wave.GetWave(dataPacket.Length, channels, timeOffset, parameters);
        // If we wanted to add modifiers to individual waves, we'd ideally do it at this point.
        // Something like :
        // AddModifiers(wavePacket ... )
        // And then the modified wavePacket to the dataPacket.

        for (int i = 0; i < dataPacket.Length; i++) {
            dataPacket[i] += volume * factor * wavePacket[i];
        }

        return dataPacket;
    }

    public float[] AddModifiers(float[] data, int channels, int timeOffset, float attack, float sustain, float decay, float sampleRate = -1f) {

        if (sampleRate == -1f) {
            sampleRate = Synth.sampleRate;
        }

        // Apply the modifiers
        for (int i = 0; i < data.Length; i += channels) {
            float time =  (float)(i + timeOffset) / (float)sampleRate / (float)channels;
            float factor = 1f;

            if (time < attack) {
                factor *= Mathf.Pow((time / attack), 2);
            }
            if (time > sustain) {
                factor *= Mathf.Exp(-decay * (time - sustain));
            }

            for (int j = 0; j < channels; j++) {
                data[i + j] *= factor;
            }

        }

        return data;

    }

}