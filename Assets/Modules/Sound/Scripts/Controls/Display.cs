using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Parameters = Wave.Parameters;

[RequireComponent(typeof(BoxCollider2D))]
public class Display : MonoBehaviour {

    public Wave[] waves;
    static int samples = 32;
    public static float size = 3f;
    float scale;  // length - borders - pixel offset
    public Vector3 offset;
    float timeInterval = 1f;

    [SerializeField] protected float lineWidth = 0.1f; // The width of the lines.
    [Range(0.1f, 1f)] public float updateInterval;

    GameObject[] points;
    public GameObject point;
    public bool periodDisplay = true;


    void Awake() {

        scale = size - 4f / 16f - 1f / 16f;
        offset = new Vector3(-scale/2f, 0f, -1f);

        points = new GameObject[samples];
        for (int i = 0; i < samples; i++) {

            GameObject newPoint = Instantiate(point);
            newPoint.SetActive(true);
            points[i] = newPoint;

        }

        StartCoroutine(IEWaveForm(updateInterval));
    }

    void OnMouseDown() {
        periodDisplay = !periodDisplay;
    }

    IEnumerator IEWaveForm(float delay) {
        yield return new WaitForSeconds(delay);
        WaveForm();
        StartCoroutine(IEWaveForm(updateInterval));
        yield return null;
    }


    void WaveForm() {

        float fundamental = Score.NoteFrequencies[Score.Note.A];
        float sampleRate = periodDisplay ? samples * fundamental : samples * timeInterval;

        float[][] wavePackets = new float[waves.Length][];
        for (int i = 0; i < waves.Length; i++) {
            float[] wavePacket;
            if (periodDisplay) {
                wavePacket = waves[i].Generate(samples, 1, 0, sampleRate, fundamental, periodDisplay, !periodDisplay);
            }
            else {
                wavePacket = waves[i].GenerateModifiedLine(samples, sampleRate, 5f);
            }
            wavePackets[i] = wavePacket;
        }

        float[] values = new float[samples];
        for (int i = 0; i < samples; i++) {
            values[i] = 0f;
             for (int j = 0; j < waves.Length; j++) {
                values[i] += 1.25f / waves.Length * wavePackets[j][i];
            }
        }

        for (int i = 0; i < samples; i++) {
            points[i].transform.position = new Vector3(i * scale / samples + offset.x, offset.y + 0.5f * values[i], offset.z) + transform.position;

        }

    }

}
