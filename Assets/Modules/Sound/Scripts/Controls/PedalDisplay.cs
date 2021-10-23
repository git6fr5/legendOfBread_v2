using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PedalDisplay : MonoBehaviour {

    public Wave wave;
    public Label label;

    int value = 0;

    void Start() {
        StartCoroutine(UpdateDisplay());
    }

    void Update() {
        wave.octaveShift = wave.octaveShift > 6 ? 6 : wave.octaveShift < -6 ? -6 : wave.octaveShift;
        value = wave.octaveShift;

        label.text = value.ToString();

    }

    IEnumerator UpdateDisplay() {
        yield return new WaitForSeconds(0.15f);
        label.SetText(label.text);
        yield return StartCoroutine(UpdateDisplay());
    }

}
