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
    public Dictionary<string, Controller[]> controllerDirectory = new Dictionary<string, Controller[]>();
    public Exitbox[] exitDirectory = new Exitbox[0];


    void Start() {
        map.Open(mapfile);
        mapRooms = IO.ReadListFile();
        LoadRoom(id);
    }

    /* --- FILES --- */
    public void LoadRoom(int[] newID) {
        // Deload the previous room.
        DeloadRoom();
        // Load the new room.
        id = newID;
        (int[], int) roomData = GetRequiredIdentifiersFromMap(id);
        // Opens a room that matches the criteria from the room data.
        OpenMatchingRoom(roomData.Item1);
        // Rotates the room to the correct orientation.
        RotateRoom(roomData.Item2);
        // Prints the tiles.
        room.PrintRoom();
        // Loads the objects.
        LoadRoomObjects();      
    }

    public void DeloadRoom() {
        string str_id = GetIDString(id);
        // Delete the exits
        foreach (Exitbox exit in exitDirectory) {
            Destroy(exit.gameObject);
        }
        // Deactive the controllers
        if (controllerDirectory.ContainsKey(str_id)) {
            Janitor.LoadControllers(controllerDirectory[str_id], false);
        }
        room.Reset();
    }

    void LoadRoomObjects() {
        string str_id = GetIDString(id);
        // Load the exits.
        exitDirectory = Janitor.AddExitboxes(room.environment.exit, room.transform, room.borderGrid);
        // Check if the controllers have already been loaded
        if (controllerDirectory.ContainsKey(str_id)) {
            Janitor.LoadControllers(controllerDirectory[str_id], true);
        }
        // Otherwise load new controllers.
        else {
            Controller[] mobs = Janitor.LoadNewControllers(room.transform, room.mobGrid, room.environment.OrderedControllers(room.environment.mobs));
            Controller[] traps = Janitor.LoadNewControllers(room.transform, room.trapGrid, room.environment.OrderedControllers(room.environment.traps));
            Controller[] controllers = new Controller[mobs.Length + traps.Length];
            mobs.CopyTo(controllers, 0);
            traps.CopyTo(controllers, mobs.Length);
            controllerDirectory.Add(str_id, controllers);
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
            room.trapGrid = Geometry.RotateClockwise(room.trapGrid);
        }
    }

    public static string GetIDString(int[] id) {
        return id[0].ToString() + ", " + id[1].ToString();
    }

}
