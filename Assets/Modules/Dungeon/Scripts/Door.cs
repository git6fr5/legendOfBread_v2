using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapSwitch = Map.Switch;
using DoorData = Loader.LDtkTileData;

public class Door : MonoBehaviour {

    int id; // The type of door this is.
    Vector2Int pathway;
    public Lock lockType;
    public SpriteRenderer lockSprite; 

    public enum Lock {
        None,
        On,
        Off,
        Key,
        Puzzle
    }

    public void Load(DoorData doorData, MapSwitch doorSwitch, List<int> unlockedDoors) {

        // Read the door data.
        id = doorData.index;
        lockType = GetLock(doorData, doorSwitch, unlockedDoors);
        pathway = GetPath(doorData);

        // Asjust the door position.
        transform.localPosition += new Vector3(pathway.x, -pathway.y, 0f);
        transform.eulerAngles = Vector3.forward * 90f * doorData.rotation;

    }

    public static Lock GetLock(DoorData doorData, MapSwitch doorSwitch, List<int> unlockedDoors) {

        if ((Lock)doorData.vectorID.x == Lock.None) {
            return Lock.None;
        }
        else if ((Lock)doorData.vectorID.x == Lock.On && doorSwitch != MapSwitch.On) {
            return Lock.Off;
        }
        else if ((Lock)doorData.vectorID.x == Lock.Off && doorSwitch != MapSwitch.Off) {
            return Lock.Off;
        }
        else if ((Lock)doorData.vectorID.x == Lock.Puzzle) { // Actually puzzle doors.
            return Lock.Puzzle;
        }
        else if ((Lock)doorData.vectorID.x == Lock.Key) {
            for (int i = 0; i < unlockedDoors.Count; i++) {
                if (doorData.index == unlockedDoors[i]) {
                    return Lock.None;
                }
            }
            return Lock.Key;
        }
        return Lock.None;
    }

    public static Vector2Int GetPath(DoorData doorData) {
        Vector2 id = (doorData.offsetPosition - new Vector2(3.5f, 3.5f));
        Vector2Int pathway;
        if (Mathf.Abs(id.y) > Mathf.Abs(id.x)) {
            pathway = new Vector2Int(0, -(int)Mathf.Sign(id.y));
        }
        else {
            pathway = new Vector2Int((int)Mathf.Sign(id.x), 0);
        }
        return pathway;
    }
}
