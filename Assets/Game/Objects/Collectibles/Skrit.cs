using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skrit : Collectible {

    public int value;
    public Dictionary<int, Sprite> val_sprites;

    public Sprite[] sprites;
    public static int[] Values = new int[] { 1, 5, 20, 50 };
    public SpriteRenderer spriteRenderer;

    public Vector2 direction;

    /* --- Overridden Methods --- */
    void Awake() {

        val_sprites = new Dictionary<int, Sprite>();
        for (int i = 0; i < Values.Length; i++) {
            if (i < sprites.Length) {
                val_sprites.Add(Values[i], sprites[i]);
            }
            else {
                val_sprites.Add(Values[i], sprites[sprites.Length - 1]);
            }
        }
        origin = transform.position;
        SetValue(value);
    }

    public void SetValue(int setVal) {

        value = setVal;

        if (!val_sprites.ContainsKey(value)) {
            int newValue = 0;
            int minDiff = (int)1e9;
            for (int i = 0; i < Values.Length; i++) {
                if (Mathf.Abs(value - Values[i]) < minDiff) {
                    newValue = Values[i];
                    minDiff = Mathf.Abs(value - Values[i]);
                }
            }
            value = newValue;
        }

        Vector2 offset = Random.insideUnitCircle.normalized * 0.15f;
        transform.position += (Vector3)offset;
        GetComponent<Rigidbody2D>().velocity = offset.normalized;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

        collectDelay = Random.Range(0.5f, 0.75f);
        shadow.gameObject.SetActive(false);

        GetComponent<Rigidbody2D>().AddTorque(360f / collectDelay);


        GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(IEEnableCollection(collectDelay));
        spriteRenderer.sprite = val_sprites[value];
    }

    protected override void Collect(Player player) {

        // player.keys.Add(this);
        player.skrit += value;
        Destroy(gameObject);

    }


    private IEnumerator IEEnableCollection(float delay) {
        yield return new WaitForSeconds(delay);

        // Bob in the opposite direction
        GetComponent<CircleCollider2D>().enabled = true;

        yield return null;
    }

}
