using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Dialogue : MonoBehaviour {

    /* --- Components --- */
    public ASCII ascii;
    public int charactersPerLine = 36; // characters per line in the dialogue box
    public static string path = "DataFiles/Dialogue/";

    /* --- Variables --- */
    private static float talkDelay = 0.075f; // time between letters appearing;
    private static float talkBuffer = 0.1f; // time before the next line starts

    public static bool isRunningCommand = false;

    /* --- Unity --- */
    void Update() {

    }

    public void Run(string filename) {
        isRunningCommand = true;
        gameObject.SetActive(true);
        string outputString = IO.OpenText(path, filename);
        Talk(outputString);
    }

    // run through the talk command
    bool Talk(string outputString) {
        IEnumerator talkCoroutine = IETalk(talkDelay, outputString);
        StartCoroutine(talkCoroutine);
        return true;
    }

    // run through each letter of the talk command
    IEnumerator IETalk(float delay, string outputString) {
        // the list of characters that have been printed
        List<char> partialCharList = new List<char>();
        // store the time in between letters locally
        // so that we can adjust its rate easily
        float localDelay = delay;

        // print each letter one at a time
        for (int i = 0; i < outputString.Length; i++) {

            // add the next letter to the string
            partialCharList.Add(outputString[i]);
            string partialString = new string(partialCharList.ToArray());

            // skip waiting on spaces
            if (outputString[i] == ' ') {
                yield return new WaitForSeconds(0);
            }
            // wait before printing the next letter
            else {
                yield return new WaitForSeconds(localDelay);
            }

            // print the string to the dialogue box
            // dialogue.SetText(partialString);
            if (partialString.Length > charactersPerLine) {
                partialString = partialString.Substring(partialString.Length - charactersPerLine, charactersPerLine);
            }
            ascii.SetText(partialString);
        }

        // wait a bit before exiting the current text 
        // to give some time to read
        yield return new WaitForSeconds(talkBuffer);

        // when this command has completed
        // switch this to inform the overhead
        // to run the next command
        print("finished");
        isRunningCommand = false;
        yield return null;
    }

    public void Clear() {
        ascii.SetText("");
    }
    
}
