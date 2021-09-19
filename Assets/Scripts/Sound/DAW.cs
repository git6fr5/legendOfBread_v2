using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Note = Score.Note;
using Tone = Score.Tone;
using Value = Score.NoteLength;
using Shape = Wave.Shape;
using Parameters = Wave.Parameters;
using Sheet = Score.Sheet;

public class DAW : MonoBehaviour {

    public Synth synthA;
    public Synth synthB;

    public class Channel {
        public Sheet sheet;
        public Synth synth;
        public int index;
        public float timeInterval;

        public Channel(Sheet _sheet, Synth _synth) {
            timeInterval = 0f;
            index = 0;
            sheet = _sheet;
            synth = _synth;
            synth.isActive = false;
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

    void Awake() {
        channels.Add(new Channel(Score.GetBasicBar(), synthA));
        channels.Add(new Channel(Score.GetRandomBar(), synthB));
    }

    void Update() {

        BPM = (int)(BPMKnob.value * (maxBPM - minBPM)) + minBPM;
        secondsPerQuarterNote = 60f / BPM;


        for (int i = 0; i < channels.Count; i++) {

            if (channels[i].index >= channels[i].sheet.tones.Count && channels[i].synth.audioSource.isPlaying) {
                ReplayChannel(channels[i]);
                print("Replaying");
            }

            if (Input.GetKeyDown(KeyCode.Space) && !channels[i].synth.audioSource.isPlaying) {
                PlayChannel(channels[i]);
                print("Playing");
            }

            else if (Input.GetKeyDown(KeyCode.Space) && channels[i].synth.audioSource.isPlaying) {
                StopChannel(channels[i]);
                print("Finished");
            }

            if (Input.GetKeyDown(KeyCode.N)) {
                if (i == editingChannel) {
                    channels[i].sheet = Score.GetRandomBar();
                }
                StopChannel(channels[i]);
                PlayChannel(channels[i]);
            }

            if (channels[i].synth.audioSource.isPlaying) {
                WhilePlayingChannel(channels[i]);
            }
        }

    }

    void PlayChannel(Channel channel) {
        Score.PrintSheet(channel.sheet);
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

    void WhilePlayingChannel(Channel channel) {

        // float timeInterval = (float)AudioSettings.dspTime - channel.synth.startTime;
        channel.timeInterval += Time.deltaTime; // Need to fix this to be a bit more synchronous.

        // Check if we need to move to the next note
        Value length = channel.sheet.lengths[channel.index];
        float noteLength = Score.LengthMultipliers[length];

        if (channel.timeInterval >= noteLength * secondsPerQuarterNote) {
            channel.synth.tone = channel.sheet.tones[channel.index]; // This feels a little backwards, like I should be incrementing first then changing notes right?
            channel.synth.newKey = true;
            channel.timeInterval = 0f;
            channel.index++;
        }
    }

}
