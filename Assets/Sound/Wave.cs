using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave {

    public enum Shape {
        Sine,
        Square
    }

    public static float[]  AddWave(
        Shape shape,
        float sampleRate,
        float[] data,
        int channels,
        int phaseIndex,
        float fundamentalFrequency,
        int overtones,
        float[] overtoneDistribution,
        float volume) {

        float[] newData = new float[data.Length];
        (float[], int) dataPacket = GetWave(shape, sampleRate, newData, channels, phaseIndex, fundamentalFrequency, overtones, overtoneDistribution, volume);
        for (int i = 0; i < data.Length; i++) {
            data[i] += dataPacket.Item1[i];
        }

        return data;
    }

    public static (float[], int) GetWave(
        Shape shape, 
        float sampleRate,
        float[] data,
        int channels,
        int phaseIndex,
        float fundamentalFrequency, 
        int overtones,
        float[] overtoneDistribution,
        float volume ) {

        (float[], int) dataPacket;

        switch (shape) {
            case Shape.Square:
                dataPacket = Square(sampleRate, data, channels, phaseIndex, fundamentalFrequency, overtones, overtoneDistribution, volume);
                break;
            case Shape.Sine:
                dataPacket = Sine(sampleRate, data, channels, phaseIndex, fundamentalFrequency, overtones, overtoneDistribution, volume);
                break;
            default:
                dataPacket = Sine(sampleRate, data, channels, phaseIndex, fundamentalFrequency, overtones, overtoneDistribution, volume);
                break;
        }

        return dataPacket;
    }

    public static (float[], int) Sine(float sampleRate, float[] data, int channels, int phaseIndex, float rootFrequency, int overtones, float[] overtoneDistribution, float volume) {

        // Itterate through the data.
        for (int i = 0; i < data.Length; i += channels) {
            float time = (float)phaseIndex / (float)sampleRate;

            // Get the value for this index.
            float value = 0f;
            for (int overtone = 0; overtone < overtones; overtone++) {
                // The factors for this overtone.
                float volumeFactor = (overtone < overtoneDistribution.Length) ? overtoneDistribution[overtone] : Mathf.Exp(-overtone);
                float frequency = rootFrequency * (overtone + 1);
                value += volume * volumeFactor * Mathf.Sin(time * 2 * Mathf.PI * frequency);
            }

            // Put that value into both the channels.
            for (int j = 0; j < channels; j++) {
                data[i + j] = value;
            }

            phaseIndex++;
            phaseIndex = phaseIndex % (int)(sampleRate / rootFrequency);
        }

        return (data, phaseIndex);
    }

    public static (float[], int) Square(float sampleRate, float[] data, int channels, int phaseIndex, float fundamentalFrequency, int overtones, float[] overtoneDistribution, float volume) {

        // Itterate through the data.
        for (int i = 0; i < data.Length; i += channels) {
            float time = (float)phaseIndex / (float)sampleRate;

            // Get the value for this index.
            float value = 0f;
            for (int overtone = 0; overtone < overtones; overtone++) {
                // The factors for this overtone.
                float volumeFactor = (overtone < overtoneDistribution.Length) ? overtoneDistribution[overtone] : Mathf.Exp(-overtone);
                float frequency = fundamentalFrequency * (overtone + 1);
                float period = 1 / frequency;
                value += (time % period < period / 2f) ? volume * volumeFactor : -volume * volumeFactor;
            }

            // Put that value into both the channels.
            for (int j = 0; j < channels; j++) {
                data[i + j] = value;
            }

            phaseIndex++;
            phaseIndex = phaseIndex % (int)(sampleRate / fundamentalFrequency);
        }

        return (data, phaseIndex);
    }

}
