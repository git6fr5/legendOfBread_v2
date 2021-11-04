/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class DifficultySelector : DungeonSelector {

    /* --- Override --- */
    // The logic to be executed when this is selected.
    protected override void Select() {
        room.isDifficult = !room.isDifficult;
    }

    // A condition that determines if this selector is highlighted.
    protected override bool HighlightCondition() {
        return (room.isDifficult);
    }

}
