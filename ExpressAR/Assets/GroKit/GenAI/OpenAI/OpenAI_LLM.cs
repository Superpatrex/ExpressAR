using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.Networking;
using UnityEngine;
using AICoreSimpleJSON;
using System.Collections;


namespace AICore3lb
{
    public class OpenAI_LLM : BaseAI_LLM
    {
        private const string OPENAI_API_URL = "https://api.openai.com/v1/chat/completions";
        [AICoreHeader("OpenAI Settings")]
        public float temp = .7f;
        public int maxTokens = 500;
        public string model = "gpt-4o";

        private string BuildMessagesArrayJson(BaseAI_Conversation conv, string prompt, string image64 = null)
        {
            List<OpenAIMessage> messages = new List<OpenAIMessage>();
            string holdingSystemPrompt = systemPrompt;
            if (conv != null)
            {
                if(conv.overrideSystemPrompt)
                {
                    holdingSystemPrompt = conv.systemPromptOverride;
                }
                foreach (var item in conv.systemConversationList)
                {
                    messages.Add(new OpenAIMessage("user", AICoreExtensions.SanitizeForJson(item.user)));
                    messages.Add(new OpenAIMessage("assistant", AICoreExtensions.SanitizeForJson(item.assistant)));
                }
                foreach (var item in conv.conversationList)
                {
                    messages.Add(new OpenAIMessage("user", AICoreExtensions.SanitizeForJson(item.user)));
                    messages.Add(new OpenAIMessage("assistant", AICoreExtensions.SanitizeForJson(item.assistant)));
                }
            }
            messages.Add(new OpenAIMessage("system", AICoreExtensions.SanitizeForJson(systemPrompt)));
            messages.Add(new OpenAIMessage("user", AICoreExtensions.SanitizeForJson(prompt),image64));

            return ConvertToOpenAIJson(messages, model, temp, maxTokens);
        }

        protected override IEnumerator DoLLMPrompt(string promptText, Texture2D image = null)
        {
            AIStarted(promptText);
            UnityWebRequest www = new UnityWebRequest(OPENAI_API_URL, "POST");
            // Set the request headers
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + apiKey);

            string image64 = null;
            if (image != null)
            {
                image64 = ConvertTextureToBase64(image);
            }

            // Add conversation stuff and image
            string json = BuildMessagesArrayJson(AIConversation, promptText, image64);
            if(showDebugs)
            {
                Debug.LogError($"JSON Created {json}");
            }
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"OpenAI_LLM Error : {www.error} Response: {www.downloadHandler.text}");
                AIFailed(promptText);
            }
            else
            {
                if (showDebugs)
                {
                    Debug.LogError("Response: " + www.downloadHandler.text, gameObject);
                }
                JSONNode jsonResponse = JSON.Parse(www.downloadHandler.text);
                string assistantResponse = jsonResponse["choices"][0]["message"]["content"];
                AddConversationHistory(promptText, assistantResponse);
                AISuccess(assistantResponse);
            }
        }

        public string ConvertToOpenAIJson(List<OpenAIMessage> messages, string model, float temp, int tokens)
        {
            JSONObject json = new JSONObject();
            json["model"] = model;
            json["temperature"] = temp;
            json["max_tokens"] = tokens;

            JSONArray messagesArray = new JSONArray();
            foreach (var message in messages)
            {
                JSONObject messageJson = new JSONObject();
                messageJson["role"] = message.role;
                //Image data stuff
                if (message.imageData == null)
                {
                    messageJson["content"] = message.content;
                }
                else
                {
                    JSONArray contentArray = new JSONArray();
                    JSONObject textObject = new JSONObject();
                    textObject["type"] = "text";
                    textObject["text"] = message.content;
                    contentArray.Add(textObject);

                    JSONObject imageObject = new JSONObject();
                    imageObject["type"] = "image_url";
                    JSONObject imageUrlObject = new JSONObject();
                    imageUrlObject["url"] = $"data:image/jpeg;base64,{message.imageData}";
                    imageObject["image_url"] = imageUrlObject;
                    contentArray.Add(imageObject);

                    messageJson["content"] = contentArray;
                }

                messagesArray.Add(messageJson);
            }

            json["messages"] = messagesArray;
            return json.ToString();
        }
    }

    [Serializable]
    public class OpenAIMessage
    {
        public string role;
        public string content;
        public string imageData = null;

        public OpenAIMessage(string role, string content, string imageData = null)
        {
            this.role = role;
            this.content = content;
            this.imageData = imageData;
        }
    }
}