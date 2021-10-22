using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Note = Score.Note;
using Tone = Score.Tone;
using Value = Score.NoteLength;
using Shape = Wave.Shape;
using Parameters = Wave.Parameters;
using Clef = Score.Clef;

public class DAW : MonoBehaviour {

    public Synth synthA;
    public Synth synthB;

    public class Channel {
        public Clef clef;
        public Synth synth;
        public int index;
        public float timeInterval;

        public Channel(Clef clef, Synth synth) {
            this.timeInterval = 0f;
            this.index = 0;
            this.clef = clef;
            synth.isPlayable = false;
            this.synth = synth;
        }
    }
    public List<Channel> channels = new List<Channel>();

    //BPM.
    [Space(5)] [Header("BPM")]
    [SerializeField] public Knob BPMKnob;
    [SerializeField] [ReadOnly] protected int BPM;
    [SerializeField] [ReadOnly] protected int minBPM = 30;
    [SerializeField] [ReadOnly] protected int maxBPM = 240;
    [HideInInspector] protected float secondsPerQuarterNote;

    public int editingChannel = 1;

    public Score score;

    public string scoreFile;
    public string fileA;
    public string fileB;

    public bool isEditing;

    void Awake() {
        score.Open(scoreFile, false);
        score.Instantiate();

        print("daw");

        // score.RandomScore();
        // score.SetNodesFromScore();
        channels.Add(new Channel(score.bass, synthA));
        channels.Add(new Channel(score.treble, synthB));

        StartCoroutine(IELoad());

        if (!isEditing) {
            DisableSprites(transform);
        }

    }

    private void DisableSprites(Transform transform) {
        foreach (Transform child in transform) {
            if (child.GetComponent<SpriteRenderer>() != null) {
                child.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (child.GetComponent<Collider2D>() != null) {
                child.GetComponent<Collider2D>().enabled = false;
            }
            if (child.childCount > 0) {
                DisableSprites(child);
            }
        }
    }

    IEnumerator IELoad() {
        yield return new WaitForSeconds(0.1f);
        synthA.Open(fileA);
        synthB.Open(fileB);
        yield return null;
    }

    void Update() {


        if (!isEditing) { BPM = 160; }
        else {
            BPM = (int)(BPMKnob.value * (maxBPM - minBPM)) + minBPM;
        }
        secondsPerQuarterNote = 60f / BPM;

        timeInterval += Time.deltaTime;
        float subdividedInterval = Score.LengthMultipliers[Value.SIXTEENTH];
        if (timeInterval >= subdividedInterval * secondsPerQuarterNote) {
            timeInterval -= subdividedInterval * secondsPerQuarterNote;
            subdividedIndex++;
        }
        maxIndex = (int)(barLength * score.bars / subdividedInterval);
        if (subdividedIndex >= maxIndex) {
            subdividedIndex = 0;
        }

        //if (Input.GetKeyDown(KeyCode.M)) {
        //    sheet = Score.MarioTheme();
        //    channels[0].clef = sheet.treble;
        //    channels[1].clef = sheet.bass;
        //}

        channels[0].clef = score.bass;
        channels[1].clef = score.treble;

        for (int i = 0; i < channels.Count; i++) {

            channels[i].synth.root = score.root;

            if (channels[i].index >= channels[i].clef.tones.Count && channels[i].synth.audioSource.isPlaying) {
                ReplayChannel(channels[i]);
                print("Replaying");
            }

            if (Input.GetKeyDown(KeyCode.Space) && !channels[i].synth.audioSource.isPlaying) {
                timeInterval = 0f;
                subdividedIndex = 0;
                PlayChannel(channels[i]);
                print("Playing");
            }

            else if (Input.GetKeyDown(KeyCode.Space) && channels[i].synth.audioSource.isPlaying) {
                StopChannel(channels[i]);
                print("Finished");
            }

            if (Input.GetKeyDown(KeyCode.N)) {
                if (isEditing && i == editingChannel) {
                    if (i == 0) {
                        score.bass = Score.GetRandomBar(score.bars, score.bass);
                    }
                    else if (i == 1) {
                        score.treble = Score.GetRandomBar(score.bars, score.treble);
                    }
                    // channels[i].clef = Score.GetRandomBar(score.bars);

                    // Need to make it so these automatically get from treble and bass.
                    score.SetNodesFromScore(score.treble, score.trebleNodes);
                    score.SetNodesFromScore(score.bass, score.bassNodes);
                    //score.SetNodesFromScore(channels[1].clef, score.trebleNodes);
                    //score.SetNodesFromScore(channels[0].clef, score.bassNodes);

                    StopChannel(channels[i]);
                    PlayChannel(channels[i]);
                }
                else {

                    for (int j = 0;  j < score.treble.tones.Count;  j++) {
                        // score.treble.tones[j] = (Random.Range(0f, 1f) > 0.85f) ? (Tone)(((int)score.treble.tones[j] + 2) % (int)Tone.toneCount) : score.treble.tones[j];
                    }

                }

                
            }

            if (channels[i].synth.audioSource.isPlaying) {
                WhilePlayingChannel(channels[i]);
            }
        }

    }

    void PlayChannel(Channel channel) {
        Score.PrintClef(channel.clef);
        channel.synth.audioSource.Play();
        channel.synth.newKey = true;
        channel.index = 0;
    }

    void StopChannel(Channel channel) {
        channel.synth.audioSource.Stop();
    }

    void ReplayChannel(Channel channel) {
        channel.index = 0;
    }

    public int subdividedIndex;
    public float timeInterval = 0f;
    public int maxIndex;

    float barLength = 4f;

    void WhilePlayingChannel(Channel channel) {

        // float timeInterval = (float)AudioSettings.dspTime - channel.synth.startTime;
        // Need to fix this to be a bit more synchronous.
        // Figure out which note of the lowest subdivision that we're in.

        int channelSubdivisionIndex = 0;
        for (int i = 0; i < channel.clef.lengths.Count; i++) {
            channelSubdivisionIndex += (int)(Score.LengthMultipliers[channel.clef.lengths[i]] * barLength);
            if (channelSubdivisionIndex > subdividedIndex) {
                if (channel.index != i) {
                    channel.synth.tone = channel.clef.tones[i];
                    channel.synth.newKey = true;
                    channel.index = i;
                }
                break;
            }
        }
    }

        //    // Check if we need to move to the next note
        //Value length = channel.sheet.lengths[channel.index];
        //float noteLength = Score.LengthMultipliers[length];

        //if (channel.timeInterval >= noteLength * secondsPerQuarterNote) {
        //    channel.synth.tone = channel.sheet.tones[channel.index]; // This feels a little backwards, like I should be incrementing first then changing notes right?
        //    channel.synth.newKey = true;
        //    channel.timeInterval -= noteLength * secondsPerQuarterNote;
        //    channel.index++;
        //}

        //    // float timeInterval = (float)AudioSettings.dspTime - channel.synth.startTime;
        //timeInterval += Time.deltaTime; // Need to fix this to be a bit more synchronous.
        //if (timeInterval >= 4f) {
        //    timeInterval -= 4f;
        //}

        //// 
        //float index = (int)(timeInterval / noteLength);

        //int channelIndex = 0;

        //float channelNoteTime = 0f;
        //for (int i = 0; i < channel.index; i++) {
        //    float channelNoteTime += Score.LengthMultipliers[channel.sheet.lengths[i]];
        //}

}
