using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearts : MonoBehaviour {

    /* --- Components --- */
    public SpriteRenderer defaultHeartRenderer;
    SpriteRenderer[] heartRenderers = new SpriteRenderer[0];
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public State state;

    void Start() {
        SetMaxHearts(state);
    }

    void Update() {
        SetHearts(state);
    }

    /* --- Methods --- */
    void SetMaxHearts(State state) {
        // Delete the previous hearts.
        for (int i = 0; i < heartRenderers.Length; i++) {
            Destroy(heartRenderers[i].gameObject);
        }
        // Create new hearts
        heartRenderers = new SpriteRenderer[(int)state.maxHealth];
        for (int i = 0; i < heartRenderers.Length; i++) {
            SpriteRenderer heartRenderer = Instantiate(defaultHeartRenderer.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            heartRenderer.transform.localPosition = new Vector3(i, 0, 0);
            heartRenderer.gameObject.SetActive(true);
            heartRenderers[i] = heartRenderer;
        }
    }

    void SetHearts(State state) {
        for (int i = 0; i < heartRenderers.Length; i++) {
            if (i < state.health) {
                // full heart
                heartRenderers[i].sprite = fullHeart;
            }
            else {
                // emptyHeart
                heartRenderers[i].sprite = emptyHeart;
            }
        }

    }

}
