using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class SetSelector : DungeonSelector {

    public LDtkComponentProject lDtkData;

    protected override void Select() {
        room.lDtkData = lDtkData;
    }

    protected override bool HighlightCondition() {
        return (room.lDtkData == lDtkData);
    }

}
