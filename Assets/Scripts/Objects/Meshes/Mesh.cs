using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh : MonoBehaviour {

    /* --- Components --- */
    public Transform hull;

    /* --- Variables --- */
    public float depth;
    protected Vector3 groundPosition;
    // For the colliders.
    public float height = 1f;
    public float width = 12f / 16f;

    /* --- Unity --- */
    // Runs once before the first frame
    void Awake() {
        tag = GameRules.meshTag;

        groundPosition = new Vector3(0, height / 2, 0);
        BoxCollider2D meshCollider = hull.gameObject.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
        meshCollider.size = new Vector2(width, height * Mathf.Sin(GameRules.perspectiveAngle));
        meshCollider.offset = new Vector2(0f, height * Mathf.Sin(GameRules.perspectiveAngle) / 2f);
        CapsuleCollider2D hullCollider = hull.gameObject.AddComponent(typeof(CapsuleCollider2D)) as CapsuleCollider2D;
        hullCollider.direction = CapsuleDirection2D.Horizontal;
        hullCollider.size = new Vector2(width, 1f / 16f);
    }

    // Runs every frame.
    void Update() {
        Render();
        SetDepth();
    }

    /* --- Rendering --- */
    // The parameters to be rendered every frame
    public virtual void Render() {
        // Determined by the particular type of mesh.
    }

    /* --- Depth --- */
    // Sets the depth of this mesh
    void SetDepth() {
        depth = -(hull.position.y);
    }

    // Compare the depth of the meshes.
    public static int Compare(Mesh meshA, Mesh meshB) {
        print(meshA); print(meshB);
        return meshA.depth.CompareTo(meshB.depth);
    }

}
