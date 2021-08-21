using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SHAPE = Geometry.SHAPE;
using CHALLENGE = Room.CHALLENGE;
using DIRECTION = Compass.DIRECTION;
using EXIT = Compass.EXIT;

public class Dungeon : MonoBehaviour {

    /* --- COMPONENTS --- */
    public Map map;
    public Room room;
    public string mapfile;


    /* --- Variables --- */
    [HideInInspector] public int[] id = new int[2] { 0, 0 };
    // Loaded objects
    public Dictionary<string, int[]> mapRooms;
    public Dictionary<string, GameObject[]> loadedObjects = new Dictionary<string, GameObject[]>();
    public Exit[] loadedExits = new Exit[0];


    void Start() {
        map.Open(mapfile);
        mapRooms = IO.ReadListFile();
        LoadRoom(id);
    }

    /* --- FILES --- */
    public void LoadRoom(int[] newID) {

        DeloadRoom();

        (int[], int) roomData = GetRequiredIdentifiersFromMap(newID);
        OpenMatchingRoom(roomData.Item1);
        RotateRoom(roomData.Item2);
        LoadRoomObjects(newID);      

        id = newID;
        room.PrintRoom();
    }

    public void DeloadRoom() {
        string str_id = GetIDString(id);
        if (loadedObjects.ContainsKey(str_id)) {
            for (int i = 0; i < loadedObjects[str_id].Length; i++) {
                if (loadedObjects[str_id][i] != null) {
                    loadedObjects[str_id][i].gameObject.SetActive(false);
                }
            }
        }
        foreach (Exit exit in loadedExits) {
            Destroy(exit.gameObject);
        }
        room.Reset();

    }

    public (int[], int) GetRequiredIdentifiersFromMap(int[] id) {
        int[] requiredIdentifiers = new int[(int)MapEditor.CHANNEL.count];
        requiredIdentifiers[0] = map.shapeGrid[id[0]][id[1]];

        DIRECTION direction = (DIRECTION)map.exitAndRotationsGrid[id[0]][id[1]];
        (EXIT, int) exitAndRotation = Compass.DirectionToExitAndRotations(direction);
        requiredIdentifiers[1] = (int)exitAndRotation.Item1; // Convert to exit
        int rotations = exitAndRotation.Item2;

        requiredIdentifiers[2] = map.challengeGrid[id[0]][id[1]];

        return (requiredIdentifiers, rotations);
    }

    public void CreateDefaultRoom(int[] identifiers) {
        room.shape = (SHAPE)identifiers[0];
        room.exits = (EXIT)identifiers[1];
        room.challenge = (CHALLENGE)identifiers[2];
        room.Construct();
    }

    void OpenMatchingRoom(int[] requiredIdentifiers) {
        List<string> roomNames = FindMatchingRoom(requiredIdentifiers);
        if (roomNames.Count > 0) {
            room.Open(roomNames[0]);
        }
        else {
            CreateDefaultRoom(requiredIdentifiers);
        }
    }

    List<string> FindMatchingRoom(int[] requiredIdentifiers) {
        List<string> matchingRooms = new List<string>();
        foreach (KeyValuePair<string, int[]> identifier in mapRooms) {
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

    void RotateRoom(int rotations) {
        for (int i = 0; i < rotations; i++) {
            room.borderGrid = Geometry.RotateClockwise(room.borderGrid);
            room.borderGrid = Janitor.RotateExitID(room.borderGrid);
            room.mobGrid = Geometry.RotateClockwise(room.mobGrid);
        }
    }

    void LoadRoomObjects(int[] newID) {
        loadedExits = Janitor.AddExitObjects(room.environment.exit, room.transform, room.borderGrid);

        string idString = newID[0].ToString() + ", " + newID[1].ToString();
        if (loadedObjects.ContainsKey(idString)) {
            for (int i = 0; i < loadedObjects[idString].Length; i++) {
                if (loadedObjects[idString][i] != null && !loadedObjects[idString][i].state.isDead) {
                    loadedObjects[idString][i].gameObject.SetActive(true);
                }
            }
        }
        else {
            Controller[] roomControllers = Janitor.LoadControllers(room.transform, room.mobGrid, room.environment.OrderedControllers(room.environment.mobs));
            loadedObjects.Add(idString, roomControllers);
        }
    }

    public static string GetIDString(int[] id) {
        return id[0].ToString() + ", " + id[1].ToString();
    }

}
