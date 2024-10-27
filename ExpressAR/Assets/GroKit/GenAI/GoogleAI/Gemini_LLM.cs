using UnityEngine;
using AICoreSimpleJSON;
using UnityEngine.Networking;
using System.Collections;
using System.Text;


namespace AICore3lb
{
    public class Gemini_LLM : BaseAI_LLM
    {
        private readonly string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";


        [AICoreHeader("Gemini Settings")]
        [SerializeField] float temperature = 0.9f;
        [SerializeField] int topK = 1;
        [SerializeField] int topP = 1;
        [SerializeField] int maxOutputTokens = 500;

        void BuildJsonString(StringBuilder contentsBuilder, string role, string text, Texture2D image = null)
        {
            contentsBuilder.Append("{");
            contentsBuilder.AppendFormat("\"role\": \"{0}\",", role);
            contentsBuilder.Append("\"parts\": [");
            contentsBuilder.Append("{");
            contentsBuilder.AppendFormat("\"text\": \"{0}\"", text);
            contentsBuilder.Append("}");

            if (image != null)
            {
                contentsBuilder.Append(",{");
                contentsBuilder.Append("\"inlineData\": {");
                contentsBuilder.Append("\"mimeType\": \"image/png\",");
                contentsBuilder.AppendFormat("\"data\": \"{0}\"", ConvertTextureToBase64(image));
                contentsBuilder.Append("}");
                contentsBuilder.Append("}");
            }

            contentsBuilder.Append("]");
            contentsBuilder.Append("},");
        }

        protected override IEnumerator DoLLMPrompt(string promptText = "Are you Online?", Texture2D image = null)
        {
            StringBuilder contentsBuilder = new StringBuilder();
            contentsBuilder.Append("\"contents\": [");
            AIStarted(promptText);

            if (AIConversation)
            {
                for (int i = 0; i < AIConversation.systemConversationList.Count; i++)
                {
                    BuildJsonString(contentsBuilder, "user", AIConversation.conversationList[i].user);
                    BuildJsonString(contentsBuilder, "model", AIConversation.conversationList[i].assistant);
                }
                for (int i = 0; i < AIConversation.conversationList.Count; i++)
                {
                    BuildJsonString(contentsBuilder, "user", AIConversation.conversationList[i].user);
                    BuildJsonString(contentsBuilder, "model", AIConversation.conversationList[i].assistant);
                }
                if (AIConversation.overrideSystemPrompt)
                {
                    systemPrompt = AIConversation.systemPromptOverride;
                }

            }

            BuildJsonString(contentsBuilder, "user", promptText, image);

            contentsBuilder.Append("],"); // Close the contents array

            string jsonBody = "{" + $@"
            ""systemInstruction"": {{
                ""role"": ""user"",
                ""parts"": [
                  {{
                    ""text"": ""{systemPrompt}""
                  }}
                ]
              }}," + contentsBuilder.ToString() + $@"
                ""generationConfig"": {{
                    ""temperature"": {temperature},
                    ""topK"": {topK},
                    ""topP"": {topP},
                    ""maxOutputTokens"": {maxOutputTokens},
                    ""stopSequences"": []
                }},
                ""safetySettings"": [
                    {{
                        ""category"": ""HARM_CATEGORY_HARASSMENT"",
                        ""threshold"": ""BLOCK_ONLY_HIGH""
                    }},
                    {{
                        ""category"": ""HARM_CATEGORY_HATE_SPEECH"",
                        ""threshold"": ""BLOCK_ONLY_HIGH""
                    }},
                    {{
                        ""category"": ""HARM_CATEGORY_SEXUALLY_EXPLICIT"",
                        ""threshold"": ""BLOCK_ONLY_HIGH""
                    }},
                    {{
                        ""category"": ""HARM_CATEGORY_DANGEROUS_CONTENT"",
                        ""threshold"": ""BLOCK_ONLY_HIGH""
                    }}
                ]
            }}";

            using (UnityWebRequest webRequest = new UnityWebRequest(url + "?key=" + apiKey, "POST"))
            {
                byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonBody);
                webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
                webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    AIFailed("Request Failure " + webRequest.error + " Response:" + webRequest.downloadHandler.text);
                }
                else
                {
                    string responseText = webRequest.downloadHandler.text;
                    JSONNode parsedData = JSONNode.Parse(responseText);
                    string outputText = parsedData["candidates"][0]["content"]["parts"][0]["text"];
                    if (showDebugs)
                    {
                        Debug.LogError(promptText + " : " + responseText);
                    }
                    textOutput = outputText;
                    AddConversationHistory(promptText, textOutput);
                    AISuccess(textOutput);
                }
            }
        }
    }
}

