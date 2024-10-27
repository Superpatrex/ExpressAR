using UnityEngine;
using UnityEngine.Events;

namespace Core3lb
{
    public class AudioStatusEvents : MonoBehaviour
    {
        public AudioSource audioSource;
        public bool isChecking;
        public float checkTime = 0.1f;


        public UnityEvent startedAudio;
        public UnityEvent stoppedAudio;

        bool playing;
        float timer;

        private void FixedUpdate()
        {
            if (isChecking)
            {
                timer += Time.fixedDeltaTime;
                if (timer > checkTime)
                {
                    if (!playing && audioSource.isPlaying)
                    {
                        playing = true;
                        startedAudio.Invoke();
                    }
                    else if (playing && !audioSource.isPlaying)
                    {
                        playing = false;
                        stoppedAudio.Invoke();
                    }
                    timer = 0;
                }
            }
        }

        public void _isChecking(bool chg)
        {
            isChecking = chg;
        }
    }

}
