/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using LDtkUnity;

/* --- Definitions --- */
using LDtkRoom = LDtkUnity.Level;
using LDtkMap = LDtkUnity.Level;

/// <summary>
/// Loads a map from the lDtk Data.
/// </summary>
public class Map : Loader {

    /* --- Sub-classes --- */
    [Serializable]
    public class MapData {

        /* --- Properties --- */
        public Vector2Int entrance;
        public List<int> unlockedDoors;
        public Switch doorSwitch;
        public Dictionary<Vector2Int, Room> loc_room;

        // Tile Data
        public List<LDtkTileData> shapes;
        public List<LDtkTileData> challenges;
        public List<LDtkTileData> prizes;
        public List<LDtkTileData> doors;

        /* --- Constructors --- */
        public MapData(Vector2Int entrance, List<LDtkTileData> shapes, List<LDtkTileData> challenges, List<LDtkTileData> prizes, List<LDtkTileData> doors) {
            // Tile Data.
            this.entrance = entrance;
            this.shapes = shapes;
            this.challenges = challenges;
            this.prizes = prizes;
            this.doors = doors;

            // Initialize.
            this.unlockedDoors = new List<int>();
            this.doorSwitch = Switch.On;
            this.loc_room = new Dictionary<Vector2Int, Room>();
        }

    }

    public class RoomData {
        public Shape shape;
        public int shapeOffset;
        public Challenge challenge;
        public Prize prize;
        public List<LDtkTileData> doorData;

        public RoomData(Shape shape, int shapeOffset, Challenge challenge, Prize prize, List<LDtkTileData> doorData) {
            this.shape = shape;
            this.shapeOffset = shapeOffset;
            this.challenge = challenge;
            this.prize = prize;
            this.doorData = doorData;
        }
    }

    /* --- Static Variables --- */
    // Layer Names
    public static string ShapeLayer = "Shape";
    public static string ChallengeLayer = "Challenge";
    public static string DoorLayer = "Door_Item";
    public static string PrizeLayer = "Key";

    // Grid Sizes
    public static int DoorGridSize = 2;

    /* --- Enumerations --- */
    // The shapes a room can be.
    public enum Shape {
        Square,
        Screw,
        Corridor,
        DoubleCorridor,
        Podium,
        Island,
        Hallway
    }

    // The challenges a room can have.
    public enum Challenge {
        Entrance,
        Combat,
        Puzzle,
        Trap,
        Boss,
        None
    }

    // The prizes a room can hold (includes switches and stuff for now).
    public enum Prize {
        Key,
        Treasure,
        Lever,
        None
    }

    // The different types of doors.
    public enum Door {
        Regular,
        On,
        Off,
        Key,
        Item
    }

    // The switch state of the map.
    public enum Switch {
        On,
        Off,
        Count
    }

    // The origin location of the tiles in the source sprite.
    private Vector2Int challenge_src_origin = new Vector2Int(4, 0);
    private Vector2Int prize_src_origin = new Vector2Int(4, 1);

    /* --- Components --- */
    public Room roomBase;
    public Minimap minimap;
    // LDtk Map.
    public LDtkComponentProject lDtkMapData;
    // LDtk Rooms.
    public LDtkComponentProject[] lDtkCombatRoomData;
    public LDtkComponentProject[] lDtkPuzzleRoomData;
    public LDtkComponentProject[] lDtkTrapRoomData;
    public LDtkComponentProject[] lDtkLeverRoomData;
    public LDtkComponentProject[] lDtkKeyRoomData;

    /* --- Variables --- */
    [SerializeField] public int seed = 0;
    [SerializeField] public int id;

    /* --- Properties --- */
    [SerializeField] [ReadOnly] private int generatedSeed;
    [SerializeField] [ReadOnly] public LdtkJson mapJson;
    [SerializeField] [ReadOnly] public int height;
    [SerializeField] [ReadOnly] public int width;
    [SerializeField] [ReadOnly] public Vector2Int location;
    [SerializeField] [ReadOnly] public MapData mapData;

    /* --- Unity --- */
    public override void Start() {
        // Generate a seed.
        generatedSeed = GameRules.PrimeRandomizer(seed);
        // Open the first map.
        OpenMap(id);
    }

    /* --- Opening Maps --- */
    // Opens a map with a string id.
    public void OpenMap(string str_id) {
        int id = Int32.Parse(str_id);
        OpenMap(id);
    }

    // Opens a map with an integer id.
    private void OpenMap(int id) {
        // Get the LDtk Level data based on the id from the map.
        LDtkMap ldtkMap = GetMapByID(id);
        // Load the ldtk map.
        LoadMap(ldtkMap);
        // Open the entrance room.
        OpenRoom(mapData.entrance);
        SetMapStream(id);
    }

    private LDtkMap GetMapByID(int id) {

        // Get the json file from the LDtk Data.
        mapJson = lDtkMapData.FromJson();

        // Read the json data.
        height = (int)(mapJson.DefaultLevelHeight / mapJson.DefaultGridSize);
        width = (int)(mapJson.DefaultLevelWidth / mapJson.DefaultGridSize);

        // Grab the level by the id.
        if (id < mapJson.Levels.Length && id >= 0) {
            return mapJson.Levels[id];
        }
        print("Could not find map");
        return null;
    }

