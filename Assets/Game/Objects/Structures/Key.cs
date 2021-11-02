using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Key : Structure {

    // Motion
    private Vector3 floatVelocity = new Vector3(0f, 0.5f, 0f); // bob up
    private float floatDuration = 0.5f;

    Rigidbody2D body;

    void Awake() {
        interactAction = State.Action.Pushing;

        body = GetComponent<Rigidbody2D>();
        body.isKinematic = false;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.gravityScale = 0f;

        Float();

    }

    /* --- Overridden Methods --- */
    public override bool Interact(Controller controller) {

        
        if (controller.GetComponent<Player>()) {
            print("hello");
            Player player = controller.GetComponent<Player>();
            // player.keys.Add(this);
            player.numKeys += 1;
            gameObject.SetActive(false);
        }

        return true;

    }

    void Float() {
        body.velocity = floatVelocity;
        StartCoroutine(IEFloat(0.5f));
    }

    /* --- Coroutines --- */
    private IEnumerator IEFloat(float delay) {
        yield return new WaitForSeconds(delay);

        // Bob in the opposite direction
        body.velocity = -body.velocity;
        StartCoroutine(IEFloat(floatDuration));

        yield return null;
    }


}
