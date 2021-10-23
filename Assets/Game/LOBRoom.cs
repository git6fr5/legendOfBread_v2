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
        }

    }

    // Create an exit.



    // Attach the entities controllers.

    // Be able to reset the player.
}
