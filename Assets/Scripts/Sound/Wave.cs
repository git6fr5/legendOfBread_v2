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

    public static float[] Sine(int packetSize, int channels, float currTime, Parameters parameters) {

        // Cache the sample rate.
        float sampleRate = Synth.sampleRate;
        // Debug.Log(sampleRate);
        float[] wavePacket = new float[packetSize];

        // Itterate through the data.
        for (int i = 0; i < packetSize; i += channels) {
            float time = currTime + (float)i / (float)Synth.sampleRate / (float) channels;

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

    public static float[] Square(int packetSize, int channels, float currTime, Parameters parameters) {

        // Cache the sample rate.
        float sampleRate = Synth.sampleRate;
        float[] wavePacket = new float[packetSize];

        // Itterate through the data.
        for (int i = 0; i < packetSize; i += channels) {
            float time = currTime + (float)i / (float)sampleRate / (float) channels;

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
