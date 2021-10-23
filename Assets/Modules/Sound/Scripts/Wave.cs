using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 
/// </summary>
public class Wave : MonoBehaviour {

    public enum Shape {
        Sine,
        Square,
        Triangle
    }
    public Shape shape;

    [SerializeField] public Distribution distributionA;
    public float[] overtones;

    // Modifiers.
    [Space(5)]
    [Header("Modifiers")]
    [SerializeField] public Knob attackKnob;
    [SerializeField] [ReadOnly] public float attack;
    [SerializeField] [ReadOnly] public float maxAttack = 1f;

    [SerializeField] public Knob sustainKnob;
    [SerializeField] [ReadOnly] public float sustain;
    [SerializeField] [ReadOnly] public float maxSustain = 1f;

    [SerializeField] public Knob decayKnob;
    [SerializeField] [ReadOnly] public float decay;
    [SerializeField] [ReadOnly] public float maxDecay = 20f;

    // Wave Adders.
    [Space(5)]
    [Header("Wave Adders")]
    [SerializeField] public Knob volumeKnob;
    [SerializeField] [ReadOnly] public float volume;
    [SerializeField] [ReadOnly] protected float maxVolume = 0.5f;


    public delegate float WaveFunction(float fundamental, float time);
    WaveFunction waveFunction;

    public int octaveShift;

    float[] frequencies = new float[0];
    float[] intensities = new float[0];

    public struct NoiseSettings {

        public int i;
        public float lo;
        public float inc;
        public float mean;
        public float a;
        public float s;
        public float d;

        public NoiseSettings(int i, float lo, float inc, float[] asd, float mean = -1f) {
            this.i = i;
            this.lo = lo;
            this.inc = inc;
            this.mean = mean;
            // These are ratios
            this.a = asd[0];
            this.s = asd[1];
            this.d = asd[2];
        }

    }

    void Start() {

        /* Some cool settings,
         * at i = 20
         * lo = 50f, inc = 50f, intensities = [1] (sounds kinda like a detuned bass string) a0,s0,d0.3
         * lo = 50f, inc = 27f, intensities = [1] (sounds like a wub wub alien) a0,s0,d0.3
         * lo = 200f, inc = 27f, intensities = [1] (with a little on the attack, sounds like a growl) a0.1,s0,d0.3
         * at i = 100
         * lo = 50f, inc = 20f, intensities = [1] (very alien sounding) a0,s0,d0.3
         * lo = 54.9131f, inc = 27.13131f, intensities = [mean = 100, upperBound = Mathf.Abs(mean / (mean + (mean - frequencies[i])))], a0.25,s0.5,d0.5
         */

        NoiseSettings detunedBassString = new NoiseSettings(20, 50f, 50f, new float[] { 0f, 0f, 0.3f });
        NoiseSettings wubwubAlien = new NoiseSettings(20, 50f, 27f, new float[] { 0f, 0f, 0.3f });
        NoiseSettings growl = new NoiseSettings(20, 200f, 27f, new float[] { 0.1f, 0f, 0.3f });
        NoiseSettings zingyAlien = new NoiseSettings(100, 50f, 20f, new float[] { 0f, 0f, 0.3f });

        NoiseSettings currSettings = detunedBassString;

        if (frequencies.Length == 0) {
            frequencies = new float[currSettings.i];
            intensities = new float[currSettings.i];
            for (int i = 0; i < frequencies.Length; i++) {
                frequencies[i] = currSettings.lo + currSettings.inc * i;
                if (currSettings.mean > 0) {
                    intensities[i] = Mathf.Abs(currSettings.mean / (currSettings.mean + (currSettings.mean - frequencies[i])));
                }
                else {
                    intensities[i] = 1f;
                }
            }
        }

    }

    public void GetWave() {
        GetWaveType();
        GetValues();
    }


    public void GetWaveType() {
        switch (shape) {
            case Shape.Sine:
                waveFunction = new WaveFunction(SineFunction);
                break;
            case Shape.Square:
                waveFunction = new WaveFunction(SquareFunction);
                break;
            case Shape.Triangle:
                waveFunction = new WaveFunction(NoiseFunction);
                break;
            default:
                break;
        }
    }

    void GetValues() {
        volume = volumeKnob.value * maxVolume;
        attack = attackKnob.value * maxAttack;
        sustain = sustainKnob.value * maxSustain;
        decay = decayKnob.value * maxDecay;
        overtones = distributionA.GetValues();
    }

