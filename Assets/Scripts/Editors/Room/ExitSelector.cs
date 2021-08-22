using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using EXIT = Compass.EXIT;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ExitSelector : MonoBehaviour {

    /* --- Events --- */
    [System.Serializable] public class ExitEvent : UnityEvent<EXIT> { }
    public ExitEvent OnSelect;

    /* --- Components --- */
    public Sprite[] sprites;

    /* --- Variables --- */
    public EXIT exits;
    SpriteRenderer spriteRenderer;
    Room room;

    /* --- Unity --- */

    void Awake() {
        room = GameObject.FindWithTag(GameRules.roomTag).GetComponent<Room>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(exits);
    }

    void Update() {
        if ((int)room.exits < sprites.Length) {
            spriteRenderer.sprite = sprites[(int)room.exits];
        }
        else {
            spriteRenderer.sprite = sprites[0];
        }
    }

}
