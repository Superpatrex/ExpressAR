using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace Core3lb
{
    public class AudioPooler : MonoBehaviour
    {
        [CoreHelpBox("This Component is for Internal Only")]
        [CoreReadOnly]
        public bool disableWhenDone = false;
        public AudioSource mySource;
        [CoreHeader("Override")]
        public AudioMixerGroup overrideMixer;
        public AudioClip overRideClip;
        public float fadeTime = 2f;
        IEnumerator myCoroutine;


        [CoreHeader("Randomize")]
        public Vector2 randomPitch = Vector2.one;
        public Vector2 randomVolume = Vector2.one;
        bool fixPitchAfter;
        bool fixVolumeAfter;

        [CoreHeader("Distance Adjust")]
        public float maxChange = 200;
        public float distanceChangeTime = .5f;


        [CoreHeader("Debug")]
        [CoreReadOnly]
        public Transform followTarget;
        [CoreReadOnly]
        public GameObject lastCaller;
        [CoreReadOnly]
        public AudioSource sourceTemplate;

        private void Start()
        {
            if (!mySource)
            {
                mySource = GetComponent<AudioSource>();
                mySource.outputAudioMixerGroup = overrideMixer;
            }
        }
        void FixedUpdate()
        {
            if(followTarget)
            {
               transform.position = followTarget.position;
               transform.rotation = followTarget.rotation;
            }
            if (disableWhenDone)
            {
                if (!mySource.isPlaying)
                {
                    AudioPool.Despawn(gameObject);
                }
            }
            if (!mySource.isPlaying)
            {
                if (fixPitchAfter)
                {
                    ResetSource();
                }
                if (fixVolumeAfter)
                {
                    ResetSource();
                }
            }
        }
        //[CoreButton]
        public void _TestPlay()
        {
            ResetSource();
            SetOverride();
            mySource.Play();
        }

        //[CoreButton]
        public void _DistanceAdjust()
        {
            _DistanceAdjustChange(maxChange);
        }

        public void _DistanceAdjustChange(float chg)
        {
            StartCoroutine(mySource.AnimateDistanceChange(chg, distanceChangeTime));
        }

        //[CoreButton]
        public void _PlayRandomPitch()
        {
            ResetSource();
            mySource.pitch = randomPitch.Randomize();
            fixPitchAfter = true;
            SetOverride();
            mySource.Play();
        }
        //[CoreButton]
        public void _PlayRandomVolume()
        {
            ResetSource(); SetOverride();
            mySource.volume = randomVolume.Randomize();
            fixVolumeAfter = true;
            SetOverride();
            mySource.Play();
        }

        //[CoreButton]
        public void _PlayRandomVolumeNPitch()
        {
            ResetSource();
            mySource.volume = randomVolume.Randomize();
            mySource.pitch = randomPitch.Randomize();
            fixPitchAfter = true;
            fixVolumeAfter = true;
            SetOverride();
            mySource.Play();
        }
        void _PlayOneShotSFX(AudioClip myClip)
        {
            ResetSource();
            mySource.clip = myClip;
            mySource.Play();
        }
        [CoreButton]
        public void _FadeOutAndStop()
        {
            if (myCoroutine != null)
            {
                StopCoroutine(myCoroutine);
                myCoroutine = null;
            }
            myCoroutine = mySource.FadeOut(fadeTime, false, ResetSource);
            StartCoroutine(myCoroutine);
        }

        //[CoreButton]
        public void _FadeInOn()
        {
            if (myCoroutine != null)
            {
                StopCoroutine(myCoroutine);
                myCoroutine = null;
            }
            myCoroutine = mySource.FadeIn(fadeTime);
            StartCoroutine(myCoroutine);
        }


        public void SetOverride()
        {
            if (overRideClip)
            {
                mySource.clip = overRideClip;
            }
        }

        public void ResetSource()
        {
            mySource.Stop();
            if (fixVolumeAfter)
            {
                mySource.volume = 1;
                fixVolumeAfter = false;
            }
            if (fixPitchAfter)
            {
                mySource.pitch = 1;
                fixPitchAfter = false;
            }
        }
    }
}