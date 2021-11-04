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
    protected Room room;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Runs once every frame.
    void Update() {
        GetRoom();
        Highlight();
    }

    // Runs when the collider is clicked.
    void OnMouseDown() {
        if (room != null) {
            Select();
            Refresh();
        }
    }

    /* --- Methods --- */
    void GetRoom() {
        if (loader.GetComponent<Map>() != null) {
            Map map = loader.GetComponent<Map>();
            room = map.mapData.loc_room[map.location];
        }
        else if (loader.GetComponent<Room>() != null) {
            room = loader.GetComponent<Room>();
        }
    }

    // Refresh the loader after the selection.
    void Refresh() {
        room.Open(room.id);
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
