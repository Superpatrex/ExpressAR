using CoreSecurity;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AICore3lb
{
    public class PlayHT_TTS : BaseAI_TextToSpeech
    {
        private string url = "https://api.play.ht/api/v2/tts/stream";
        [AICoreHeader("Play.ht")]
        public SO_StringEncrypted userId;
        //Only way to get Audio data back
        private string accept = "undefined";
        private string contentType = "application/json";

        private string outputFormat = "mp3";
        string voiceEngine = "PlayHT2.0-turbo";
        public string quality = "draft";
        public float speed = 1f;
        int sampleRate = 24000;
        public int seed = 1;
        public float temperature = 1f;
        public string emotion = "female_happy";

        protected override IEnumerator ProcessVoice(string whatToSay, string voice)
        {
            string jsonData;
            if (emotion == "" || emotion == "NONE")
            {
                jsonData = $"{{\"text\":\"{whatToSay}\",\"voice\":\"{voice}\",\"output_format\":\"{outputFormat}\",\"voice_engine\":\"{voiceEngine}\",\"quality\":\"{quality}\",\"speed\":{speed},\"sample_rate\":{sampleRate},\"seed\":{seed},\"temperature\":{temperature}}}";
            }
            else
            {
                jsonData = $"{{\"text\":\"{whatToSay}\",\"voice\":\"{voice}\",\"output_format\":\"{outputFormat}\",\"voice_engine\":\"{voiceEngine}\",\"quality\":\"{quality}\",\"speed\":{speed},\"sample_rate\":{sampleRate},\"seed\":{seed},\"temperature\":{temperature},\"emotion\":\"{emotion}\"}}";
            }
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG); // Request the audio clip directly
            request.SetRequestHeader("AUTHORIZATION", apiKey);
            request.SetRequestHeader("X-USER-ID", userId.GetString);
            request.SetRequestHeader("accept", accept);
            request.SetRequestHeader("content-type", contentType);

            AIStarted(whatToSay);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                UnityWebRequest getError = new UnityWebRequest(url, "POST");
                getError.uploadHandler = new UploadHandlerRaw(bodyRaw);
                getError.downloadHandler = new DownloadHandlerBuffer();
                getError.SetRequestHeader("AUTHORIZATION", apiKey);
                getError.SetRequestHeader("X-USER-ID", userId.GetString);
                getError.SetRequestHeader("accept", accept);
                getError.SetRequestHeader("content-type", contentType);
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
                    Debug.Log("OpenAI Length: " + audioClip.length);
                    AISuccess(whatToSay);
                }
                else
                {
                    Debug.LogError("Failed to create audio clip from downloaded data.");
                    AIFailed(whatToSay);
                }
                AISuccess(whatToSay);
            }
        }

    }
}
