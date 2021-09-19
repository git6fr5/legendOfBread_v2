using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Note = Score.Note;
using Tone = Score.Tone;
using Value = Score.NoteLength;
using Shape = Wave.Shape;
using Parameters = Wave.Parameters;
using Sheet = Score.Sheet;

[RequireComponent(typeof(AudioSource))]
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

    public AudioSource audioSource;

    //BPM.
    [Space(5)] [Header("BPM")]
    [SerializeField] public Knob BPMKnob;
    [SerializeField] [ReadOnly] protected int BPM;
    [SerializeField] [ReadOnly] protected int minBPM = 30;
    [SerializeField] [ReadOnly] protected int maxBPM = 240;
    [HideInInspector] protected float secondsPerQuarterNote;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        channels.Add(new Channel(Score.GetBar(), synthA));
        channels.Add(new Channel(Score.GetBar(), synthB));
    }

    void Update() {

        BPM = (int)(BPMKnob.value * (maxBPM - minBPM)) + minBPM;
        secondsPerQuarterNote = 60f / BPM;    

        if (Input.GetKeyDown(KeyCode.Space) && !audioSource.isPlaying) {
            for (int i = 0; i < channels.Count; i++) {
                PlayChannel(channels[i]);
            }
            audioSource.Play();
            print("Playing");
        }

        else if (Input.GetKeyDown(KeyCode.Space) && audioSource.isPlaying) {
            for (int i = 0; i < channels.Count; i++) {
                StopChannel(channels[i]);
            }
            audioSource.Stop();
            print("Finished");
        }

        if (audioSource.isPlaying) {
            for (int i = 0; i < channels.Count; i++) {
                if (channels[i].index >= channels[i].sheet.tones.Count) {
                    ReplayChannel(channels[i]);
                    print("Replaying");
                }
                WhilePlayingChannel(channels[i]);
            }
        }

    }

    void OnAudioFilterRead(float[] data, int channels) {

        for (int i = 0; i < data.Length; i++) {
            data[i] = 0f;
        }

        for (int i = 0; i < this.channels.Count; i++) {

            float[] dataPacket = this.channels[i].synth.GetData(data.Length, channels);
            for (int j = 0; j < data.Length; j++) {
                data[j] += dataPacket[j];
            }

        }

    }

    void PlayChannel(Channel channel) {
        Score.PrintSheet(channel.sheet);
        // channel.synth.audioSource.Play();
        channel.synth.newKey = true;
        channel.index = 0;
    }

    void StopChannel(Channel channel) {
        // channel.synth.audioSource.Stop();
    }

    void ReplayChannel(Channel channel) {
        channel.index = 0;
    }

    void WhilePlayingChannel(Channel channel) {

        // float timeInterval = (float)AudioSettings.dspTime - channel.synth.startTime;
        channel.timeInterval += Time.deltaTime;

        // Check if we need to move to the next note
        Value length = channel.sheet.lengths[channel.index];
        float noteLength = Score.LengthMultipliers[length];

        if (channel.timeInterval >= noteLength * secondsPerQuarterNote) {
            channel.synth.tone = channel.sheet.tones[channel.index];
            channel.synth.newKey = true;
            channel.timeInterval = 0f;
            channel.index++;
        }
    }

}
