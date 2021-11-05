using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : Controller {

    /* --- Variables --- */
    // Controls the decision flow of the mob.
    public enum MINDSET {
        IDLE,
        ANGRY,
        count
    }
    protected MINDSET mindset;

    // The loot that this mob will drop on death.
    public Item[] loot;

    // The vision of this mob.
    public Vision vision;

    // Mob-Specific Action Controls
    [Range(0.05f, 1f)] public float idleFactor = 1f;
    [Range(0.05f, 2f)] public float aggressiveFactor = 1.25f;

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {

        // Reset the movement.
        movementVector = Vector2.zero;
        moveSpeed = state.baseSpeed;

        switch (mindset) {
            case (MINDSET.IDLE):
                Idle();
                break;
            case (MINDSET.ANGRY):
                Angry();
                break;
            default:
                break;
        }
    }

    /* --- Action Flow --- */
    // The action logic while this mob is idle.
    protected virtual void Idle() {
        // Determined by the particular type of mob.
    }

    // The action logic while this mob is angry.
    protected virtual void Angry() {
        // Determined by the particular type of mob.
    }

    /* --- Event Actions --- */
    protected override void SetEnemies() {
        state.enemyTags = new List<string>() { GameRules.playerTag };
    }

    public Skrit skritBase;
    public int skritMinValue; public int skritMaxValue;

    protected void DropSkrit() {

        int totalValue = Random.Range(skritMinValue, skritMaxValue);

        List<int> denominations = new List<int>();
        int repeats = 0;
        while (totalValue > 0 || repeats < 32) {
            for (int j = Skrit.Values.Length - 1; j >= 0; j--) {
                if (totalValue >= Skrit.Values[j]) {
                    denominations.Add(Skrit.Values[j]);
                    totalValue -= Skrit.Values[j];
                }
            }
            repeats = repeats + 1;
        }

        print(denominations.Count);

        for (int i = 0; i < denominations.Count; i++) {
            Skrit newSkrit = Instantiate(skritBase, transform.position, Quaternion.identity, transform.parent).GetComponent<Skrit>();
            newSkrit.SetValue(denominations[i]);
            newSkrit.gameObject.SetActive(true);
        }
    }
}
