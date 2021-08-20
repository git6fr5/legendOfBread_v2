using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameRules : MonoBehaviour {

    /* --- Path --- */
    public static string Path = "Assets/Resources/";

    /* --- Time Keeping --- */
    public static int gameTicks;
    public static float gameTime;

    /* --- Tags --- */
    public static string dungeonTag = "Dungeon";
    public static string mapTag = "Map";
    public static string roomTag = "Room";
    public static string playerTag = "Player";
    public static string mobTag = "Mob";
    public static string meshTag = "Mesh";

    /* --- Layers --- */
    public static string backGround = "Background";
    public static string midGround = "Midground";
    public static string foreGround = "Foreground";

    /* --- Movement Precision --- */
    public static float movementPrecision = 0.05f;

    /* --- Unity --- */
    void Update() {
        Tick();
        Sort();
    }

    /* --- Methods --- */
    void Tick() {
        gameTime += Time.deltaTime;
        gameTicks++;
    }

    public static void Sort() {
        // Declare the object array and the array of sorted characters
        GameObject[] unsortedObjects = GameObject.FindGameObjectsWithTag(meshTag);

        // assumes all the objects tagged with meshes have mesh components
        Mesh[] meshes = new Mesh[unsortedObjects.Length];
        for (int i = 0; i < unsortedObjects.Length; i++) {
            meshes[i] = unsortedObjects[i].GetComponent<Mesh>();
        }

        // the depth is understood as the position of the y axis
        Array.Sort<Mesh>(meshes, new Comparison<Mesh>((meshA, meshB) => Mesh.Compare(meshA, meshB)));
        for (int i = 0; i < meshes.Length; i++) {
            if (meshes[i].GetComponent<SpriteRenderer>() != null) {
                meshes[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            }
            else if (meshes[i].GetComponent<TilemapRenderer>() != null) {
                meshes[i].GetComponent<TilemapRenderer>().sortingOrder = i;
            }
        }
    }

}
