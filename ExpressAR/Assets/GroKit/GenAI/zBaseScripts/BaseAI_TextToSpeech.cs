using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using CoreSecurity;

namespace AICore3lb
{
    public class BaseAI_TextToSpeech : BaseAI
    {
        [AICoreHeader("Text To Speech")]
        [AICoreRequired]
        public AudioSource speakerSource;
        public string testPrompt = "Today is a wonderful day to build something people love!";
        public bool useScriptable;
        [AICoreHideIf("useScriptable")]
        public string voiceID;
        [AICoreShowIf("useScriptable")]
        public SO_StringEncrypted voiceSO;

        public UnityEvent<AudioClip> audioPlayStarted;



        [AICoreButton]
        public virtual void TestSpeak()
        {
            _Speak(testPrompt);
        }

        public void _StopAudio()
        {
            speakerSource.Stop();
        }

        public virtual void _Speak(string chg)
        {
            chg = AICoreExtensions.SanitizeForJson(chg);
            StartCoroutine(ProcessVoice(chg, GetVoiceID));
        }

        public virtual string GetVoiceID
        {
            get
            {
                if (useScriptable)
                {
                    return voiceSO.GetString;
                }
                return voiceID;
            }
        }

        public void _ChangeVoice(string voiceChange)
        {
            useScriptable = false;
            voiceID = voiceChange;
        }

        public void _ChangeVoice(SO_StringEncrypted goString )
        {
            useScriptable = true;
            voiceSO = goString;
        }


        protected virtual IEnumerator ProcessVoice(string input, string voice)
        {
            yield return null;
        }

        public void ConvertPcmAndPlay(byte[] pcmData, int sampleRate = 24000, int channels = 1)
        {
            int samples = pcmData.Length / 2; // Each sample is 2 bytes (16-bit PCM)
            float[] floatData = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                short pcmValue = (short)(pcmData[i * 2] | (pcmData[i * 2 + 1] << 8));
                floatData[i] = pcmValue / 32768.0f; // Convert to float in range [-1.0, 1.0]
            }
            AudioClip audioClip = AudioClip.Create("TextToSpeech", samples, channels, sampleRate, false);
            audioClip.SetData(floatData, 0);
            _PlayAudio(audioClip);
        }



        public virtual void _PlayAudio(AudioClip whatClip)
        {
            if (showDebugs)
            {
                Debug.LogError("Audio Length: " + whatClip.length);
            }
            if(speakerSource)
            {
                speakerSource.clip = whatClip;
                speakerSource.Play();
            }
            audioPlayStarted.Invoke(whatClip);
        }

        /// <summary>
        /// This will bypass and speak through this system
        /// </summary>
        /// <param name="url"></param>
        public void _PlayAudioFromURL(string url)
        {
            StartCoroutine(LoadAndPlayURL(url));
        }
        /// <summary>
        /// Load and play from a URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual IEnumerator LoadAndPlayURL(string url,AudioType type = AudioType.MPEG)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, type))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                    if (audioClip != null)
                    {
                        _PlayAudio(audioClip);
                    }
                    else
                    {
                        Debug.LogError("Failed to load audio clip from file.");
                    }
                }
            }
        }
    }
}
