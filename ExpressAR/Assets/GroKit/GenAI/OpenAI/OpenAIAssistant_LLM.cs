using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using AICoreSimpleJSON;

namespace AICore3lb
{

    public class OpenAIAssistant_LLM : BaseAI_LLM
    {
        //Notes
        //With this version, setup your own assistant at https://platform.openai.com/assistants
        //This does not use a base conversation everything is stored in a thread
        //It does support Images in threads but those get uploaded to your openAI storage. 


        private const string baseApiUrl = "https://api.openai.com/v1";
        public string assistantID;
 //Unity Test Assist
        [TextArea]
        [AICoreHeader("OpenAI Assistant Settings")]
        [HideInInspector]
        public bool NoConvo = true;

        private float pollSpeed = .2f;
        [AICoreReadOnly]
        public string fileImageID;
        [AICoreReadOnly]
        public string threadId;



        [AICoreButton("Manage Assistant",true)]
        void ManageAssistant()
        {
            Application.OpenURL("https://platform.openai.com/assistants");
        }

        [AICoreButton]
        public void _NewThread()
        {
            StartCoroutine(CleanThread());
        }

        public void _SetAssistant(string chg)
        {
            assistantID = chg;
        }

        public void _SetThreadID(string chg)
        {
            threadId = chg;
        }

        protected override IEnumerator DoLLMPrompt(string promptText = "Are you Online?", Texture2D image = null)
        {
            if(string.IsNullOrEmpty(assistantID))
            {
                Debug.LogError("Assistant is required");
                yield return null;
            }
            if (string.IsNullOrEmpty(threadId))
            {
                yield return StartCoroutine(CreateThread());
            }
            if (image == null)
            {
                // Normal prompt without image
                string messageUrl = $"{baseApiUrl}/threads/{threadId}/messages";
                string messageJson = $@"
                {{
                    ""role"": ""user"",
                    ""content"": [
                        {{
                            ""type"": ""text"",
                            ""text"": ""{promptText}""
                        }}
                    ]
                }}";

                UnityWebRequest messageRequest = new UnityWebRequest(messageUrl, "POST");
                byte[] messageBodyRaw = Encoding.UTF8.GetBytes(messageJson);
                messageRequest.uploadHandler = new UploadHandlerRaw(messageBodyRaw);
                messageRequest.downloadHandler = new DownloadHandlerBuffer();
                messageRequest.SetRequestHeader("Content-Type", "application/json");
                messageRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
                messageRequest.SetRequestHeader("OpenAI-Beta", "assistants=v2");

                yield return messageRequest.SendWebRequest();

                if (messageRequest.result == UnityWebRequest.Result.Success)
                {
                    if(showDebugs)
                    {
                        Debug.Log("Message sent successfully: " + messageRequest.downloadHandler.text);
                    }
                    yield return StartCoroutine(CreateRun());
                }
                else
                {
                    Debug.LogError("Error sending message: " + messageRequest.error);
                }
            }
            else
            {
                yield return StartCoroutine(UploadImageCoroutine(image));
                string messageUrl = $"{baseApiUrl}/threads/{threadId}/messages";
                string messageJson = $@"
                {{
                    ""role"": ""user"",
                    ""content"": [
                        {{
                            ""type"": ""text"",
                            ""text"": ""{promptText}""
                        }},
                        {{
                            ""type"": ""image_file"",
                            ""image_file"": {{
                                ""file_id"": ""{fileImageID}"",
                                ""detail"": ""high""
                            }}
                        }}
                    ]
                }}";

                UnityWebRequest messageRequest = new UnityWebRequest(messageUrl, "POST");
                byte[] messageBodyRaw = Encoding.UTF8.GetBytes(messageJson);
                messageRequest.uploadHandler = new UploadHandlerRaw(messageBodyRaw);
                messageRequest.downloadHandler = new DownloadHandlerBuffer();
                messageRequest.SetRequestHeader("Content-Type", "application/json");
                messageRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
                messageRequest.SetRequestHeader("OpenAI-Beta", "assistants=v2");

                yield return messageRequest.SendWebRequest();

                if (messageRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Message sent successfully: " + messageRequest.downloadHandler.text);
                    yield return StartCoroutine(CreateRun());
                }
                else
                {
                    Debug.LogError("Error sending message: " + messageRequest.error);
                }
            }
        }

        IEnumerator CreateThread()
        {
            string url = $"{baseApiUrl}/threads";
            string jsonData = "{}";

            UnityWebRequest threadRequest = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            threadRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            threadRequest.downloadHandler = new DownloadHandlerBuffer();
            threadRequest.SetRequestHeader("Content-Type", "application/json");
            threadRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            threadRequest.SetRequestHeader("OpenAI-Beta", "assistants=v2");
            yield return threadRequest.SendWebRequest();
            if (threadRequest.result == UnityWebRequest.Result.Success)
            {
                if(showDebugs)
                {
                    Debug.LogError("Thread created successfully: " + threadRequest.downloadHandler.text);
                }

                var jsonResponse = JSON.Parse(threadRequest.downloadHandler.text);
                threadId = jsonResponse["id"];
            }
            else
            {
                Debug.LogError("Error creating thread: " + threadRequest.error + ":" + threadRequest.downloadHandler.text);
                AIFailed("Failed to get thread");
            }
        }


