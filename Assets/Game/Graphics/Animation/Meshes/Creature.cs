/* --- Modules --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using Orientation = Compass.Orientation;
using Action = State.Action;
using Vitality = State.Vitality;
using Movement = State.Movement;

public class Creature : Mesh {

    /* --- Components --- */
    public State state;

    /* --- Animations --- */
    public Sprite[] idle;

    /* --- Materials --- */
    public Material defaultMaterial;
    public Material hurtMaterial;
    public Material deathMaterial;

    /* --- Variables --- */
    [SerializeField] public bool isBobbly;
    Sprite[] active; // This is solely to be used as a switch
    [HideInInspector] public float timeInterval = 0f;

    /* --- Dictionaries --- */
    public Dictionary<Action, (Sprite[], bool)> actionAnimations = new Dictionary<Action, (Sprite[], bool)>();
    public Dictionary<Movement, (Sprite[], bool)> movementAnimations = new Dictionary<Movement, (Sprite[], bool)>();
    public Dictionary<Vitality, Material> vitalityMaterials = new Dictionary<Vitality, Material>();

    protected override void AddMaterials() {
        vitalityMaterials.Add(Vitality.Healthy, defaultMaterial);
        vitalityMaterials.Add(Vitality.Hurt, hurtMaterial);
        vitalityMaterials.Add(Vitality.Dead, deathMaterial);
    }

    /* --- Override --- */
    // The parameters to be rendered every frame
    public override void Render() {
        RenderAnimation();
        RenderMaterial();
    }

    /* --- Parameters --- */
    // Renders the sprite based on the state.
    void RenderAnimation() {

        timeInterval += Time.deltaTime;
        float? buffer = null;
        (Sprite[], bool) animationData;
        bool movementBob = false;
        transform.localPosition = Vector3.zero;

        // Check action animations.
        if (state.activeItem != null && actionAnimations.ContainsKey(state.activeItem.action)) {
            animationData = actionAnimations[state.activeItem.action];
            buffer = state.activeItem.actionBuffer;
        }
        // Check movement animations.
        else if (movementAnimations.ContainsKey(state.movement)) {
            animationData = movementAnimations[state.movement];
            movementBob = isBobbly;
        }
        // Default to idle animation.
        else {
            animationData = (idle, false);
        }

        // Get the animation and reset timer if switching animations.
        Sprite[] animation = animationData.Item1;
        if (animation != active) {
            timeInterval = 0f;
            active = animation;
        }

        // Check if the animation accounts for the orientation, otherwise manually rotate.
        int orientatable = animationData.Item2 ? 1 : 0; ;
        if (orientatable == 0) {
            transform.localRotation = Compass.OrientationAngles[state.orientation];
        }

        // Set the current frame.
        int cycle = (int)(animation.Length * 0.25f * orientatable + animation.Length * (1 - orientatable));
        float frameRate = (buffer != null) ? cycle / (float)buffer : GameRules.defaultFrameRate;
        int index = (orientatable * cycle * (int)state.orientation) + ((int)Mathf.Floor(timeInterval * frameRate) % cycle);
        transform.localPosition = (movementBob && index % 2 == 1) ? new Vector3(0f, 0.05f, 0f) : Vector3.zero;
        spriteRenderer.sprite = animation[index];
    }

    // Renders the material based on the state.
    void RenderMaterial() {
        if (vitalityMaterials.ContainsKey(state.vitality)) {
            spriteRenderer.material = vitalityMaterials[state.vitality];
        }
        else {
            spriteRenderer.material = defaultMaterial;
        }
    }

}
