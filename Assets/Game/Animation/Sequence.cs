using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : MonoBehaviour {

    public Particle[] sequence;
    int index = 0;
    bool isDisposable = true;

    public void Activate(bool activate, int _index = 0) {
        index = _index;
        for (int i = 0; i < sequence.Length; i++) {
            sequence[i].Activate(true);
            sequence[i].Activate(false);
        }
        sequence[index].Activate(activate);
    }

    public void Next() {
        sequence[index].Activate(false);
        index = index + 1 % sequence.Length;
        sequence[index].Activate(true);
    }

    public void NextAndLast() {
        sequence[index].Activate(false);
        index = index + 1 % sequence.Length;
        sequence[index].FireAndDestroy();
    }

    public void InheritLayer(string layerName, int index, int discrepancy = 1) {
        for (int i = 0; i < sequence.Length; i++) {
            sequence[i].InheritLayer(layerName, index, discrepancy);
        }
    }

    void Update() {
        if (isDisposable && sequence[index] == null) {
            Destroy(gameObject);
        }
    }

}
