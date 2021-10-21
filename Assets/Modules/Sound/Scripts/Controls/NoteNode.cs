using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NoteLength = Score.NoteLength;
using Tone = Score.Tone;

[RequireComponent(typeof(BoxCollider2D))]
public class NoteNode : MonoBehaviour {

    public SpriteRenderer spriteRenderer;

    public Sprite active;
    public Sprite inactive;

    public bool isActive = false;

    public Tone tone = Tone.REST;
    public NoteLength length = NoteLength.EIGTH;

    BoxCollider2D boxCollider;
    Vector3 origin;

    void Awake() {
        // spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        origin = transform.position;
    }

    void OnMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(IEActive(0.1f));
        }
    }

    IEnumerator IEActive(float delay) {
        yield return new WaitForSeconds(delay);
        isActive = true;
        yield return null;
    }

    public static float scale = 2f/16f;
    public int skipCount = 0;
    public int toneIndex = 0;

    void Update() {

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)) {
            
            isActive = false;
            
        }

        if (isActive) {

            if (Input.GetKeyDown(KeyCode.W)) {
                toneIndex += 1;
                if (toneIndex >= Score.MajorScale.Length) {
                    toneIndex = Score.MajorScale.Length - 1;
                }
                tone = Score.MajorScale[toneIndex];
            }
            else if (Input.GetKeyDown(KeyCode.S)) {
                toneIndex -= 1;
                if (toneIndex < 0) {
                    toneIndex = 0;
                }
                tone = Score.MajorScale[toneIndex];
            }

            if (Input.GetKeyDown(KeyCode.A)) {
                int newLength = (int)length + 1;
                if (newLength > (int)NoteLength.noteLengthCount - 1) {
                    newLength = (int)NoteLength.noteLengthCount - 1;
                }
                length = (NoteLength)newLength;
            }
            else if (Input.GetKeyDown(KeyCode.D)) {
                int newLength = (int)length - 1;
                if (newLength < 0) {
                    newLength = 0;
                }
                length = (NoteLength)newLength;
            }

        }

        float subdivision = Score.LengthMultipliers[NoteLength.EIGTH];
        skipCount = (int)(Score.LengthMultipliers[length] / subdivision) - 1;

        transform.position = origin + new Vector3(0f, toneIndex * scale, 0f);
        spriteRenderer.sprite = (tone == Tone.REST) ? inactive : active;

        spriteRenderer.transform.localScale = new Vector3(skipCount + 1f - skipCount * a / 16f, 1f - skipCount / 16f, 1f);
        spriteRenderer.transform.localPosition = new Vector3(skipCount / 4f - skipCount * b/16f, 0, 0);

        boxCollider.size =  new Vector2(0.35f * (skipCount + 1f - skipCount * a / 16f), 0.5f * (1f - skipCount / 16f));
        boxCollider.offset = new Vector2(skipCount / 4f - skipCount * b / 16f, 0f);

    }

    public float a;
    public float b;

    public void UpdateTone(int i) {
        toneIndex = i;
        tone = Score.MajorScale[i];
    }

}
