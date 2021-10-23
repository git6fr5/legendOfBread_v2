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
    public float talkDelay = 0.075f; // time between letters appearing;
    public float regularTalkDelay = 0.075f; // time between letters appearing;
    public float fastTalkDelay = 0.025f; // time between letters appearing;
    private static float talkBuffer = 0.1f; // time before the next line starts

    public bool isRunningCommand = false;
    public bool isPrintingDialogue = false;

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
        IEnumerator talkCoroutine = IETalk(outputString);
        StartCoroutine(talkCoroutine);
        return true;
    }

    // run through each letter of the talk command
    IEnumerator IETalk(string outputString) {
        // the list of characters that have been printed
        List<char> partialCharList = new List<char>();

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
                yield return new WaitForSeconds(talkDelay);
            }

            // print the string to the dialogue box
            // dialogue.SetText(partialString);
            if (partialString.Length > charactersPerLine) {
                partialString = partialString.Substring(partialString.Length - charactersPerLine, charactersPerLine);
            }
            isPrintingDialogue = true;
            ascii.SetText(partialString);
        }

        // wait a bit before exiting the current text 
        // to give some time to read
        isPrintingDialogue = false;
        yield return new WaitForSeconds(talkBuffer);

        // when this command has completed
        // switch this to inform the overhead
        // to run the next command
        isRunningCommand = false;
        yield return null;
    }

    public void Clear() {
        ascii.SetText("");
    }
    
}
