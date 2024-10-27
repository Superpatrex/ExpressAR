using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Core3lb
{
    public class LoopedSource : MonoBehaviour
    {
        [CoreHeader("Required")]
        [CoreRequired]
        public AudioSource mySource;
        float startVolume;
        [CoreHeader("Settings")]
        public bool PlayOnAwakeWithFade;
        public bool fadeInOnEnable;
        //ADD FADE IN AND FADE OUT!
        public float fadeTime = .06f;

        [CoreHeader("Distance Adjust")]
        public float maxChange = 200;
        public float distanceChangeTime = .5f;
        bool isFadingDown;
        bool isFadingUp;
        IEnumerator myCoroutine;


        public UnityEvent fadeInCalled;
        public UnityEvent fadeOutCalled;

        public bool showDebugs;


        public void Awake()
        {
            startVolume = mySource.volume;
            //FORCE SOURCE TO LOOP!!!
            mySource.loop = true;
            if (PlayOnAwakeWithFade)
            {
                _PlayFadeIn();
            }
        }

        private void OnEnable()
        {
            if (fadeInOnEnable)
            {
                _PlayFadeIn();
            }
        }

        public void _DistanceAdjustChange(float chg)
        {
            StartCoroutine(mySource.AnimateDistanceChange(chg, distanceChangeTime));
        }

        public IEnumerator FadeInAndOut(AudioClip clip)
        {
            yield return StartCoroutine(mySource.FadeOut(fadeTime, true, FadeInComplete));
            mySource.clip = clip;
            yield return StartCoroutine(mySource.FadeIn(fadeTime, startVolume, FadeInComplete));
        }


        public void _PlayFadeInClip(AudioClip clip)
        {
            if (mySource.isPlaying)
            {
                StartCoroutine(FadeInAndOut(clip));
            }
            else
            {
                mySource.clip = clip;
                _PlayFadeIn();
            }
        }

        [CoreButton]
        public void _PlayFadeIn()
        {
            if (isFadingUp)
            {
                return;
            }
            isFadingUp = true;
            isFadingDown = false;
            fadeInCalled.Invoke();
            if (myCoroutine != null)
            {
                StopCoroutine(myCoroutine);
                myCoroutine = null;
            }
            myCoroutine = mySource.FadeIn(fadeTime,1, FadeInComplete);
            StartCoroutine(myCoroutine);
            if (showDebugs)
            {
                Debug.LogError(name + "---- Called FADE IN", gameObject);
            }
        }


        public void _StopNow()
        {
            mySource.Stop();
        }

        [CoreButton]
        public void _StopFadeOut()
        {
            if (isFadingDown)
            {
                return;
            }
            isFadingDown = true;
            isFadingUp = false;
            fadeOutCalled.Invoke();
            if (showDebugs)
            {
                Debug.LogError(name + " ---- Called FADE OUT", gameObject);
            }
            if (myCoroutine != null)
            {
                StopCoroutine(myCoroutine);
                myCoroutine = null;
            }
            myCoroutine = mySource.FadeOut(fadeTime, false, FadeOutDone);
            StartCoroutine(myCoroutine);
        }

        public void _FadeOutFast()
        {
            if (isFadingDown)
            {
                return;
            }
            isFadingDown = true;
            isFadingUp = false;
            fadeOutCalled.Invoke();
            if (showDebugs)
            {
                Debug.LogError(name + " ---- Called FADE OUT FAST",gameObject);
            }
            if (myCoroutine != null)
            {
                StopCoroutine(myCoroutine);
                myCoroutine = null;
            }
            myCoroutine = mySource.FadeOut(.06f, false, FadeOutDone);
        }


        public virtual void FadeOutDone()
        {
            mySource.Stop();
        }

        public virtual void FadeInComplete()
        {
            //NothingFOrnow
        }
    }
}
