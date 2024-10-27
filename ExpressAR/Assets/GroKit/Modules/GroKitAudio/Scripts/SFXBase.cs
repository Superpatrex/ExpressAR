using UnityEngine;
using UnityEngine.Audio;

namespace Core3lb
{
    public class SFXBase : MonoBehaviour
    {
        [CoreHeader("Universal Settings")]
        public bool playOnStart = false;
        [CoreHideIf("useOtherClips")]
        public AudioClip overrideClip;
        public bool is2D;
        public AudioMixerGroup overrideMixer;
        public bool onlyPlayIfActive = false;
        public Transform audioPosition = null;

        public virtual void Start()
        {
            if (audioPosition == null)
            {
                audioPosition = transform;
            }
            if (playOnStart)
            {
                _Play();
            }
        }

        [CoreButton]
        public virtual void _Play()
        {
            _PlayClip(overrideClip);
        }

        public virtual void _PlayClip(AudioClip myClip)
        {
            PlayerInternalComplex(myClip);
        }

        protected virtual void PlayerInternalComplex(AudioClip clip = null)
        {
            if (clip == null)
            {
                Debug.Log("No Clip to Play not playing", gameObject);
                return;
            }
            if (onlyPlayIfActive)
            {
                if (!gameObject.activeInHierarchy)
                {
                    Debug.Log("Not Playing Because Off and onlyPlayIfActive is true");
                    return;
                }
            }
            if (audioPosition == null)
            {
                audioPosition = transform;
            }
            if(is2D)
            {
                GroKitAudioManager.instance.PlaySFX2D(gameObject, clip, 1, 1, overrideMixer);
            }
            else
            {
                GroKitAudioManager.instance.PlayAudio3D(gameObject, clip, audioPosition, onlyPlayIfActive, 1, 1, overrideMixer);
            }

        }



        public virtual void _PlayHere(Transform where)
        {
            audioPosition = where;
            _Play();
        }
    }
}