    void LoadMap(LDtkMap ldtkMap) {

        // Load the shape data.
        List<LDtkTileData> shapes = LoadLayer(ldtkMap, ShapeLayer, DefaultGridSize);

        // Load the challenge data.
        List<LDtkTileData> challenges = LoadLayer(ldtkMap, ChallengeLayer, DefaultGridSize);

        // Load the prize data.
        List<LDtkTileData> prizes = LoadLayer(ldtkMap, PrizeLayer, DefaultGridSize);

        // Load the door data.
        List<LDtkTileData> doors = LoadLayer(ldtkMap, DoorLayer, DoorGridSize);

        // Set up the properties of the map.
        Vector2Int entrance = FindEntrance(challenges);

        // Set up the map.
        mapData = new MapData(entrance, shapes, challenges, prizes, doors);
        minimap.Refresh(entrance, mapData, height, DefaultGridSize, DoorGridSize);
    }

    protected void SetMapStream(int id) {
        if (stream != null) {
            stream.SetText(id.ToString());
        }
        this.id = id;
    }

    /* --- Opening Rooms --- */
    public override void OpenRoom(int id) {
        OpenRoom(location);
    }

    public void OpenRoom(Vector2Int newLocation) {

        print("Opening Room");

        // Deactivate the previous room.
        if (room != null) { room.gameObject.SetActive(false); }

        if (mapData.loc_room.ContainsKey(newLocation)) {
            OpenPreloadedRoom(newLocation);
        }
        else {
            ReloadRoom(newLocation);
        }

        LoadExtra(newLocation);

    }

    public void LoadExtra(Vector2Int newLocation) {
        // Load the doors for the room.
        LoadDoors(newLocation);

        // Update the minimap.
        location = newLocation;
        minimap.Refresh(location, mapData, height, DefaultGridSize, DoorGridSize);
        OnLoad.Invoke();
    }

    public void ReloadRoom(Vector2Int newLocation) {

        if (mapData.loc_room.ContainsKey(newLocation)) {
            Destroy(mapData.loc_room[newLocation].gameObject);
            mapData.loc_room.Remove(newLocation);
        }

        // Instantiate a new room.
        room = Instantiate(roomBase, roomBase.transform.position, Quaternion.identity, transform).GetComponent<Room>();
        room.gameObject.SetActive(true);

        // Find a matching LDtk set to pick a room from.
        RoomData roomData = GetRoomDataAtLocation(newLocation);
        lDtkData = FindMatchingLDtkSet(roomData);

        if (lDtkData != null) {

            // Pick a room from the LDtk set using the randomizer from the seed.
            LdtkJson roomJson = lDtkData.FromJson();
            int id = GameRules.PrimeRandomizer(newLocation.x + newLocation.y) % roomJson.Levels.Length;
            if (id == 0) { id = (id + 1) % roomJson.Levels.Length; }

            // Load the room.
            LDtkRoom ldtkRoom = GetRoomByID(lDtkData, id);
            LoadRoom(ldtkRoom);

        }
        else {
            room.height = 7; room.width = 7;
            LoadRoom(null);
        }

        // Set the shape.
        Geometry.Carve(roomData.shape, roomData.shapeOffset, room.borderMap, room.height, room.width);
        Geometry.Carve(roomData.shape, roomData.shapeOffset, room.floorMap, room.height, room.width);

        // Add it to the list of loaded rooms.
        mapData.loc_room.Add(newLocation, room);
    }

    private void OpenPreloadedRoom(Vector2Int newLocation) {
        // Load the previously loaded room.
        print("loading previously loaded room");
        mapData.loc_room[newLocation].gameObject.SetActive(true);
        room = mapData.loc_room[newLocation];

        // Refresh the tileset.
        environment.RefreshTiles();
        LoadTiles(room.borderMap, environment.borderTile, room.height, room.width);
        LoadTiles(room.floorMap, environment.floorTile, room.height, room.width);
        ResetDoors();
    }

    private Vector2Int FindEntrance(List<LDtkTileData> challenges) {
        for (int i = 0; i < challenges.Count; i++) {
            // Check the challenges for the entrance room.
            Vector2Int vectorID = (Vector2Int)(challenges[i].vectorID - challenge_src_origin);
            Challenge challenge = vectorID.y == 0 ? (Challenge)(vectorID.x) : Challenge.None;
            if (challenge == Challenge.Entrance) {
                return challenges[i].gridPosition;
            }
        }
        return Vector2Int.zero;
    }

