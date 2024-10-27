using UnityEngine;
using UnityEngine.Events;

namespace Core3lb
{
    [CoreClassName("Random Interval SFX")]
    public class RandomIntervalSFX : MonoBehaviour
    {
        [CoreRequired]
        public SFXBase audioToPlay;
        [Tooltip("Event used when Audio is played")]
        public UnityEvent onPlay;
        public Vector2 intervals;

        [CoreHeader("Looping Random Audio")]
        [CoreReadOnly]
        public float setTime;
        [CoreReadOnly]
        public float timer;
        [Tooltip("This means the loop is active and plays on start")]
        public bool loopActive = false;
        public bool randomizeOnAwake = true;

        public void Awake()
        {
            if (randomizeOnAwake)
            {
                NewRandom();
            }
        }


        public void _SetLoop(bool chg)
        {
            loopActive = chg;
        }

        public void FixedUpdate()
        {
            if (loopActive)
            {
                timer += Time.deltaTime;
                if (timer > setTime)
                {
                    PlayAudio();
                    NewRandom();
                }
            }
        }

        public void PlayAudio()
        {
            onPlay.Invoke();
            if (audioToPlay)
            {
                audioToPlay._Play();
            }
        }

        public void NewRandom()
        {
            setTime = intervals.Randomize();
            timer = 0;
        }

    }
}