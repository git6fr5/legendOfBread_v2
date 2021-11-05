using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueDisplay : MonoBehaviour {

    public Label label;
    [ReadOnly] public int value;

    void Update() {
        GetValue();
        string text;
        if (value < 10) {
            text = "x 00" + value.ToString();
        }
        else if (value < 100) {
            text = "x 0" + value.ToString();
        }
        else if (value < 1000) {
            text = "x " + value.ToString();
        }
        else {
            text = "x OVF";
        }
        label.SetText(text);
    }

    protected virtual void GetValue() {
        //
    }
}
