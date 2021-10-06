using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave {

    public enum Shape {
        Sine,
        Square,
        Noise
    }

    public struct Parameters {
        public Shape shape;
        public float fundamental;
        public int overtones;
        public float[] overtoneDistribution;

        public Parameters(Shape _shape, float _fundamental, int _overtones, float[] _overtoneDistribution) {
            shape = _shape;
            fundamental = _fundamental;
            overtones = _overtones;
            overtoneDistribution = _overtoneDistribution;
        }
    }

    public static float[] GetWave(
        int packetSize,
        int channels,
        int timeOffset,
        Parameters parameters,
        float sampleRate = -1f) {

        // Cache the sample rate.
        if (sampleRate == -1f) {
            sampleRate = Synth.sampleRate;
        }

        WaveFunction waveFunction = new WaveFunction(SineFunction);
        switch (parameters.shape) {
            case Shape.Square:
                waveFunction = new WaveFunction(SquareFunction);
                break;
            case Shape.Noise:
                waveFunction = new WaveFunction(NoiseFunction);
                break;
            default:
                break;
        }

        return NewWave(waveFunction, packetSize, channels, timeOffset, parameters, sampleRate);
    }

    public delegate float WaveFunction(Parameters parameters, float time);

    public static float[] NewWave(WaveFunction waveFunction, int packetSize, int channels, int timeOffset, Parameters parameters, float sampleRate) {

        // Debug.Log(sampleRate);
        float[] wavePacket = new float[packetSize];

        // Itterate through the data.
        for (int i = 0; i < packetSize; i += channels) {
            float time = (float)(timeOffset + i) / (float)sampleRate / (float)channels;

            float value = waveFunction(parameters, time);

            // Put that value into both the channels.
            for (int j = 0; j < channels; j++) {
                wavePacket[i + j] = value;
            }
        }

        return wavePacket;
    }

    private static float SineFunction(Parameters parameters, float time) {
        // Get the value for this index.
        float value = 0f;
        for (int overtone = 0; overtone < parameters.overtones; overtone++) {
            // The factors for this overtone.
            float volumeFactor = (overtone < parameters.overtoneDistribution.Length) ? parameters.overtoneDistribution[overtone] : Mathf.Exp(-overtone);
            float frequency = parameters.fundamental * (overtone + 1);
            value += volumeFactor * Mathf.Sin(time * 2 * Mathf.PI * frequency);
        }

        return value;
    }

    private static float SquareFunction(Parameters parameters, float time) {
        // Get the value for this index.
        float value = 0f;
        for (int overtone = 0; overtone < parameters.overtones; overtone++) {
            // The factors for this overtone.
            float volumeFactor = (overtone < parameters.overtoneDistribution.Length) ? parameters.overtoneDistribution[overtone] : Mathf.Exp(-overtone);
            float frequency = parameters.fundamental * (overtone + 1);
            float period = 1 / frequency;
            value += (time % period < period / 2f) ? volumeFactor : -volumeFactor;
        }

        return value;
    }

    public static float[] distribution;

    public static float[] NoiseDistribution(int size, float u = 0.5f, float a = 0.35f) {

        distribution = new float[size];

        for (int i = 0; i < size; i++) {
            // float x = (GameRules.PrimeRandomizer(i) % 100) / 100f;
            float x = Random.Range(0f, 1f);
            distribution[i] = x;// Gaussian(x, u, a);
        }

        return distribution;
    }

    static float Gaussian(float x, float u, float a) {
        return (1 / (Mathf.Sqrt(2 * Mathf.PI) * a)) * Mathf.Exp(-Mathf.Pow((x - u) / a, 2));
    }

    private static float NoiseFunction(Parameters parameters, float time) {

        // Get the value for this index.
        float value = 0f;

        for (int i = 0; i < distribution.Length; i++) {

            float volumeFactor = 5f / (float)distribution.Length;
            float frequency = parameters.fundamental * distribution[i];
            value += volumeFactor * Mathf.Sin(time * 2 * Mathf.PI * frequency);

        }

        return value;
    }

}
