using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Sprite Tile", menuName = "Tiles/Sprite Tile")]
public class SpriteTile : Tile {

    public Sprite newSprite;

    public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData) {
        //    Change Sprite
        tileData.sprite = newSprite;
    }
}
