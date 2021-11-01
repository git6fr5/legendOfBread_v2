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

    /* --- Static Variables --- */
    // Layer Names
    public static string ShapeLayer = "Shape";
    public static string ChallengeLayer = "Challenge";
    public static string DoorLayer = "Door_Item";
    public static string PrizeLayer = "Key";

    // Grid Sizes
    public static int DoorLayerGridSize = 2;

    /* --- Enumerations --- */
    public enum Shape {
        Square,
        Screw,
        Corridor,
        DoubleCorridor,
        Podium,
        Island,
        Hallway
    }

    public enum Challenge {
        Entrance,
        Combat,
        Puzzle,
        Trap,
        Boss,
        None
    }

    public enum Prize {
        Key,
        Treasure,
        Lever,
        None
    }

    public enum Door {
        Regular,
        On,
        Off,
        Key,
        Item
    }

    public enum Switch {
        On,
        Off,
        Count
    }

    // The location of the challenge tiles in the source sprite.
    private Vector2Int challenge_src_origin = new Vector2Int(4, 0);
    private Vector2Int prize_src_origin = new Vector2Int(4, 1);

    [SerializeField] public int seed = 0;
    [SerializeField] [ReadOnly] private int generatedSeed;

    /* --- Components --- */
    public LDtkComponentProject lDtkMapData;
    public LDtkComponentProject[] lDtkCombatRoomData;
    public LDtkComponentProject[] lDtkPuzzleRoomData;
    public LDtkComponentProject[] lDtkTrapRoomData;
    public LDtkComponentProject[] lDtkLeverRoomData;
    public LDtkComponentProject[] lDtkKeyRoomData;

    /* --- Variables --- */
    [SerializeField] [ReadOnly] private LdtkJson mapJson;
    [SerializeField] [ReadOnly] int height;
    [SerializeField] [ReadOnly] int width;

    public Vector2Int location;
    LDtkMap map;
    public Room roomBase;
    public Minimap minimap;
    public Exit exit;

    public Switch doorSwitch;

    public class LevelInfo {
        public Shape shape;
        public int shapeOffset;
        public Challenge challenge;
        public Prize prize;
        public List<LDtkTileData> doorData;

        public LevelInfo(Shape shape, int shapeOffset, Challenge challenge, Prize prize, List<LDtkTileData> doorData) {
            this.shape = shape;
            this.shapeOffset = shapeOffset;
            this.challenge = challenge;
            this.prize = prize;
            this.doorData = doorData;
        }
    }

    public override void Start() {
        generatedSeed = GameRules.PrimeRandomizer(seed);
        OpenMap(0);
    }

    void Update() {

    }

    public void FlipSwitch() {
        doorSwitch = (Switch)(((int)doorSwitch + 1) % (int)Switch.Count);
    }

    public Dictionary<Vector2Int, Room> loc_room = new Dictionary<Vector2Int, Room>();

    public override void OpenRoom(int id) {
        OpenRoom(location);
    }

    public void OpenRoom(Vector2Int newLocation) {

        if (loc_room.ContainsKey(newLocation)) {
            room.gameObject.SetActive(false);
            loc_room[newLocation].gameObject.SetActive(true);
            room = loc_room[newLocation];

            // Load the tiles.
            environment.RefreshTiles();
            LoadTiles(room.borderMap, environment.borderTile, room.height, room.width);
            LoadTiles(room.floorMap, environment.floorTile, room.height, room.width);

            LoadDoors(GetDoors());

            location = newLocation;
        }
        else {
            location = newLocation;
            if (room != null) {
                room.gameObject.SetActive(false);
            }

            room = Instantiate(roomBase, roomBase.transform.position, Quaternion.identity, transform).GetComponent<Room>();

            LevelInfo info = GetRequiredLevelInfo(map, location);
            lDtkData = FindMatchingLevel(info);

            if (lDtkData != null) {

                // Pick a level.
                LdtkJson roomJson = lDtkData.FromJson();
                int id = GameRules.PrimeRandomizer(location.x + location.y) % roomJson.Levels.Length;
                if (id == 0) {
                    id = (id + 1) % roomJson.Levels.Length;
                }
                print("THE ID WE'RE TRYING TO LOAD IS " + id.ToString());

                LDtkRoom ldtkRoom = GetRoomByID(lDtkData, id);
                LoadRoom(ldtkRoom);

            }
            else {
                room.height = 7; room.width = 7;
                LoadRoom(null);
            }

            loc_room.Add(location, room);

            print("SHAPE" + info.shape);
            Geometry.Carve(info.shape, info.shapeOffset, room.borderMap, room.height, room.width);
            Geometry.Carve(info.shape, info.shapeOffset, room.floorMap, room.height, room.width);
            LoadDoors(info.doorData);
        }

        OnLoad.Invoke();
        minimap.SetPlayerLocation(location, (float)height);

    }

    private List<LDtkTileData> GetDoors() {
        // Reset the exits.

        print("Reloading Doors");

        List<LDtkTileData> doors = new List<LDtkTileData>();

        if (room.exits != null) {
            for (int i = 0; i < room.exits.Count; i++) {
                doors.Add(new LDtkTileData(room.exits[i].exitData.gridPosition, room.exits[i].exitData.vectorID, room.exits[i].index));
            }
        }
        // Reset the exits.
        if (room.exits != null) {
            for (int i = 0; i < room.exits.Count; i++) {
                if (room.exits[i] != null) {
                    Destroy(room.exits[i].gameObject);
                }
            }
            room.exits = new List<Exit>();
        }

        return doors;
    }

    private void LoadDoors(List<LDtkTileData> doorData) {
        for (int i = 0; i < doorData.Count; i++) {
            room.AddDoor(this, doorData[i], exit);
        }
    }

    public void OpenMap(string str_id) {
        int id = Int32.Parse(str_id);
        OpenMap(id);
    }

    private void OpenMap(int id) {

        unlockedExits = new List<int>();
        foreach (Room room in loc_room.Values) {
            Destroy(room.gameObject);
        }
        loc_room = new Dictionary<Vector2Int, Room>();

        map = GetMapByID(id);
        LoadMinimap();
        Vector2Int entrance = GetEntrance();
        print(entrance);
        OpenRoom(entrance);
        SetMapStream(id);
    }

    public int id;
    protected void SetMapStream(int id) {
        if (stream != null) {
            stream.SetText(id.ToString());
        }
        this.id = id;
    }

    private Vector2Int GetEntrance() {
        LDtkUnity.LayerInstance challengeLayer = GetLayer(map, ChallengeLayer);
        if (challengeLayer != null) {
            for (int i = 0; i < challengeLayer.GridTiles.Length; i++) {
                // Get the source that this tile is pointing to.
                LDtkUnity.TileInstance tile = challengeLayer.GridTiles[i];
                Vector2Int gridPosition = tile.UnityPx / (int)mapJson.DefaultGridSize;

                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / (int)mapJson.DefaultGridSize;
                Challenge challenge = ((Vector2Int)vectorID - challenge_src_origin).y == 0 ? (Challenge)((Vector2Int)vectorID - challenge_src_origin).x : Challenge.None;
                if (challenge == Challenge.Entrance) {
                    return gridPosition;
                }
            }
        }
        return Vector2Int.zero;
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

    public void LoadMinimap() {
        List<Vector2Int> gridPositions = new List<Vector2Int>();

        LDtkUnity.LayerInstance shapeLayer = GetLayer(map, ShapeLayer);
        if (shapeLayer != null) {
            for (int i = 0; i < shapeLayer.GridTiles.Length; i++) {
                // Get the source that this tile is pointing to.
                LDtkUnity.TileInstance tile = shapeLayer.GridTiles[i];
                Vector2Int gridPosition = tile.UnityPx / (int)mapJson.DefaultGridSize;
                gridPositions.Add(gridPosition);
            }
        }
        minimap.Create(gridPositions, (float)height);


        LDtkUnity.LayerInstance doorLayer = GetLayer(map, DoorLayer);
        List<Vector2> doorPositions = new List<Vector2>();
        if (doorLayer != null) {
            for (int i = 0; i < doorLayer.GridTiles.Length; i++) {
                // Get the source that this tile is pointing to.
                LDtkUnity.TileInstance tile = doorLayer.GridTiles[i];
                Vector2Int gridPosition = tile.UnityPx / (int)mapJson.DefaultGridSize;
                //Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / DoorLayerGridSize;
                Vector2Int offsetPosition = (tile.UnityPx / DoorLayerGridSize) - gridPosition * ((int)mapJson.DefaultGridSize / DoorLayerGridSize);
                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / DoorLayerGridSize;
                minimap.AddDoor(this, vectorID, gridPosition, offsetPosition, (float)height);
            }
        }

    }

    private LevelInfo GetRequiredLevelInfo(LDtkMap map, Vector2Int location) {

        Shape shape = Shape.Square;
        int offset = 0;
        Vector2Int? shapeID = FindInLayerAtLocation(map, location, ShapeLayer);
        if (shapeID != null) {
            shape = (Shape)((Vector2Int)shapeID).y;
            offset = ((Vector2Int)shapeID).x;
        }

        Challenge challenge = Challenge.None;
        Vector2Int? challengeID = FindInLayerAtLocation(map, location, ChallengeLayer);
        if (challengeID != null) {
            challenge = (Challenge)((Vector2Int)challengeID - challenge_src_origin).x;
        }

        Prize prize = Prize.None;
        Vector2Int? prizeID = FindInLayerAtLocation(map, location, PrizeLayer);
        if (prizeID != null) {
            prize = (Prize)((Vector2Int)prizeID - prize_src_origin).x;
        }

        List<LDtkTileData> doorData = LoadDoorLayer(map, location);

        Debug.Log("Loading Room: " + shape.ToString() + ", " + challenge.ToString());
        return new LevelInfo(shape, offset, challenge, prize, doorData);
    }

    public List<int> unlockedExits = new List<int>();

    private List<LDtkTileData> LoadDoorLayer(LDtkUnity.Level map, Vector2Int location, List<LDtkTileData> exitData = null) {

        if (exitData == null) { exitData = new List<LDtkTileData>(); }

        // Get the layer.
        LDtkUnity.LayerInstance doorLayer = GetLayer(map, DoorLayer);
        
        // Get the appropriate adjacent locations.
        Vector2Int leftLocation = new Vector2Int(location.x - 1, location.y);
        Vector2Int upLocation = new Vector2Int(location.x, location.y - 1);

        // Compile the locations into an itterable.
        Vector2Int[] locations = new Vector2Int[] { location, leftLocation, upLocation };

        if (doorLayer != null) {
            for (int i = 0; i < doorLayer.GridTiles.Length; i++) {
                // Get the source that this tile is pointing to.
                LDtkUnity.TileInstance tile = doorLayer.GridTiles[i];
                Vector2Int gridPosition = tile.UnityPx / (int)mapJson.DefaultGridSize;
                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / DoorLayerGridSize;
                Vector2Int offsetPosition = (tile.UnityPx / DoorLayerGridSize) - gridPosition * ((int)mapJson.DefaultGridSize / DoorLayerGridSize);

                if (gridPosition == upLocation) {

                    if (offsetPosition.y == 7) {
                        // offsetPosition = offsetPosition / height * (height + 2)
                        LDtkTileData newExitData = new LDtkTileData(offsetPosition, vectorID, i);
                        exitData.Add(newExitData);
                    }
                }

                if (gridPosition == leftLocation) {

                    if (offsetPosition.x == 7) {
                        // offsetPosition = offsetPosition / height * (height + 2)
                        offsetPosition = new Vector2Int(offsetPosition.x, 6 - offsetPosition.y);
                        LDtkTileData newExitData = new LDtkTileData(offsetPosition + new Vector2Int(-8, 0), vectorID, i);
                        exitData.Add(newExitData);
                    }
                }

                if (gridPosition == location) {

                    if (offsetPosition.y == 7) {
                        // offsetPosition = offsetPosition / height * (height + 2)
                        LDtkTileData newExitData = new LDtkTileData(offsetPosition + new Vector2Int(0, -8), vectorID, i);
                        exitData.Add(newExitData);
                    }

                    if (offsetPosition.x == 7) {
                        // offsetPosition = offsetPosition / height * (height + 2)
                        offsetPosition = new Vector2Int(offsetPosition.x, 6 - offsetPosition.y);
                        LDtkTileData newExitData = new LDtkTileData(offsetPosition, vectorID, i);
                        exitData.Add(newExitData);
                    }

                }

            }
        }

        return exitData;
    }

    private Vector2Int? FindInLayerAtLocation(LDtkUnity.Level map, Vector2Int location, string layerName) {
        LDtkUnity.LayerInstance layer = GetLayer(map, layerName);
        if (layer != null) {
            for (int i = 0; i < layer.GridTiles.Length; i++) {
                // Get the source that this tile is pointing to.
                LDtkUnity.TileInstance tile = layer.GridTiles[i];
                Vector2Int gridPosition = tile.UnityPx / (int)mapJson.DefaultGridSize;

                if (gridPosition == location) {
                    Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / (int)mapJson.DefaultGridSize;
                    return (Vector2Int?)vectorID;
                }
            }
        }
        return null;
    }

    private LDtkComponentProject FindMatchingLevel(LevelInfo info) {

        int seedValue = Int32.Parse(generatedSeed.ToString().Substring(0, 2));
        int index;

        // Do prizes first i guess...
        switch(info.prize) {
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
