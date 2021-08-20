using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour {

    /* --- COMPONENTS --- */
    public Map map;
    public Room room;

    public Dictionary<string, int[]> roomIdentifiers;

    public int[] id = new int[2] { 0, 0 };

    public string mapfile;

    void Start() {
        map.Open(mapfile);
        roomIdentifiers = IO.ReadListFile();
        LoadRoom(id);
    }

    /* --- FILES --- */
    public void LoadRoom(int[] id) {
        int[] requiredIdentifiers = GetRequiredIdentifiersFromMap(id);
        List<string> roomNames = FindMatchingRoom(requiredIdentifiers);
        if (roomNames.Count > 0) {
            room.Open(roomNames[0]);
        }
        else {
            print("Found no matching rooms");
        }
    }

    public int[] GetRequiredIdentifiersFromMap(int[] id) {
        int[] requiredIdentifiers = new int[(int)MapEditor.CHANNEL.count];
        requiredIdentifiers[0] = map.shapeGrid[id[0]][id[1]];
        requiredIdentifiers[1] = map.exitAndRotationsGrid[id[0]][id[1]]; // Convert to exit
        requiredIdentifiers[2] = map.challengeGrid[id[0]][id[1]];
        for (int i = 0; i < requiredIdentifiers.Length; i++) {
            print((MapEditor.CHANNEL)i);
            print(requiredIdentifiers[i]);
        }
        return requiredIdentifiers;
    }

    List<string> FindMatchingRoom(int[] requiredIdentifiers) {
        List<string> matchingRooms = new List<string>();
        foreach (KeyValuePair<string, int[]> identifier in roomIdentifiers) {
            bool match = true;
            for (int i = 0; i < requiredIdentifiers.Length; i++) {
                if (identifier.Value[i] != requiredIdentifiers[i]) {
                    match = false;
                    break;
                }
            }
            if (match) {
                matchingRooms.Add(identifier.Key);
            }
        }
        return matchingRooms;
    }

}
