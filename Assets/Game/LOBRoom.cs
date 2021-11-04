using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class LOBRoom : MonoBehaviour {

    /* --- Components --- */
    [SerializeField] public Loader loader;
    [SerializeField] public Player player;
    [SerializeField] public Transform spawn;
    [SerializeField] public Block shadowBlock;

    /* --- Properties --- */
    [SerializeField] private List<Block> shadowBlocks = new List<Block>();

    /* --- Methods --- */
    public void OnLoadRoom() {

        LoadExit();
        LoadPlayer();
        LoadBorderBlocks();
        LoadEntityInfo();

    }

    private void LoadExit() {

        if (loader.GetComponent<Map>() != null) { return; }
        // loader.room.AddExit(loader.height, Level.Location.Right, 0);
        // loader.room.AddExit(loader.height, Level.Location.Left, 0);
        // loader.room.AddExit(loader.height, Level.Location.Up, 0);
        loader.room.AddDoor(Map.Switch.On, new Loader.LDtkTileData(new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(3, -1) ), new List<int>());
    }

    private void LoadBorderBlocks() {

        // Clear the previous shadow blocks.
        for (int i = 0; i < shadowBlocks.Count; i++) {
            if (shadowBlocks[i] != null) {
                Destroy(shadowBlocks[i].gameObject);
            }
        }

        // Spawn the shadow blocks.
        shadowBlocks = new List<Block>();
        int height = loader.room.height;
        for (int i = -1; i < height + 1; i++) {
            CreateShadowBlock(i, height);
            CreateShadowBlock(i, -1);
            CreateShadowBlock(height, i);
            CreateShadowBlock(-1, i);
        }

        // Group the shadow blocks together.
        for (int i = 0; i < shadowBlocks.Count; i++) {
            for (int j = 0; j < shadowBlocks.Count; j++) {
                if (i != j) {
                    shadowBlocks[i].shadowGroup.Add(shadowBlocks[j]);
                }
            }
        }
    }

    private void LoadPlayer() {
        // Reset the player position.
        if (loader.GetComponent<Map>() != null) { return; }

        player.transform.position = spawn.position;

        // Reset the player's health.
        player.state.health = player.state.maxHealth;
        player.state.vitality = State.Vitality.Healthy;
        if (player.state.vitalityTimer != null) {
            StopCoroutine(player.state.vitalityTimer);
            player.state.vitalityTimer = null;
        }

        // Enable the player.
        player.enabled = true;
        player.gameObject.SetActive(true);
    }

    private void CreateShadowBlock(int j, int i) {

        // Create the shadow block.
        Vector3 position = loader.room.GridToWorld(new Vector2Int(j, i));
        Block newShadowBlock = Instantiate(shadowBlock.gameObject, position, Quaternion.identity, transform).GetComponent<Block>();
        newShadowBlock.transform.position = position;
        shadowBlocks.Add(newShadowBlock);

        // If the tile at this block is null, make the shadow block have no collision.
        int y = loader.room.height - i - 1;
        Vector3Int tilePosition = new Vector3Int(j, y, 0);
        if (loader.room.borderMap.GetTile(tilePosition) == null) {
            newShadowBlock.mesh.frame.isTrigger = true;
        }
    }

    private void LoadEntityInfo() {

        // Collect the fireballs, and appropriately attach them to the spinners.
        for (int i = 0; i < loader.room.entities.Count; i++) {
            if (loader.room.entities[i] != null) {
                if (loader.room.entities[i].GetComponent<Attachable>() != null) {
                    Attachable attachable = loader.room.entities[i].GetComponent<Attachable>();
                    foreach (Transform child in attachable.transform) {
                        if (child.GetComponent<Hitbox>() != null) {
                            Hitbox hitbox = child.GetComponent<Hitbox>();
                            if (attachable.transform.parent?.parent?.GetComponent<Controller>() != null) {
                                hitbox.controller = attachable.transform.parent.parent.GetComponent<Controller>();
                            }
                        }
                    }
                }
            }
        }

    }
}
