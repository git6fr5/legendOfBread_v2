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

    /* --- Override --- */
    // The logic to be executed when this is selected.
    protected override void Select() {
        loader.environment.borderTile = borderTile;
        loader.environment.floorSprites = floorSprites;
    }

    // A condition that determines if this selector is highlighted.
    protected override bool HighlightCondition() {
        return (loader.environment.borderTile == borderTile && loader.environment.floorSprites == floorSprites);
    }

}
