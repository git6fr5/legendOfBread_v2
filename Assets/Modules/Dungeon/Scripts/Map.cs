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

/* --- Enumerations --- */
using Lock = Door.Lock;

/// <summary>
/// Loads a map from the lDtk Data.
/// </summary>
public class Map : Loader {

    /* --- Static Variables --- */
    // Layer Names
    public static string ShapeLayer = "Shape";
    public static string ChallengeLayer = "Challenge";
    public static string DoorLayer = "Door_Item";
    public static string PrizeLayer = "Key";
    // Grid Sizes
    public static int DoorGridSize = 2;

    /* --- Sub-Classes --- */
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
    public Door doorBase;
    public Transform doorParentTransform;
    public Minimap minimap;
    // LDtk Map.
    public LDtkComponentProject lDtkMapData;
    // LDtk Rooms.
    public LDtkComponentProject[] lDtkCombatRoomData;
    public LDtkComponentProject[] lDtkPuzzleRoomData;
    public LDtkComponentProject[] lDtkTrapRoomData;
    public LDtkComponentProject[] lDtkLeverRoomData;
    public LDtkComponentProject[] lDtkKeyRoomData;

    /* --- Properties --- */
    [SerializeField] [ReadOnly] public LdtkJson mapJson;
    [SerializeField] [ReadOnly] public int height;
    [SerializeField] [ReadOnly] public int width;
    [SerializeField] [ReadOnly] public Vector2Int location;
    [SerializeField] [ReadOnly] public MapData mapData;
    [SerializeField] [ReadOnly] public List<Door> doors; // The list of currently loaded exits.

    public List<int> unlockedDoors = new List<int>();


    /* --- Unity --- */
    public override void Start() {
        height = 7;
        width = 7;
        // Open the first map.
        // OpenMap(id);
    }

    /* --- Map --- */
    // Opens a map with a string id.
    public void Open(string str_id) {
        Open(Int32.Parse(str_id));
    }

    // Opens a map with an integer id.
    public void Open(int id) {
        Reset();
        // Get the LDtk Level data based on the id from the map.
        LDtkMap ldtkMap = GetLevelByID(lDtkMapData, id);
        // Load the ldtk map.
        LoadMap(ldtkMap);
        // Open the entrance room.
        OpenRoom(0, mapData.entrance);
        // Set the stream if necessary.
        SetStream();
    }

    private void LoadMap(LDtkMap ldtkMap) {

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
    }

    private void Reset() {
        if (mapData != null) {
            if (mapData.loc_room != null) {
                foreach (Room room in mapData.loc_room.Values) {
                    Destroy(room.gameObject);
                }
                mapData.loc_room = new Dictionary<Vector2Int, Room>();
            }
        }
    }

