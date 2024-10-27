using UnityEngine;
using UnityEngine.Audio;

namespace Core3lb
{
    public class AudioMixerFixer : MonoBehaviour
    {
        [Header("Defaults")]
        [CoreEmphasize]
        public AudioMixerSnapshot mySnapShot;
        public float transitionTime = 1;

        [Header("DoWeighted")]
        public AudioMixerSnapshot[] snapshots;
        public float[] weights;
        public float transitionTimeWeighted = 1;

        [CoreButton]
        public void _TransitionTo()
        {
            mySnapShot.TransitionTo(transitionTime);
        }

        public void _TransitionToChg(AudioMixerSnapshot mySnap)
        {
            mySnap.TransitionTo(transitionTime);
        }

        public void _WeightedTransitions()
        {
            GroKitAudioManager.instance.masterMixer.TransitionToSnapshots(snapshots, weights, transitionTimeWeighted);
        }
    }
}