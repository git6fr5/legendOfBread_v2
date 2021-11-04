using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lock = Door.Lock;

public class LockBox : Structure {

    public Exitbox exitbox;
    Dictionary<Lock, Sprite> lock_sprite;

    public Sprite noLock;
    public Sprite keyLock;
    public Sprite offLock;
    public Sprite itemLock;

    void Awake() {
        interactAction = State.Action.Pushing;
        condition = Condition.Interactable;

        lock_sprite = new Dictionary<Room.Lock, Sprite>() {
            { Room.Lock.None, noLock },
            {Room.Lock.Key, keyLock },
            {Room.Lock.Off, offLock },
            {Room.Lock.Item, itemLock }
        };

    }

    /* --- Overridden Methods --- */

    protected override void Interactable() {
        mesh.spriteRenderer.sprite = lock_sprite[exit.lockType];
    }

    public override bool Interact(Controller controller) {


        if (controller.GetComponent<Player>()) {
            Player player = controller.GetComponent<Player>();

            if (exit.lockType == Room.Lock.Key && player.numKeys > 0) {
                player.numKeys -= 1;
                exit.lockType = Room.Lock.None;

                exit.map.mapData.unlockedDoors.Add(exit.index);

            }
        }

        return true;

    }

}
