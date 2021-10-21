using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Note = Score.Note;
using Tone = Score.Tone;
using Value = Score.NoteLength;
using Shape = Wave.Shape;
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
    [Space(5)]
    [Header("BPM")]
    [SerializeField] public Knob BPMKnob;
    [SerializeField] [ReadOnly] protected int BPM;
    [SerializeField] [ReadOnly] protected int minBPM = 30;
    [SerializeField] [ReadOnly] protected int maxBPM = 240;
    [HideInInspector] protected float secondsPerQuarterNote;

    public int editingChannel = 1;

    public Score score;

    public string scoreFile;
    public string synthFileA;
    public string synthFileB;

    public bool isEditing;

    void Awake() {

        // Load scores.
        score.RandomScore();

        // Load channels.
        synthA.Open(synthFileA);
        channels.Add(new Channel(score.treble, synthA));

        synthB.Open(synthFileB);
        channels.Add(new Channel(score.bass, synthB));

        score.Instantiate();

    }

    void Update() {

        if (isEditing) {
            // Get BPM.
            BPM = (int)(BPMKnob.value * (maxBPM - minBPM)) + minBPM;
        }
        // Get the duration of a note with respect to the BPM.
        secondsPerQuarterNote = 60f / BPM;

        GetVolumes();

        // The time control logic.
        Tick();

        // Play the music~
        Play();

    }

    public float maxVolume = 0.5f;

    void GetVolumes() {
        for (int i = 0; i < channels.Count; i++) {
            channels[i].synth.volume = channels[i].synth.volumeKnob.value * maxVolume;
        }
    }

    private void Play() {
        channels[0].clef = score.treble;
        channels[1].clef = score.bass;

        for (int i = 0; i < channels.Count; i++) {

            channels[i].synth.root = score.root;

            if (channels[i].index >= channels[i].clef.tones.Count && channels[i].synth.audioSource.isPlaying) {
                ReplayChannel(channels[i]);
            }

            if (Input.GetKeyDown(KeyCode.Space) && !channels[i].synth.audioSource.isPlaying && isEditing) {
                timeInterval = 0f;
                subdividedIndex = 0;
                PlayChannel(channels[i]);
            }

            else if (Input.GetKeyDown(KeyCode.Space) && channels[i].synth.audioSource.isPlaying && isEditing) {
                StopChannel(channels[i]);
            }

            if (channels[i].synth.audioSource.isPlaying) {
                WhilePlayingChannel(channels[i]);
            }

        }
    }

    private void Tick() {
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

}
