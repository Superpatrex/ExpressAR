using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AICore3lb
{
    public class ElevenLabs_TTS : BaseAI_TextToSpeech
    {
        [AICoreHeader("Eleven Labs")]
        public float stability = .5f;
        public float similarityBoost = .5f;
        private string baseUrl = "https://api.elevenlabs.io/v1/text-to-speech/";

        protected override IEnumerator ProcessVoice(string whatToSay, string voice)
        {
            if (string.IsNullOrEmpty(voice))
            {
                Debug.LogError("Voice Cannot be Null");
                yield break;
            }

            string url = baseUrl + voice;

            // Prepare JSON body as a string

            string jsonData = $"{{\"text\":\"{whatToSay}\", \"model_id\":\"eleven_monolingual_v1\", \"voice_settings\":{{\"stability\":{stability}, \"similarity_boost\":{similarityBoost}}}}}";
            if (showDebugs)
            {
                Debug.LogError($"Sent to 11Labs:{jsonData} to {url}");
            }
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG); // Directly request the audio clip
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "audio/mpeg");
            request.SetRequestHeader("xi-api-key", apiKey);

            AIStarted(whatToSay);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                //Send Request again find why it failed
                UnityWebRequest getError = new UnityWebRequest(url, "POST");
                getError.uploadHandler = new UploadHandlerRaw(bodyRaw);
                getError.downloadHandler = new DownloadHandlerBuffer();
                getError.SetRequestHeader("Content-Type", "application/json");
                getError.SetRequestHeader("xi-api-key", apiKey);
                yield return getError.SendWebRequest();
                Debug.LogError("Error: " + getError.error + " Response is: " + getError.downloadHandler.text);
                AIFailed(whatToSay);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                if (audioClip != null)
                {
                    _PlayAudio(audioClip);
                    AISuccess(whatToSay);
                }
                else
                {
                    Debug.LogError("Failed to create audio clip from downloaded data.");
                    AIFailed(whatToSay);
                }
            }
        }
    }
}
