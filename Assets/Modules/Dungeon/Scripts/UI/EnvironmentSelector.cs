/* --- Selector --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnvironmentSelector : DungeonSelector {

    /* --- Components --- */
    public RuleTile borderTile;
    public Sprite[] floorSprites;
    public Transform entityParentTransform;

    /* --- Override --- */
    // The logic to be executed when this is selected.
    protected override void Select() {
        room.environment.borderTile = borderTile;
        room.environment.floorSprites = floorSprites;
        room.environment.entityParentTransform = entityParentTransform;
    }

    // A condition that determines if this selector is highlighted.
    protected override bool HighlightCondition() {
        return (room.environment.borderTile == borderTile && room.environment.floorSprites == floorSprites);
    }

}
