using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class SceneSelector : MonoBehaviour {

    /* --- Variables --- */
    public string sceneString;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    public Stream fileStream;
    public Stream seedStream;

    /* --- Unity --- */
    // Runs once on compilation.
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        if (fileStream != null) {
            Dungeon.mapfile = fileStream.text;
        }
        if (seedStream != null) {
            Dungeon.seed = int.Parse(seedStream.text);
        }
        SceneManager.LoadScene(sceneString, LoadSceneMode.Single);
    }

    void OnMouseOver() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
    }
}
