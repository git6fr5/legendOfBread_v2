using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Mesh : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] public SpriteRenderer spriteRenderer;

    /* --- Variables --- */
    [SerializeField] public Transform overhead; // The position from which the entities carries structures.
    [SerializeField] public Transform hull; // The position where this entities feet contacts the ground.
    [SerializeField] public Collider2D frame; // The collision frame of this entity.

    /* --- Unity --- */
    // Runs once before the first frame
    void Awake() {
        tag = GameRules.meshTag;
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set the layer of this mesh;
        spriteRenderer.sortingLayerName = GameRules.midGround;
        AddAnimations();
        AddMaterials();
    }

    // Runs every frame.
    void Update() {
        Render();
    }

    /* --- Dictionaries --- */
    protected virtual void AddAnimations() {

    }

    protected virtual void AddMaterials() {

    }

    /* --- Rendering --- */
    // The parameters to be rendered every frame
    public virtual void Render() {
        // Determined by the particular type of mesh.
    }

    // Compare the depth of the meshes.
    public static int Compare(Mesh meshA, Mesh meshB) {
        return (-meshA.hull.position.y).CompareTo((-meshB.hull.position.y));
    }

}
