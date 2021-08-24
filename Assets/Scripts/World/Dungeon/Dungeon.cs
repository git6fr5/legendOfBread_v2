using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SHAPE = Geometry.SHAPE;
using CHALLENGE = Room.CHALLENGE;
using NODE = Compass.DIRECTION;

public class Dungeon : MonoBehaviour {

    /* --- COMPONENTS --- */
    public Map map;
    public Room room;
    public static string mapfile = "mapA";
    public static int seed = 1;


    /* --- Variables --- */
    [HideInInspector] public int[] id = new int[2] { 0, 0 };
    // Loaded objects
    public Dictionary<string, int[]> roomDirectory;
    public Dictionary<string, Controller[]> controllerDirectory = new Dictionary<string, Controller[]>();
    public Exitbox[] exitDirectory = new Exitbox[0];


    void Start() {
        seed = GameRules.PrimeRandomizer(seed);
        map.Open(mapfile);
        roomDirectory = IO.ReadListFile();
        LoadRoom(map.entrance);
    }

    /* --- FILES --- */
    public void LoadRoom(int[] newID) {
        // Deload the previous room.
        DeloadRoom();
        // Load the new room.
        id = newID;
        (SHAPE, NODE, CHALLENGE) mapData = ParseMapData(id);
        // Opens a room that matches the criteria from the room data.
        OpenMatchingRoom(mapData);
        // Loads the objects.
        LoadRoomObjects();
        // Prints the tiles.
        room.PrintRoom();
    }

    /* --- LOADING ROOMS --- */
    public static string GetIDString(int[] id) {
        return id[0].ToString() + ", " + id[1].ToString();
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

    /* --- MATCHING ROOMS --- */
    public (SHAPE, NODE, CHALLENGE) ParseMapData(int[] id) {
        
        SHAPE shape = (SHAPE)map.shapeGrid[id[0]][id[1]];
        NODE direction = (NODE)map.nodeGrid[id[0]][id[1]];
        CHALLENGE challenge = (CHALLENGE)map.challengeGrid[id[0]][id[1]];

        return (shape, direction, challenge);
    }

    void OpenMatchingRoom((SHAPE, NODE, CHALLENGE) mapData) {
        List<string> roomNames = FindMatchingRoom(mapData);
        int roomSeed = int.Parse(seed.ToString().Substring(2, 2));
        if (roomNames.Count > 0) {
            int index = GameRules.PrimeRandomizerID(roomSeed, id) % roomNames.Count;
            room.Open(roomNames[index]);
        }
        else {
            DefaultRoom(mapData);
        }
        room.floorGrid = Geometry.RandomizeGrid(room.size, room.size, 4, GameRules.PrimeRandomizerID(roomSeed, id));
        room.borderGrid = Janitor.AddExits(mapData.Item2, room.borderGrid, room.border);
    }

    void DefaultRoom((SHAPE, NODE, CHALLENGE) mapData) {
        room.shape = (SHAPE)mapData.Item1;
        room.challenge = (CHALLENGE)mapData.Item3;
        room.Construct();
    }

    List<string> FindMatchingRoom((SHAPE, NODE, CHALLENGE) mapData) {
        List<string> matchingRooms = new List<string>();
        foreach (KeyValuePair<string, int[]> roomEntry in roomDirectory) {
            bool shapeMatch = (int)mapData.Item1 == roomEntry.Value[0];
            bool challengeMatch = (int)mapData.Item3 == roomEntry.Value[1];
            if (shapeMatch && challengeMatch) {
                matchingRooms.Add(roomEntry.Key);
            }
        }
        return matchingRooms;
    }

}
