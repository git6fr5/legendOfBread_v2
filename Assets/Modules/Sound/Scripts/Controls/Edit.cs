using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

[RequireComponent(typeof(BoxCollider2D))]
public class Edit : MonoBehaviour
{

    public GameObject synthPrefab;
    
    void OnMouseDown() {
        // Only specifying the sceneName or sceneBuildIndex will load the Scene with the Single mode
        //SceneManager.LoadScene("Synth", LoadSceneMode.Additive);

        GameObject newSynth = Instantiate(synthPrefab);
        newSynth.transform.position = new Vector3(15f, 15f, 0);

        newSynth.GetComponent<Synth>().synthCam.targetDisplay = 1;
        print(UnityEngine.Display.displays.Length);
        //UnityEngine.Display.displays[1].Activate();
    }

}
