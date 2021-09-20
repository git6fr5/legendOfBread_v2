using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave {

    public enum Shape {
        Sine,
        Square
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

        switch (parameters.shape) {
            case Shape.Square:
                return Wave.Square(packetSize, channels, timeOffset, parameters, sampleRate);
            case Shape.Sine:
                return Wave.Sine(packetSize, channels, timeOffset, parameters, sampleRate);
            default:
                return Wave.Sine(packetSize, channels, timeOffset, parameters, sampleRate);
        }

    }

    public static float[] Sine(int packetSize, int channels, int timeOffset, Parameters parameters, float sampleRate = -1f) {

        if (sampleRate == -1f) {
            sampleRate = Synth.sampleRate;
        }

        Debug.Log(sampleRate);

        // Debug.Log(sampleRate);
        float[] wavePacket = new float[packetSize];

        // Itterate through the data.
        for (int i = 0; i < packetSize; i += channels) {
            float time = (float)(timeOffset + i) / (float)sampleRate / (float) channels;

            // Get the value for this index.
            float value = 0f;
            for (int overtone = 0; overtone < parameters.overtones; overtone++) {
                // The factors for this overtone.
                float volumeFactor = (overtone < parameters.overtoneDistribution.Length) ? parameters.overtoneDistribution[overtone] : Mathf.Exp(-overtone);
                float frequency = parameters.fundamental * (overtone + 1);
                value += volumeFactor * Mathf.Sin(time * 2 * Mathf.PI * frequency);
            }

            // Put that value into both the channels.
            for (int j = 0; j < channels; j++) {
                wavePacket[i + j] = value;
            }
        }

        return wavePacket;
    }

    public static float[] Square(int packetSize, int channels, int timeOffset, Parameters parameters, float sampleRate = -1f) {

        // Cache the sample rate.
        if (sampleRate == -1f) {
            sampleRate = Synth.sampleRate;
        }
        float[] wavePacket = new float[packetSize];

        // Itterate through the data.
        for (int i = 0; i < packetSize; i += channels) {
            float time = (float)(timeOffset + i) / (float)sampleRate / (float)channels;

            // Get the value for this index.
            float value = 0f;
            for (int overtone = 0; overtone < parameters.overtones; overtone++) {
                // The factors for this overtone.
                float volumeFactor = (overtone < parameters.overtoneDistribution.Length) ? parameters.overtoneDistribution[overtone] : Mathf.Exp(-overtone);
                float frequency = parameters.fundamental * (overtone + 1);
                float period = 1 / frequency;
                value += (time % period < period / 2f) ? volumeFactor : -volumeFactor;
            }

            // Put that value into both the channels.
            for (int j = 0; j < channels; j++) {
                wavePacket[i + j] = value;
            }
        }

        return wavePacket;
    }

}
