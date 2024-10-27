using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace AICore3lb
{
    public class OpenAI_TTS : BaseAI_TextToSpeech
    {
        [AICoreHeader("OpenAI Settings")]
        [Range(.25f,4)]
        public float speed = 1.0f;
        public string model = "tts-1";
        private const string apiUrl = "https://api.openai.com/v1/audio/speech";

        protected override IEnumerator ProcessVoice(string whatToSay, string voice)
        {
            if (string.IsNullOrEmpty(whatToSay) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(voice))
            {
                Debug.LogError("Input text, model, and voice cannot be null or empty");
                yield break;
            }

            string audioOutput = "mp3"; // UnityWebRequestMultimedia.GetAudioClip supports mp3 format

            // Prepare JSON body as a string
            string jsonData = $"{{\"model\": \"{model}\", \"input\": \"{whatToSay}\", \"voice\": \"{voice}\", \"response_format\": \"{audioOutput}\", \"speed\": {speed}}}";

            byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerAudioClip(apiUrl, AudioType.MPEG); // Request the audio clip directly
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            request.SetRequestHeader("Content-Type", "application/json");

            AIStarted(whatToSay);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                //Send Request again find why it failed
                UnityWebRequest getError = new UnityWebRequest(apiUrl, "POST");
                getError.uploadHandler = new UploadHandlerRaw(postData);
                getError.downloadHandler = new DownloadHandlerBuffer();
                getError.SetRequestHeader("Authorization", "Bearer " + apiKey);
                getError.SetRequestHeader("Content-Type", "application/json");
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
