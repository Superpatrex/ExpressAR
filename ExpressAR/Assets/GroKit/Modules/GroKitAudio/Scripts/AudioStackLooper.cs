using System.Collections.Generic;
using UnityEngine;

namespace Core3lb
{
    public class AudioStackLooper : MonoBehaviour
    {
        [CoreHeader("Stack Looper")]
        [CoreRequired]
        public SOGroKitAudioStack audioClipData;
        private List<AudioPooler> audioComponents;
        private List<float> audioTimes = new List<float>();
        [CoreReadOnly]
        public bool isPlaying = false;

        public void Start()
        {
            if (!audioClipData.isLooper)
            {
                Debug.LogError("This is not a looper, please use UnityAudioClipPlayer instead", gameObject);
                return;
            }

            GenerateAudioSources();
        }

        [CoreButton]
        public void _StartAudio()
        {
            isPlaying = true;
            for (int i = 0; i < audioClipData.clipStack.Length; i++)
            {
                audioComponents[i].mySource.volume = audioClipData.clipStack[i].template.volume;
            }
        }


        protected void _StopAllAudio()
        {
            foreach (var item in audioComponents)
            {
                item._FadeOutAndStop();
            }
        }

        [CoreButton]
        public void _StopAudio()
        {
            isPlaying = false;
            _StopAllAudio();
        }

        public void GenerateAudioSources()
        {
            for (int i = 0; i < audioClipData.clipStack.Length; i++)
            {
                GameObject go = new GameObject();
                go.gameObject.name = "SoundInstance";
                AudioPooler audio = new AudioPooler();
                audio = go.AddComponent<AudioPooler>();
                audio.mySource = go.GetComponent<AudioSource>();
                audio.mySource.CopyFrom(audioClipData.clipStack[i].template);
                audioComponents.Add(audio);
                audioTimes.Add(0);
            }
        }

        public void FixedUpdate()
        {
            if(isPlaying)
            {
                ProcessAudio();
            }

            //Check if the audio is playing
        }



        public void ProcessAudio()
        {
            for (int i = 0; i < audioComponents.Count; i++)
            {
                if (!audioComponents[i].mySource.isPlaying)
                {
                    audioTimes[i] += Time.deltaTime;
                    if (audioTimes[i] >= audioClipData.clipStack[i].delay)
                    {
                        audioComponents[i].mySource.clip = audioClipData.clipStack[i].myClips[Random.Range(0, audioClipData.clipStack[i].myClips.Length)];
                        audioComponents[i].mySource.Play();
                        audioTimes[i] = 0;
                    }
                }
            }
        }
    }
}
