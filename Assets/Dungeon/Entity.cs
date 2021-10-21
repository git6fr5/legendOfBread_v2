using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public Vector2Int gridPosition;
    public Vector2Int vectorID;
    public bool requiresDirection = false;

    public Transform attachment;
    [ReadOnly] public int indexLoadedFrom;

    public void UseDirection(List<Entity> otherEntities, Vector2Int direction) {

        print("using direction");

        Fireball(otherEntities, gridPosition, direction, 0);
    }

    int maxDepth = 4;

    void Fireball(List<Entity> otherEntities, Vector2Int prevGridPosition, Vector2Int direction, int depth) {
        Vector2Int nextGridPosition = prevGridPosition - direction;

        // Needs to check it is the correct type of object that we run into.

        for (int i = 0; i < otherEntities.Count; i++) {
            // print(otherEntities[i].gridPosition);
            if (otherEntities[i].gridPosition == nextGridPosition && otherEntities[i].attachment != null) {

                print("found the target entity");

                transform.parent = otherEntities[i].attachment;
                foreach (Transform child in transform) {
                    if (child.name == "Hitbox") {
                        child.GetComponent<Hitbox>().controller = otherEntities[i].GetComponent<Controller>();
                    }
                }

                return;

            }
        }

        depth = depth + 1;
        print(depth);

        if (depth < maxDepth) {
            Fireball(otherEntities, nextGridPosition, direction, depth);
        }

    }

}
