using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Note = Score.Note;

[RequireComponent(typeof(BoxCollider2D))]
public class Transpose : MonoBehaviour {

    public int increment;

    public Score score;

    void OnMouseDown() {
        if ((int)score.root == 0 && increment < 0) {
            score.root = (Note)((int)Note.noteCount - 1);
        }
        else {
            score.root = (Note)(((int)score.root + increment) % (int)Note.noteCount);
        }
    }

}
