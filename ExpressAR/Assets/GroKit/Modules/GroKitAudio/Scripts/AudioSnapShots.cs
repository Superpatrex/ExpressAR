using UnityEngine;
using UnityEngine.Audio;

namespace Core3lb
{
    public class AudioSnapShots : MonoBehaviour
    {
        public AudioMixer audioMixer; // Reference to your Audio Mixer
        public AudioMixerSnapshot mySnapShot;
        public float transitionTime = 0.5f; // The time it takes to transition to this snapshot


        public void _SwitchSnapshot()
        {
            if (audioMixer != null)
            {
                audioMixer.FindSnapshot(mySnapShot.name)?.TransitionTo(transitionTime); // Switch to the specified snapshot with a transition time of 0.5 seconds
            }
            else
            {
                if (GroKitAudioManager.instance.masterMixer)
                {
                    Debug.LogError("No default defaultmixer set in UnityAudio3lbManager");
                }
                GroKitAudioManager.instance.masterMixer.FindSnapshot(mySnapShot.name)?.TransitionTo(transitionTime);
            }
        }


        public void _SwitchSnapshotName(AudioMixerSnapshot change)
        {
            if (audioMixer != null)
            {
                audioMixer.FindSnapshot(change.name)?.TransitionTo(transitionTime); // Switch to the specified snapshot with a transition time of 0.5 seconds
            }
            else
            {
                if (GroKitAudioManager.instance.masterMixer)
                {
                    Debug.LogError("No default defaultmixer set in UnityAudio3lbManager");
                }
                GroKitAudioManager.instance.masterMixer.FindSnapshot(mySnapShot.name)?.TransitionTo(transitionTime);
            }
        }

        public void _SwitchToDefault()
        {
            if(GroKitAudioManager.instance.defaultSnapshot)
            {
                Debug.LogError("No default snapshot set in UnityAudio3lbManager");
            }
            _SwitchSnapshotName(GroKitAudioManager.instance.defaultSnapshot);
        }

    }
}
