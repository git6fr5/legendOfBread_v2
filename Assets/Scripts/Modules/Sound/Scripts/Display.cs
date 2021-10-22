using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Parameters = Wave.Parameters;

[RequireComponent(typeof(BoxCollider2D))]
public class Display : MonoBehaviour {

    public Synth synth;
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

        float fundamental = Score.NoteFrequencies[synth.root];
        float sampleRate = periodDisplay ? samples * fundamental : samples * timeInterval;

        Parameters waveA = new Parameters(synth.shapeA, fundamental, synth.overtones, synth.overtoneDistributionA); // synth.overtoneDistributionA
        Parameters waveB = new Parameters(synth.shapeB, fundamental, synth.overtones, synth.overtoneDistributionB);

        float[] waveAValues = Wave.GetWave(samples, 1, 0, waveA, sampleRate);
        float[] waveBValues = Wave.GetWave(samples, 1, 0, waveB, sampleRate);

        float[] yValues = new float[samples];
        if (!periodDisplay) {
            for (int i = 0; i < samples; i++) {
                waveAValues[i] = 1.25f;  Mathf.Abs(waveAValues[i]);
                waveBValues[i] = 1.25f;  Mathf.Abs(waveBValues[i]);
            }
        }
        for (int i = 0; i < samples; i++) {
            yValues[i] = synth.factorA * waveAValues[i] + synth.factorB * waveBValues[i];
        }
        if (!periodDisplay) {
            yValues = synth.AddModifiers(yValues, 1, 0, synth.attack, synth.sustain, synth.decay, sampleRate);
        }

        for (int i = 0; i < samples; i++) {
            points[i].transform.position = new Vector3(i * scale / samples + offset.x, offset.y + 0.5f * yValues[i], offset.z) + transform.position;

        }

    }

}
