using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alphabet : MonoBehaviour {

    /* --- Components --- */
    public string ascii = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    public Sprite[] letterSprites;
    public SpriteRenderer defaultCharacterRenderer;
    public Material textMaterial;

    /* --- Variables --- */
    public Dictionary<char, Sprite> letters;
    SpriteRenderer[] characterRenderers;


    /* --- Unity --- */
    protected virtual void Awake() {
        // set the dictionary
        letters = new Dictionary<char, Sprite>();
        int length = (int)Mathf.Min(ascii.Length, letterSprites.Length);
        for (int i = 0; i < length; i++) {
            letters.Add(ascii[i], letterSprites[i]);
        }
    }

    public void SetText(string text) {
        // Delete the previous text
        if (characterRenderers != null) {
            for (int i = 0; i < characterRenderers.Length; i++) {
                Destroy(characterRenderers[i].gameObject);
            }
        }
        // Create the new characters
        characterRenderers = new SpriteRenderer[text.Length];
        for (int i = 0; i < text.Length; i++) {
            SpriteRenderer characterRenderer = Instantiate(defaultCharacterRenderer.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            characterRenderer.transform.localPosition = new Vector3(0.4f * i, 0f, 0f);
            characterRenderer.sprite = letters[text[i]];
            characterRenderer.material = textMaterial;
            characterRenderers[i] = characterRenderer;
        }
    }

}
