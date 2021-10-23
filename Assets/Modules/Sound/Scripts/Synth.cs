/* --- Modules --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/* --- Enumerations --- */
using Note = Score.Note;
using Tone = Score.Tone;
using Value = Score.NoteLength;
using Shape = Wave.Shape;
//using Parameters = Wave.Parameters;

/// <summary>
/// Generates noises.
/// </summary>
public class Synth : MonoBehaviour {

    /* --- Components --- */
    public AudioSource audioSource;

    /* --- Settings --- */
    [Space(5)] [Header("Settings")]
    [SerializeField] public bool isEditing;
    [SerializeField] [ReadOnly] public static float sampleRate; // The amount of times per second that the value of the wave is queried.

    /* --- Volume --- */
    [Space(5)] [Header("Volume")]
    [SerializeField] public Knob volumeKnob;
    [SerializeField] [ReadOnly] public float volume; // The volume that this synth outputs at.
    [SerializeField] [ReadOnly] protected float maxVolume = 0.5f;

    [Space(5)] [Header("Waves")]
    public Wave waveA;
    public Wave waveB;

    // Playback.
    [Space(5)] [Header("Playback")]
    [SerializeField] public bool isPlayable; // Determines whether the synth can be directly played or not.
    [SerializeField] [ReadOnly] protected int timeOffset = 0; // The time since at which the last note was played.
    [SerializeField] [ReadOnly] public Note root; // The root note of this
    [SerializeField] [ReadOnly] public Tone tone = Tone.REST; // The current note being played.
    [SerializeField] [ReadOnly] protected KeyCode currKey = KeyCode.None; // The current key being pressed.
    [HideInInspector] public bool newKey; // Triggers if a new key is pressed.
    
    public Stream stream;
    public Camera synthCam;

    void Awake() {
        sampleRate = AudioSettings.outputSampleRate;
    }

    void Update() {

        if (isEditing) {
            volume = volumeKnob.value * maxVolume;
            waveA.GetWave();
            waveB.GetWave();
            if (stream.isActive) {
                isPlayable = false;
            }
            else {
                isPlayable = true;
            }
        }

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

        // Get the current note.
        float fundamental = Score.NoteFrequencies[root] * Score.ToneMultipliers[tone]; // Mathf.Max(1, octave + 1) / Mathf.Min(1, octave - 1);

        // Get the wave data.
        float[] wavePacketA = waveA.Generate(data.Length, channels, timeOffset, sampleRate, fundamental);
        float[] wavePacketB = waveB.Generate(data.Length, channels, timeOffset, sampleRate, fundamental);

        for (int i = 0; i < data.Length; i++) { 
            data[i] = volume * (wavePacketA[i] + wavePacketB[i]); 
        }

        // Increment the time.
        timeOffset += (int)((float)data.Length); // / channels ??? I don't get it.

    }

    /* --- IO --- */
    public static string path = "Synths/";
    public static string filetype = ".synth";

    [System.Serializable]
    public class SynthData : Data {

        float volume;
        Wave.WaveData waveDataA;
        Wave.WaveData waveDataB;

        public SynthData(Synth synth) {

            this.volume = synth.volume;
            waveDataA = new Wave.WaveData(synth.waveA);
            waveDataB = new Wave.WaveData(synth.waveB);

        }

        public void Load(Synth synth) {

            synth.volume = this.volume;
            waveDataA.Load(synth.waveA, synth.isEditing);
            waveDataB.Load(synth.waveB, synth.isEditing);

            if (synth.isEditing) {
                synth.volumeKnob.value = synth.volume / synth.maxVolume;
            }

        }

    }

    public void Save(string filename) {
        SynthData data = new SynthData(this);
        IO.SaveDataFile(data, path, filename, filetype);
    }

    public void Open(string filename) {
        SynthData data = IO.OpenDataFile(path, filename, filetype) as SynthData;
        if (data != null) {
            data.Load(this);
        }
        stream.text = filename;
    }


}