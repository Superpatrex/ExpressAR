using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AICoreSimpleJSON;
using System.Linq;

namespace AICore3lb
{
    public class OpenAI_STT : BaseAI_SpeechToText
    {
        public override IEnumerator PostAudioFileCoroutineData(AudioClip myClip)
        {
            myClip = AISaveWavUtil.TrimSilenceFromEnd(myClip);
            if(showDebugs)
            {
                Debug.LogError($"Started Processing {myClip.length}");
            }
            byte[] fileBytes = AISaveWavUtil.AudioClipToByteArray(myClip);
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("file", fileBytes, "transcript.mp3", "audio/wav"));
            formData.Add(new MultipartFormDataSection("model", "whisper-1"));

            UnityWebRequest request = UnityWebRequest.Post("https://api.openai.com/v1/audio/transcriptions", formData);
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.downloadHandler.text);
                aiFailed.Invoke(request.downloadHandler.text);
            }
            else
            {
                //Debug.Log("Response: " + request.downloadHandler.text);
                var N = JSON.Parse(request.downloadHandler.text);
                AISuccess(N["text"]);
            }
        }

      
    }
}