    public void Refresh(Vector2Int location) {
        this.location = location;
        minimap.Refresh(location, mapData, height, DefaultGridSize, DoorGridSize);
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

    /* --- Rooms --- */
    public void OpenRoom(int roomID, Vector2Int newLocation, bool usePreload = true) {

        // Deactivate the previous room.
        if (mapData.loc_room.ContainsKey(location)) {
            mapData.loc_room[location].gameObject.SetActive(false); 
        }

        RoomData roomData = GetRoomData(newLocation);
        Room room = null;
        if (mapData.loc_room.ContainsKey(newLocation) && usePreload) {
            room = ReloadRoom(newLocation);
        }
        else {
            room = LoadRoom(roomID, roomData, newLocation);
        }
        EditRoom(room, roomData);

        // Refresh the map.
        Refresh(newLocation);

    }

    // Get the room data.
    private RoomData GetRoomData(Vector2Int location) {

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

        List<LDtkTileData> doorData = FindDoorsAtLocation(location, mapData.doors);

        return new RoomData(shape, offset, challenge, prize, doorData);
    }

    // Reload a previously loaded room.
    public Room ReloadRoom(Vector2Int newLocation) {
        // Load the previously loaded room.
        Room room = mapData.loc_room[newLocation];
        room.gameObject.SetActive(true);
        room = mapData.loc_room[newLocation];
        room.Refresh();
        return room;
    }

    // Load up a new room or reset a preloaded room.
    public Room LoadRoom(int id, RoomData roomData, Vector2Int newLocation) {

        if (mapData.loc_room.ContainsKey(newLocation)) {
            Destroy(mapData.loc_room[newLocation].gameObject);
            mapData.loc_room.Remove(newLocation);
        }

        // Instantiate a new room.
        Room room = Instantiate(roomBase, roomBase.transform.position, Quaternion.identity, transform).GetComponent<Room>();
        room.gameObject.SetActive(true);

        // Find a matching LDtk set to pick a room from.
        LDtkComponentProject lDtkRoomData = FindMatchingLDtkSet(roomData);

        LDtkRoom ldtkRoom = null;
        if (lDtkRoomData != null) {
            // Pick a room from the LDtk set using the randomizer from the seed.
            LdtkJson roomJson = lDtkRoomData.FromJson();
            ldtkRoom = GetLevelByID(lDtkRoomData, id);
        }

        // Load the room.
        room.Reset();
        room.Refresh();
        room.Load(ldtkRoom);

        // Add it to the list of loaded rooms.
        mapData.loc_room.Add(newLocation, room);
        return room;
    }

    // Edit the room to fit the map requirements.
    public void EditRoom(Room room, RoomData roomData) {
        if (roomData != null && room != null) {
            // Set the shape.
            Geometry.Carve(roomData.shape, roomData.shapeOffset, room.borderMap, room.height, room.width);
            Geometry.Carve(roomData.shape, roomData.shapeOffset, room.floorMap, room.height, room.width);
            // Load the doors.
            LoadDoors(room, roomData.doorData);
        }
    }

    /* --- Doors --- */
    private void ResetDoors() {
        for (int i = 0; i < doors.Count; i++) {
            if (doors[i] != null) {
                Destroy(doors[i].gameObject);
            }
        }
        doors = new List<Door>();
    }

    private void LoadDoors(Room room, List<LDtkTileData> doorData) {
        for (int i = 0; i < doorData.Count; i++) {
            // Clear the tile space.
            Vector3Int tilePosition = new Vector3Int((int)doorData[i].offsetPosition.x, (int)doorData[i].offsetPosition.y, 0);
            room.borderMap.SetTile(tilePosition, null);
            // Instantiate the door
            Door newDoor = Instantiate(doorBase, (Vector3)tilePosition, Quaternion.identity, doorParentTransform).GetComponent<Door>();
            //newDoor.gameObject.SetActive(true);
            //newDoor.Load(doorData[i], mapData.doorSwitch, mapData.unlockedDoors);
            doors.Add(newDoor);
        }
    }

    // Need a specific method for finding the doors at this location as it has to be parsed slightly differently.
    private List<LDtkTileData> FindDoorsAtLocation(Vector2Int location, List<LDtkTileData> doorData) {

        // Instantiate a new list for doors.
        List<LDtkTileData> doors = new List<LDtkTileData>();

        // Get the appropriate adjacent locations.
        Vector2Int leftLocation = new Vector2Int(location.x - 1, location.y);
        Vector2Int upLocation = new Vector2Int(location.x, location.y - 1);

        for (int i = 0; i < doorData.Count; i++) {

            if (doorData[i].gridPosition == location && doorData[i].offsetPosition.y == 7) {
                Vector2Int offsetPosition = new Vector2Int(doorData[i].offsetPosition.x, doorData[i].offsetPosition.y - 8);
                LDtkTileData adjustedDoorData = new LDtkTileData(doorData[i].vectorID, doorData[i].gridPosition, offsetPosition, doorData[i].index, 2);
                doors.Add(adjustedDoorData);
            }

            else if (doorData[i].gridPosition == location && doorData[i].offsetPosition.x == 7) {
                // offsetPosition = offsetPosition / height * (height + 2)
                Vector2Int offsetPosition = new Vector2Int(doorData[i].offsetPosition.x, 6 - doorData[i].offsetPosition.y);
                LDtkTileData adjustedDoorData = new LDtkTileData(doorData[i].vectorID, doorData[i].gridPosition, offsetPosition, doorData[i].index, 3);
                doors.Add(adjustedDoorData);
            }

            else if (doorData[i].gridPosition == upLocation && doorData[i].offsetPosition.y == 7) {
                LDtkTileData adjustedDoorData = new LDtkTileData(doorData[i].vectorID, doorData[i].gridPosition, doorData[i].offsetPosition, doorData[i].index, 0);
                doors.Add(adjustedDoorData);
            }

            else if (doorData[i].gridPosition == leftLocation && doorData[i].offsetPosition.x == 7) {
                Vector2Int offsetPosition = new Vector2Int(doorData[i].offsetPosition.x - 8, 6 - doorData[i].offsetPosition.y);
                LDtkTileData adjustedDoorData = new LDtkTileData(doorData[i].vectorID, doorData[i].gridPosition, offsetPosition, doorData[i].index, 1);
                doors.Add(adjustedDoorData);
            }

        }

        return doors;
    }

    /* --- Additional LDtk --- */
    private LDtkComponentProject FindMatchingLDtkSet(RoomData roomData) {

        int index = Seed();

        // Do prizes first i guess...
        switch (roomData.prize) {
            case (Prize.Lever):
                index = index % lDtkLeverRoomData.Length;
                return lDtkLeverRoomData[index];
            case (Prize.Key):
                index = index % lDtkKeyRoomData.Length;
                return lDtkKeyRoomData[index];
            default:
                break;
        }

        switch (roomData.challenge) {
            case (Challenge.Combat):
                index = index % lDtkCombatRoomData.Length;
                return lDtkCombatRoomData[index];
            case (Challenge.Puzzle):
                index = index % lDtkPuzzleRoomData.Length;
                return lDtkPuzzleRoomData[index];
            case (Challenge.Trap):
                index = index % lDtkTrapRoomData.Length;
                return lDtkTrapRoomData[index];
            default:
                break;
        }

        return null;
    }

    private static int Seed() {
        return 1;
    }
}
