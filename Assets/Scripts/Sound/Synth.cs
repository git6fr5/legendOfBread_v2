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


    // Playback.
    [Space(5)] [Header("Playback")]
    [SerializeField] [ReadOnly] public float startTime = 0f; // The time since at which the last note was played.
    [SerializeField] [ReadOnly] public Tone tone = Tone.REST; // The current note being played.
    [SerializeField] [ReadOnly] protected KeyCode currKey = KeyCode.None; // The current key being pressed.
    [HideInInspector] public bool newKey; // Triggers if a new key is pressed.
    public bool isActive;

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

        if (isActive) {
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

    int timeOffset = 0;

    void OnAudioFilterRead(float[] data, int channels) {

        // Increment the time.
        if (newKey) {
            timeOffset = 0;
            startTime = (float)AudioSettings.dspTime;
            newKey = false;
        }
        // float currTime = (float)AudioSettings.dspTime - startTime;

        // Play the current note.
        float fundamental = Score.NoteFrequencies[root] * Score.MajorScale[tone];

        for (int i = 0; i < data.Length; i++) { data[i] = 0f; }
        Parameters waveA = new Parameters(shapeA, fundamental, overtones, overtoneDistributionA);
        Parameters waveB = new Parameters(shapeB, fundamental, overtones, overtoneDistributionB);

        data = AddWave(data, channels, waveA, factorA);
        data = AddWave(data, channels, waveB, factorB);
        // data = AddModifiers(data, channels, currTime, attack, sustain, decay);

        timeOffset += (int)((float)data.Length); // / channels ??? I don't get it.
        // print(timeOffset);

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

    public float[] AddModifiers(float[] data, int channels, float startTime, float attack, float sustain, float decay, float sampleRate = -1f) {

        if (sampleRate == -1f) {
            sampleRate = Synth.sampleRate;
        }

        // Apply the modifiers
        for (int i = 0; i < data.Length; i += channels) {
            float time = startTime + (float)i / (float)sampleRate / (float)channels;
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