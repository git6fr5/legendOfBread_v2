using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonator : Equipable {

    public Bomb bomb;

    protected override bool OnActivate(Controller controller) {
        Bomb newBomb = Instantiate(bomb, Vector3.zero, Quaternion.identity, null).GetComponent<Bomb>();
        newBomb.condition = Structure.Condition.Interactable;
        newBomb.gameObject.SetActive(true);
        newBomb.Interact(controller);
        return true;
    }

}
