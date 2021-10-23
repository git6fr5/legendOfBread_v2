/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using Orientation = Compass.Orientation;
using SHAPE = Geometry.Shape;
using CHALLENGE = Room.CHALLENGE;
using NODE = Compass.Direction;

public class Dungeon : MonoBehaviour {

    /* --- COMPONENTS --- */
    public Map map;
    public Room room;
    public static string mapfile = "mapA";
    public static string roomListFile = "rooms";
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
        roomDirectory = IO.ReadListFile(Room.path, roomListFile);
        LoadRoom(map.entrance);
    }

    /* --- FILES --- */
    public void LoadRoom(int[] newID) {
        // Deload the previous room.
        DeloadRoom();
        // Load the new room.
        id = newID;
        (SHAPE, CHALLENGE) mapData = ParseMapData(id);
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
        LoadExits();
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

    void LoadExits() {
        // print(map.nodeGrid.Length);
        // print(map.nodeGrid[0].Length);

        List<Orientation> orientations = new List<Orientation>();
        for (int j = 0; j < map.nodeGrid.Length; j++) {
            int[] node = map.nodeGrid[j];
            bool isFirst = (node[0] == id[0] && node[1] == id[1]);
            bool isSecond = (node[2] == id[0] && node[3] == id[1]);
            if  (isFirst || isSecond) {
                print("found associated node");
                print(id[0].ToString() + ", " + id[1].ToString());
                Vector2 direction = new Vector2(node[1] - node[3], -(node[0] - node[2]));
                if (isFirst) { direction *= -1f; }
                print(direction);
                print(Compass.VectorOrientations[direction]);
                orientations.Add(Compass.VectorOrientations[direction]);
            }
        }

        room.borderGrid = Janitor.NewAddExits(room.borderGrid, orientations, room.border);
        exitDirectory = Janitor.AddExitboxes(room.environment.exit, room.transform, room.borderGrid);

    }

    /* --- MATCHING ROOMS --- */
    public (SHAPE, CHALLENGE) ParseMapData(int[] id) {
        
        SHAPE shape = (SHAPE)map.shapeGrid[id[0]][id[1]];
        CHALLENGE challenge = (CHALLENGE)map.challengeGrid[id[0]][id[1]];

        return (shape, challenge);
    }

    void OpenMatchingRoom((SHAPE, CHALLENGE) mapData) {
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
        // room.borderGrid = Janitor.AddExits(mapData.Item2, room.borderGrid, room.border);
    }

    void DefaultRoom((SHAPE, CHALLENGE) mapData) {
        room.shape = (SHAPE)mapData.Item1;
        room.challenge = (CHALLENGE)mapData.Item2;
        room.Construct();
    }

    List<string> FindMatchingRoom((SHAPE, CHALLENGE) mapData) {
        List<string> matchingRooms = new List<string>();
        foreach (KeyValuePair<string, int[]> roomEntry in roomDirectory) {
            bool shapeMatch = (int)mapData.Item1 == roomEntry.Value[0];
            bool challengeMatch = (int)mapData.Item2 == roomEntry.Value[1];
            if (shapeMatch && challengeMatch) {
                matchingRooms.Add(roomEntry.Key);
            }
        }
        return matchingRooms;
    }

}
