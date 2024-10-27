using UnityEngine;
using UnityEngine.Audio;

namespace Core3lb
{
    public class AudioFadeOutWithMixer : MonoBehaviour
    {
        [CoreRequired]
        public AudioMixerGroup mixerGroup;
        public bool fadeoutOnDisable = false;
        public float fadeOutTime = .06f;

        public void OnDisable()
        {
            _FadeOutMixer();
        }

        public void _FadeOutMixer()
        {
            foreach (var item in GroKitAudioManager.audioInstances)
            {
                if (item.activeInHierarchy)
                {
                    var holder = item.GetComponent<AudioPooler>();
                    if (holder.mySource.outputAudioMixerGroup == mixerGroup)
                    {
                        holder.fadeTime = fadeOutTime;
                        holder._FadeOutAndStop();
                    }
                }
            }
        }
    }
}
