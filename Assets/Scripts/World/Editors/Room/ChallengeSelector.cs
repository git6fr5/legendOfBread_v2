using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using CHALLENGE = Room.CHALLENGE;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ChallengeSelector : MonoBehaviour {
    
    /* --- Events --- */
    [System.Serializable] public class ChannelEvent : UnityEvent<CHALLENGE> { }
    public ChannelEvent OnSelect;

    /* --- Components --- */
    public Sprite[] sprites;

    /* --- Variables --- */
    public CHALLENGE challenge;
    SpriteRenderer spriteRenderer;
    Room room;

    /* --- Unity --- */

    void Awake() {
        room = GameObject.FindWithTag(GameRules.roomTag).GetComponent<Room>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(challenge);
    }

    void Update() {
        if ((int)room.challenge < sprites.Length) {
            spriteRenderer.sprite = sprites[(int)room.challenge];
        }
        else {
            spriteRenderer.sprite = sprites[0];
        }
    }

    void OnMouseOver() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
    }

}
