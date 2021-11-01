/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class DungeonSelector : MonoBehaviour {

    /* --- Components --- */
    protected SpriteRenderer spriteRenderer;
    public Loader loader;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Runs once every frame.
    void Update() {
        Highlight();
    }

    // Runs when the collider is clicked.
    void OnMouseDown() {
        Select();
        Refresh();
    }

    /* --- Methods --- */
    // Refresh the loader after the selection.
    void Refresh() {
        loader.OpenRoom(loader.room.id);
    }

    // Highlights the sprite if necessary.
    void Highlight() {
        bool highlight = HighlightCondition();
        if (highlight) {
            spriteRenderer.material.SetFloat("_OutlineWidth", 0.05f);
        }
        else {
            spriteRenderer.material.SetFloat("_OutlineWidth", 0f);
        }
    }

    /* --- Virtual --- */
    // The logic to be executed when this is selected.
    protected virtual void Select() {
        // Determined by the type of the selector.
    }

    // A condition that determines if this selector is highlighted.
    protected virtual bool HighlightCondition() {
        // Determined by the type of the selector.
        return false;
    }
}
