using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOBRoom : MonoBehaviour
{
    public Loader loader;
    public Player player;
    public Transform origin;

    public void OnLoadRoom() {
        print("Loaded room");
        // loader.level.AddExit(loader.height, Level.Location.Right, 0);
        // loader.level.AddExit(loader.height, Level.Location.Left, 0);
        // loader.level.AddExit(loader.height, Level.Location.Up, 0);
        loader.level.AddExit(loader.level.height, Level.Location.Down, 0);

        player.transform.position = origin.position;
        player.state.health = player.state.maxHealth;
        player.state.vitality = State.Vitality.Healthy;
        if (player.state.vitalityTimer != null) {
            StopCoroutine(player.state.vitalityTimer);
            player.state.vitalityTimer = null;
        }
        player.enabled = true;
        player.gameObject.SetActive(true);

        for (int i = 0; i < loader.level.entities.Count; i++) {
            if (loader.level.entities[i].GetComponent<Attachable>() != null) {
                Attachable attachable = loader.level.entities[i].GetComponent<Attachable>();
                foreach (Transform child in attachable.transform) {
                    if (child.GetComponent<Hitbox>() != null) {
                        Hitbox hitbox = child.GetComponent<Hitbox>();
                        if (attachable.transform.parent?.parent?.GetComponent<Controller>() != null) {
                            hitbox.controller = attachable.transform.parent.parent.GetComponent<Controller>();
                        }
                    }
                }
            }

            else if (loader.level.entities[i].GetComponent<Memory>() != null) {
                Memory memory = loader.level.entities[i].GetComponent<Memory>();
                if (loader.level.entities[i].GetComponent<Spark>()) {
                    Spark spark = loader.level.entities[i].GetComponent<Spark>();

                    Vector2 direction = new Vector2(memory.direction.x, -memory.direction.y);
                    RaycastHit2D[] hits = Physics2D.RaycastAll(spark.transform.position, direction, 1f);
                    for (int j = 0; j < hits.Length; j++) {
                        if (hits[j].collider.transform.parent.GetComponent<Block>() != null) {
                            Block block = hits[j].collider.transform.parent.GetComponent<Block>();
                            spark.currentBlock = block;
                            break;
                        }
                    }
                }
            }
        }

    }

    // Create an exit.



    // Attach the entities controllers.

    // Be able to reset the player.
}
