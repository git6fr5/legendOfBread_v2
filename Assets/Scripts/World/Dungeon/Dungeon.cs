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

    Exit[] loadedExits = new Exit[0];

    public Dictionary<string, int[]> roomIdentifiers;

    public int[] id = new int[2] { 0, 0 };

    Dictionary<string, Controller[]> loadedControllers = new Dictionary<string, Controller[]>();

    public string mapfile;

    void Start() {
        map.Open(mapfile);
        roomIdentifiers = IO.ReadListFile();
        LoadRoom(id);
    }

    /* --- FILES --- */
    public void LoadRoom(int[] newID) {

        DeloadRoom();

        (int[], int) roomData = GetRequiredIdentifiersFromMap(newID);
        int[] requiredIdentifiers = roomData.Item1;
        int rotations = roomData.Item2;
        List<string> roomNames = FindMatchingRoom(requiredIdentifiers);
        if (roomNames.Count > 0) {
            print(roomNames[0]);
            room.Open(roomNames[0]);
        }
        else {
            print("Found no matching rooms");
            CreateDefaultRoom(requiredIdentifiers);
        }

        for (int i = 0; i < rotations; i++) {
            room.borderGrid = Geometry.RotateClockwise(room.borderGrid);
        }

        loadedExits = Janitor.AddExitObjects(room.environment.exit, room.transform, room.borderGrid);

        string idString = newID[0].ToString() + ", " + newID[1].ToString();
        if (loadedControllers.ContainsKey(idString)) {
            foreach (Controller controller in loadedControllers[idString]) {
                controller.gameObject.SetActive(true);
            }
        }
        else {
            Controller[] roomControllers = Janitor.LoadControllers(room.transform, room.mobGrid, room.environment.OrderedControllers(room.environment.mobs));
            loadedControllers.Add(idString, roomControllers);
        }

        id = newID;
        room.PrintRoom();
    }

    public void DeloadRoom() {
        string idString = id[0].ToString() + ", " + id[1].ToString();
        if (loadedControllers.ContainsKey(idString)) {
            foreach (Controller controller in loadedControllers[idString]) {
                controller.gameObject.SetActive(false);
            }
        }
        foreach (Exit exit in loadedExits) {
            Destroy(exit.gameObject);
        }

    }

    public (int[], int) GetRequiredIdentifiersFromMap(int[] id) {
        int[] requiredIdentifiers = new int[(int)MapEditor.CHANNEL.count];
        requiredIdentifiers[0] = map.shapeGrid[id[0]][id[1]];

        DIRECTION direction = (DIRECTION)map.exitAndRotationsGrid[id[0]][id[1]];
        (EXIT, int) exitAndRotation = Compass.DirectionToExitAndRotations(direction);
        requiredIdentifiers[1] = (int)exitAndRotation.Item1; // Convert to exit
        int rotations = exitAndRotation.Item2;

        requiredIdentifiers[2] = map.challengeGrid[id[0]][id[1]];
        print(string.Format("{0}, {1}, {2}", requiredIdentifiers[0], requiredIdentifiers[1], requiredIdentifiers[1] ));
        print("------------------------------------------");

        return (requiredIdentifiers, rotations);
    }

    public void CreateDefaultRoom(int[] identifiers) {
        room.shape = (SHAPE)identifiers[0];
        room.exits = (EXIT)identifiers[1];
        room.challenge = (CHALLENGE)identifiers[2];
        room.Construct();
    }

    List<string> FindMatchingRoom(int[] requiredIdentifiers) {
        List<string> matchingRooms = new List<string>();
        foreach (KeyValuePair<string, int[]> identifier in roomIdentifiers) {
            print(string.Format("{0}, {1}, {2}", identifier.Value[0], identifier.Value[1], identifier.Value[1]));
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
