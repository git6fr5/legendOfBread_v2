using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour {

    public enum Season {
        Summer,
        Autumn,
        Winter,
        Spring
    }

    public enum DayTime {
        Dawn,
        Morning,
        Afternoon,
        Evening,

        Dusk,
        Night,
        Midnight,
        Twilight,

        count
    }

    Vector3 origin;
    public Transform followTransform;
    public bool isOverworld;

    [Range(0f, 5f)] public float shakeStrength = 1f;
    public float shakeDuration = 0.5f;
    float elapsedTime = 0f;
    public bool shake;
    public AnimationCurve curve;

    void Awake() {
        origin = transform.position;
        followTransform.parent = transform;

        daylightColors.Add(DayTime.Dawn, dawnColor);
        daylightColors.Add(DayTime.Morning, morningColor);
        daylightColors.Add(DayTime.Afternoon, afternoonColor);
        daylightColors.Add(DayTime.Evening, eveningColor);
        daylightColors.Add(DayTime.Dusk, duskColor);
        daylightColors.Add(DayTime.Night, nightColor);
        daylightColors.Add(DayTime.Midnight, midnightColor);
        daylightColors.Add(DayTime.Twilight, twilightColor);

    }

    void Update() {
        if (isOverworld == true) {
            Follow();
        }
        else {
            followTransform.parent = transform;
            transform.position = origin;
        }

        if (shake) {
            shake = Shake();
        }

        if (dayCycle == null) {
            // approx.
            dayCycle = StartCoroutine(DayCycle(Time.deltaTime * dayUpdateTickFrequency));
        }

        ticks = GameRules.gameTicks % ticksPerDay;
        daytime = (DayTime)((float)ticks * (int)DayTime.count / (float)ticksPerDay);

    }

    // Renderer
    public Color dawnColor = Color.white;
    public Color morningColor = Color.white;
    public Color afternoonColor = Color.white;
    public Color eveningColor = Color.white;
    public Color duskColor = Color.white;
    public Color nightColor = Color.white;
    public Color midnightColor = Color.white;
    public Color twilightColor = Color.white;
    public Dictionary<DayTime, Color> daylightColors = new Dictionary<DayTime, Color>();

    public bool renderDayCycle;
    public Material dayTimeMaterial;
    public Material defaultMaterial;
    int dayUpdateTickFrequency = 64;
    Coroutine dayCycle = null;


    // public Season season;
    public DayTime daytime;

    public int ticks;
    public int ticksPerDay;


    IEnumerator DayCycle(float delay) {
        yield return new WaitForSeconds(delay);
        dayCycle = StartCoroutine(DayCycle(delay));
        float percentPassed = ((float)ticks * (int)DayTime.count / (float)ticksPerDay) - (int)daytime;
        Color newColor = daylightColors[daytime] * (1f - percentPassed) + daylightColors[(DayTime)(((int)daytime + 1) % (int)DayTime.count)] * percentPassed;
        dayTimeMaterial.SetColor("_Color", newColor);

        daylightColors[DayTime.Dawn] = dawnColor;
        daylightColors[DayTime.Morning] = morningColor;
        daylightColors[DayTime.Afternoon] = afternoonColor;
        daylightColors[DayTime.Evening] = eveningColor;
        daylightColors[DayTime.Dusk] = duskColor;
        daylightColors[DayTime.Night] = nightColor;
        daylightColors[DayTime.Midnight] = midnightColor;
        daylightColors[DayTime.Twilight] = twilightColor;

        yield return null;

    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (renderDayCycle) {
            Graphics.Blit(source, destination, dayTimeMaterial);
        }
        else {
            Graphics.Blit(source, destination, defaultMaterial);
        }
    }

    void Follow() {
        if (followTransform.parent = transform) {
            followTransform.parent = null;
        }
        transform.position = new Vector3(followTransform.position.x, followTransform.position.y, origin.z);
    }

    bool Shake() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shakeDuration) {
            elapsedTime = 0f;
            return false;
        }
        float strength = shakeStrength * curve.Evaluate(elapsedTime / shakeDuration);
        transform.position = transform.position + (Vector3)Random.insideUnitCircle * shakeStrength;
        return true;
    }

}
