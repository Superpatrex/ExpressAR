using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using AICoreSimpleJSON;

namespace AICore3lb
{
    /// <summary>
    /// OpenAI Image Gen / Dall-e-3
    /// </summary>
    public class OpenAI_ImageGen : BaseAI_ImageGen
    {
        public string model = "dall-e-3";
        protected override IEnumerator DoImagePromptRequest(string prompt)
        {
            string starterPrompt = prompt;
            AIStarted(starterPrompt);
            prompt = addToImagePrompt + " " + prompt;
            string url = "https://api.openai.com/v1/images/generations";
            string jsonPayload = $"{{\"model\": \"{model}\", \"prompt\": \"{prompt}\", \"n\": 1, \"size\": \"{width}x{height}\"}}";

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            AIStarted(prompt);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error + " Error is: " + request.downloadHandler.text);
                AIFailed("Error: " + request.error);
            }
            else
            {
                var parsedData = JSON.Parse(request.downloadHandler.text);
                textOutput = request.downloadHandler.text;
                if (showDebugs)
                {
                    Debug.LogError("URL IS " + request.downloadHandler.text);
                    Debug.LogError(parsedData["data"][0]["url"]);
                }
                AIStarted(starterPrompt);
                StartCoroutine(DownloadAndApplyTexture(parsedData["data"][0]["url"]));
            }
        }
    }
}