    public float[] Generate(int packetSize, int channels, int timeOffset, float sampleRate, float fundamental, bool shiftOctave = true, bool addModifiers = true) {

        // Get the octave.
        if (shiftOctave) {
            float octaveFactor = (octaveShift == 0) ? 1 : ((octaveShift > 0) ? Mathf.Pow(2, octaveShift) : 1f / Mathf.Pow(2, Mathf.Abs(octaveShift)));
            fundamental *= octaveFactor;
        }

        // Debug.Log(sampleRate);
        float[] wavePacket = new float[packetSize];

        // Itterate through the data.
        for (int i = 0; i < packetSize; i += channels) {
            float time = (float)(timeOffset + i) / (float)sampleRate / (float)channels;

            float value = waveFunction(fundamental, time);

            // Put that value into both the channels.
            for (int j = 0; j < channels; j++) {
                wavePacket[i + j] = value;
            }
        }

        Modify(wavePacket, channels, timeOffset, sampleRate, addModifiers);

        return wavePacket;
    }

    public float[] Modify(float[] data, int channels, int timeOffset, float sampleRate, bool addModifiers) {

        // Apply the modifiers
        for (int i = 0; i < data.Length; i += channels) {
            float time = (float)(i + timeOffset) / (float)sampleRate / (float)channels;
            float factor = volume;

            if (addModifiers) {

                if (time < attack) {
                    factor *= Mathf.Pow((time / attack), 2);
                }
                if (time > sustain) {
                    factor *= Mathf.Exp(-decay * (time - sustain));
                }
            }

            for (int j = 0; j < channels; j++) {
                data[i + j] *= factor;
            }

        }

        return data;

    }

    private float SineFunction(float fundamental, float time) {
        // Get the value for this index.
        float value = 0f;
        for (int i = 0; i < overtones.Length; i++) {
            // The factors for this overtone.
            float volumeFactor = overtones[i];
            float frequency = fundamental * (i + 1);
            value += volumeFactor * Mathf.Sin(time * 2 * Mathf.PI * frequency);
        }

        return value;
    }

    private float SquareFunction(float fundamental, float time) {
        // Get the value for this index.
        float value = 0f;
        for (int i = 0; i < overtones.Length; i++) {
            // The factors for this overtone.
            float volumeFactor = overtones[i];
            float frequency = fundamental * (i + 1);
            float period = 1 / frequency;
            value += (time % period < period / 2f) ? volumeFactor : -volumeFactor;
        }

        return value;
    }

    private float TriangleFunction(float fundamental, float time) {
        // Get the value for this index.
        float value = 0f;
        for (int i = 0; i < overtones.Length; i++) {
            // The factors for this overtone.
            float volumeFactor = overtones[i];
            float frequency = fundamental * (i + 1);
            float period = 1 / frequency;

            float sign = (time % period < period / 2f) ? 1f : -1f;
            float offset = sign * -volumeFactor;

            value += offset + sign * (time % (period / 2f) / (period / 2f)) * 2f * volumeFactor;
        }

        return value;
    }

    private float NoiseFunction(float fundamental, float time) {
        // Get the value for this index.
        float value = 0f;       

        for (int i = 0; i < frequencies.Length; i++) {
            // The factors for this overtone.
            value += intensities[i] * Mathf.Sin(time * 2 * Mathf.PI * frequencies[i]);
        }

        return value;
    }

    public float[] GenerateModifiedLine(int packetSize, float sampleRate, float scale) {

        // Debug.Log(sampleRate);
        float[] wavePacket = new float[packetSize];

        // Itterate through the data.
        for (int i = 0; i < packetSize; i++) {
            wavePacket[i] = volume * scale;
        }

        Modify(wavePacket, 1, 0, sampleRate, true);

        return wavePacket;

    }

    [System.Serializable]
    public class WaveData : Data {

        Shape shape;
        float[] overtones;
        int octaveShift;

        float attack;
        float sustain;
        float decay;

        float volume;

        public WaveData(Wave wave) {

            this.shape = wave.shape;
            this.overtones = wave.overtones;
            this.octaveShift = wave.octaveShift;

            this.attack = wave.attack;
            this.sustain = wave.sustain;
            this.decay = wave.decay;

            this.volume = wave.volume;

        }

        public void Load(Wave wave, bool isEditing) {

            wave.shape = this.shape;
            wave.GetWaveType();

            wave.overtones = this.overtones;
            wave.octaveShift = this.octaveShift;

            wave.attack = this.attack;
            wave.sustain = this.sustain;
            wave.decay = this.decay;

            wave.volume = this.volume;

            if (isEditing) {
                wave.distributionA.SetValues(wave.overtones);

                wave.attackKnob.value = wave.attack / wave.maxAttack;
                wave.sustainKnob.value = wave.sustain / wave.maxSustain;
                wave.decayKnob.value = wave.decay / wave.maxDecay;

                wave.volumeKnob.value = wave.volume / wave.maxVolume;
            }

        }

    }

}