    private RoomData GetRoomDataAtLocation(Vector2Int location) {

        Shape shape = Shape.Square;
        int offset = 0;
        Vector2Int? shapeID = FindInLayerAtLocation(location, mapData.shapes);
        if (shapeID != null) {
            shape = (Shape)((Vector2Int)shapeID).y;
            offset = ((Vector2Int)shapeID).x;
        }

        Challenge challenge = Challenge.None;
        Vector2Int? challengeID = FindInLayerAtLocation(location, mapData.challenges);
        if (challengeID != null) {
            challenge = (Challenge)((Vector2Int)challengeID - challenge_src_origin).x;
        }

        Prize prize = Prize.None;
        Vector2Int? prizeID = FindInLayerAtLocation(location, mapData.prizes);
        if (prizeID != null) {
            prize = (Prize)((Vector2Int)prizeID - prize_src_origin).x;
        }

        List<LDtkTileData> doorData = new List<LDtkTileData>(); // LoadDoorLayer(map, location);

        return new RoomData(shape, offset, challenge, prize, doorData);
    }

    public List<int> unlockedExits = new List<int>();

    private void ResetDoors() {
        for (int i = 0; i < room.exits.Count; i++) {
            if (room.exits[i] != null) {
                Destroy(room.exits[i].gameObject);
            }
        }
        room.exits = new List<Exit>();
    }

    private void LoadDoors(Vector2Int location) {

        // Get the appropriate adjacent locations.
        Vector2Int leftLocation = new Vector2Int(location.x - 1, location.y);
        Vector2Int upLocation = new Vector2Int(location.x, location.y - 1);

        for (int i = 0; i < mapData.doors.Count; i++) {

            if (mapData.doors[i].gridPosition == location && mapData.doors[i].offsetPosition.y == 7) {
                Vector2Int offsetPosition = new Vector2Int(mapData.doors[i].offsetPosition.x, mapData.doors[i].offsetPosition.y - 8);
                LDtkTileData doorData = new LDtkTileData(mapData.doors[i].vectorID, mapData.doors[i].gridPosition, offsetPosition, mapData.doors[i].index, 2);
                room.AddDoor(mapData.doorSwitch, doorData, mapData.unlockedDoors);
            }

            if (mapData.doors[i].gridPosition == location && mapData.doors[i].offsetPosition.x == 7) {
                // offsetPosition = offsetPosition / height * (height + 2)
                Vector2Int offsetPosition = new Vector2Int(mapData.doors[i].offsetPosition.x, 6 - mapData.doors[i].offsetPosition.y);
                LDtkTileData doorData = new LDtkTileData(mapData.doors[i].vectorID, mapData.doors[i].gridPosition, offsetPosition, mapData.doors[i].index, 3);
                room.AddDoor(mapData.doorSwitch, doorData, mapData.unlockedDoors);
            }

            if (mapData.doors[i].gridPosition == upLocation && mapData.doors[i].offsetPosition.y == 7) {
                LDtkTileData doorData = new LDtkTileData(mapData.doors[i].vectorID, mapData.doors[i].gridPosition, mapData.doors[i].offsetPosition, mapData.doors[i].index, 0);
                room.AddDoor(mapData.doorSwitch, doorData, mapData.unlockedDoors);
            }

            else if (mapData.doors[i].gridPosition == leftLocation && mapData.doors[i].offsetPosition.x == 7) {
                Vector2Int offsetPosition = new Vector2Int(mapData.doors[i].offsetPosition.x - 8, 6 - mapData.doors[i].offsetPosition.y);
                LDtkTileData doorData = new LDtkTileData(mapData.doors[i].vectorID, mapData.doors[i].gridPosition, offsetPosition, mapData.doors[i].index, 1);
                room.AddDoor(mapData.doorSwitch, doorData, mapData.unlockedDoors);
            }

        }
    }

    private Vector2Int? FindInLayerAtLocation(Vector2Int location, List<LDtkTileData> data) {
        for (int i = 0; i < data.Count; i++) {
            if (data[i].gridPosition == location) {
                return (Vector2Int?)data[i].vectorID;
            }
        }
        return null;
    }

    private LDtkComponentProject FindMatchingLDtkSet(RoomData info) {

        int seedValue = Int32.Parse(generatedSeed.ToString().Substring(0, 2));
        int index;

        // Do prizes first i guess...
        switch (info.prize) {
            case (Prize.Lever):
                index = GameRules.PrimeRandomizer(seedValue) % lDtkLeverRoomData.Length;
                return lDtkLeverRoomData[index];
            case (Prize.Key):
                index = GameRules.PrimeRandomizer(seedValue) % lDtkKeyRoomData.Length;
                return lDtkKeyRoomData[index];
            default:
                break;
        }

        switch (info.challenge) {
            case (Challenge.Combat):
                index = GameRules.PrimeRandomizer(seedValue) % lDtkCombatRoomData.Length;
                return lDtkCombatRoomData[index];
            case (Challenge.Puzzle):
                index = GameRules.PrimeRandomizer(seedValue + 1) % lDtkPuzzleRoomData.Length;
                return lDtkPuzzleRoomData[index];
            case (Challenge.Trap):
                index = GameRules.PrimeRandomizer(seedValue + 2) % lDtkTrapRoomData.Length;
                return lDtkTrapRoomData[index];
            default:
                break;
        }

        return null;
    }
}