        private IEnumerator UploadImageCoroutine(Texture2D file)
        {
            string runUrl = "https://api.openai.com/v1/files";
            byte[] fileData = DuplicateTextureInSupportedFormat(file).EncodeToPNG();

            WWWForm form = new WWWForm();
            form.AddField("purpose", "vision");
            form.AddBinaryData("file", fileData, $"{file.name}.png", "image/png");

            UnityWebRequest www = UnityWebRequest.Post(runUrl, form);
            www.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error + " " + www.downloadHandler.text);
                AIFailed("File Upload Failed");
            }
            else
            {
                string responseJson = www.downloadHandler.text;
                // Parse the response to get the file ID using SimpleJSON
                var jsonResponse = JSON.Parse(responseJson);
                string fileId = jsonResponse["id"];
                fileImageID = fileId;
                Debug.LogError("File ID: " + fileId);

            }
        }

        IEnumerator CreateRun()
        {
            string runUrl = $"{baseApiUrl}/threads/{threadId}/runs";
            string runJson = "";
            //string runJson = $"{{\"assistant_id\":\"{assistantID.GetString}\", \"temperature\": {temperature}, \"top_p\": {top_p}, \"instructions\": \"{systemPrompt}\"}}";
            if(string.IsNullOrEmpty(systemPrompt)) 
            {
                runJson = $"{{\"assistant_id\":\"{assistantID}\"}}";
            }
            else
            {
                runJson = $"{{\"assistant_id\":\"{assistantID}\", \"instructions\": \"{systemPrompt}\"}}";
            }
            
            UnityWebRequest runRequest = new UnityWebRequest(runUrl, "POST");
            byte[] runBodyRaw = Encoding.UTF8.GetBytes(runJson);
            runRequest.uploadHandler = new UploadHandlerRaw(runBodyRaw);
            runRequest.downloadHandler = new DownloadHandlerBuffer();
            runRequest.SetRequestHeader("Content-Type", "application/json");
            runRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            runRequest.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            yield return runRequest.SendWebRequest();

            if (runRequest.result == UnityWebRequest.Result.Success)
            {
                if(showDebugs)
                {
                    Debug.LogError("Run created successfully: " + runRequest.downloadHandler.text);
                }

                var jsonResponse = JSON.Parse(runRequest.downloadHandler.text);
                string runId = jsonResponse["id"];
                yield return StartCoroutine(CheckRunStatus(runId));
            }
            else
            {
                Debug.LogError("Error creating run: " + runRequest.error + ":" + runRequest.downloadHandler.text);
                AIFailed("Assistant Failed To Create Run");
            }
        }

        IEnumerator CheckRunStatus(string runId)
        {
            string statusUrl = $"{baseApiUrl}/threads/{threadId}/runs/{runId}";

            while (true)
            {
                UnityWebRequest statusRequest = UnityWebRequest.Get(statusUrl);
                statusRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
                statusRequest.SetRequestHeader("OpenAI-Beta", "assistants=v2");

                yield return statusRequest.SendWebRequest();

                if (statusRequest.result == UnityWebRequest.Result.Success)
                {
                    if(showDebugs)
                        {
                            Debug.LogError("Run status: " + statusRequest.downloadHandler.text);
                        }
                    var statusResponse = JSON.Parse(statusRequest.downloadHandler.text);

                    if (statusResponse["status"] == "completed")
                    {
                        if(showDebugs)
                        {
                            Debug.LogError("Run completed successfully.");
                        }
                        yield return StartCoroutine(GetAssistantResponse());
                        yield break;
                    }
                }
                else
                {
                    Debug.LogError("Error checking run status: " + statusRequest.error + ":" + statusRequest.downloadHandler.text);
                    AIFailed("Assistant Failed");
                    yield break;
                }

                yield return new WaitForSeconds(pollSpeed); //Polling Pretty Fast
            }
        }

        IEnumerator GetAssistantResponse()
        {
            string messagesUrl = $"{baseApiUrl}/threads/{threadId}/messages";

            UnityWebRequest messagesRequest = UnityWebRequest.Get(messagesUrl);
            messagesRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            messagesRequest.SetRequestHeader("OpenAI-Beta", "assistants=v1");

            yield return messagesRequest.SendWebRequest();

            if (messagesRequest.result == UnityWebRequest.Result.Success)
            {
                if (showDebugs)
                {
                    Debug.LogError("GetAssistantResponse Messages retrieved successfully: " + messagesRequest.downloadHandler.text);
                }
                var messagesResponse = JSON.Parse(messagesRequest.downloadHandler.text);
                //0 is always the last response
                textOutput = messagesResponse["data"][0]["content"][0]["text"]["value"];
                AISuccess(textOutput);
            }
            else
            {
                Debug.LogError("Error retrieving messages: " + messagesRequest.error + ":" + messagesRequest.downloadHandler.text);
                AIFailed("Assistant To Get Messages");
            }
        }

        public IEnumerator CleanThread()
        {
            // Clear the current thread and create a new one
            threadId = null;
            yield return StartCoroutine(CreateThread());
        }

        //Create Assistant Temp Code
        //IEnumerator CreateAssistant()
        //{
        //    string url = $"{baseApiUrl}/assistants";
        //    string jsonData = "{\"name\":\"UnityAssistant\",\"instructions\":\"You are an assistant created for Unity.\",\"model\":\"gpt-4o\",\"tools\":[{\"type\":\"code_interpreter\"}]}";

        //    UnityWebRequest request = new UnityWebRequest(url, "POST");
        //    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        //    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        //    request.downloadHandler = new DownloadHandlerBuffer();
        //    request.SetRequestHeader("Content-Type", "application/json");
        //    request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        //    request.SetRequestHeader("OpenAI-Beta", "assistants=v1");

        //    yield return request.SendWebRequest();

        //    if (request.result == UnityWebRequest.Result.Success)
        //    {
        //        Debug.Log("Assistant created successfully: " + request.downloadHandler.text);
        //        var jsonResponse = JSON.Parse(request.downloadHandler.text);
        //        assistantId = jsonResponse["id"];

        //    }
        //    else
        //    {
        //        Debug.LogError("Error creating assistant: " + request.error);
        //    }
        //}
    }
}