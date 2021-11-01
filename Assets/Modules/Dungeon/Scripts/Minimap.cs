using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    public List<Transform> squares = new List<Transform>();
    public List<Transform> doors = new List<Transform>();

    public Transform squareBase;
    public Transform playerBase;
    public Transform doorBase;

    public float scale;

    public Sprite regularDoor;
    public Sprite onDoor;
    public Sprite offDoor;
    public Sprite keyDoor;
    public Sprite itemDoor;

    public Dictionary<Map.Door, Sprite> door_sprite = null;

    void SetUpDict() {
        door_sprite = new Dictionary<Map.Door, Sprite>();

        door_sprite.Add(Map.Door.Regular, regularDoor);
        door_sprite.Add(Map.Door.On, onDoor);
        door_sprite.Add(Map.Door.Off, offDoor);
        door_sprite.Add(Map.Door.Key, keyDoor);
        door_sprite.Add(Map.Door.Item, itemDoor);

    }

    public void Reset() {
        for (int i = 0; i < squares.Count; i++) {
            Destroy(squares[i].gameObject);
            squares[i] = null;
        }
        squares = new List<Transform>();
        for (int i = 0; i < doors.Count; i++) {
            Destroy(doors[i].gameObject);
            doors[i] = null;
        }
        doors = new List<Transform>();
    }

    public void Create(List<Vector2Int> gridPositions, float size) {
        Reset();
        for (int i = 0; i < gridPositions.Count; i++) {
            AddLocation(gridPositions[i], size);
        }
    }

    public float scaleDown = 8f;

    public void AddDoor(Map map, Vector2Int vectorID, Vector2Int gridPosition, Vector2Int offsetPosition, float size) {

        if (door_sprite == null) {
            SetUpDict();
        }

        if (door_sprite.ContainsKey((Map.Door)vectorID.x)) {
            float factor = scale / (size - 1);
            Transform newDoor = Instantiate(doorBase, new Vector3(squareBase.position.x + gridPosition.x * factor, squareBase.position.y - gridPosition.y * factor, 0f), Quaternion.identity, transform);

            if (offsetPosition.x == 7) {
                offsetPosition += new Vector2Int(-8, 0);
            }

            newDoor.position -= new Vector3((offsetPosition.x - 3) * factor / scaleDown, (offsetPosition.y - 3) * factor / scaleDown, 0f);

            newDoor.gameObject.SetActive(true);

            Map.Door door = (Map.Door)vectorID.x;
            if (map.doorSwitch == Map.Switch.Off) {
                if (door == Map.Door.On) { door = Map.Door.Off; }
                else if (door == Map.Door.Off) { door = Map.Door.On; }
            }

            newDoor.GetComponent<SpriteRenderer>().sprite = door_sprite[door];
            doors.Add(newDoor);

        }


    }

    public void AddLocation(Vector2Int gridPosition, float size) {

        Transform newSquare = Instantiate(squareBase, new Vector3(squareBase.position.x + gridPosition.x * scale / (size - 1), squareBase.position.y - gridPosition.y * scale / (size - 1), 0f), Quaternion.identity, transform);
        newSquare.gameObject.SetActive(true);
        squares.Add(newSquare);

    }

    public void SetPlayerLocation(Vector2Int gridPosition, float size) {
        playerBase.position = new Vector3(squareBase.position.x + gridPosition.x * scale / (size - 1), squareBase.position.y - gridPosition.y * scale / (size - 1), 0f);
        playerBase.gameObject.SetActive(true);
    }

}
