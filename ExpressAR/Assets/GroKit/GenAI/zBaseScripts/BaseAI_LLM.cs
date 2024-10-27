using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace AICore3lb
{
    public class BaseAI_LLM : BaseAI
    {
        [AICoreHeader("AI Text")]
        public string addToPromptStart;

        [Tooltip("DebugEcho means it will only echo what was said not processing to the AI")]
        public bool debugEcho;

        [AICoreEmphasize]
        [Tooltip("This allows you to store the conversation and update the system prompt")]
        public BaseAI_Conversation AIConversation;

        [TextArea]

        public string systemPrompt = "You are a helpful assistant";

        [AICoreHeader("Debug")]
        [Space(10)]
        [TextArea(2,5)]
        [SerializeField]
        protected string testPrompt = "Tell me a fact about cats";
        [SerializeField]
        [TextArea(2, 5)]
        protected string testImagePrompt = "What is this?";
        public Texture2D testImage;



        public virtual void _TextPrompt(string chg)
        {
            _TextPromptWithImage(chg);
        }

        public virtual void _TextPromptWithImage(string chg,Texture2D image = null)
        {
            chg = addToPromptStart + " " + chg;
            chg = AICoreExtensions.SanitizeForJson(chg);
            if (debugEcho)
            {
                AIStarted($"Echoing Start {chg} Image {image}");
                AISuccess($"Echoing Success {chg} Image {image}");
                return;
            }
            StartCoroutine(DoLLMPrompt(chg,image));
        }

        public virtual Texture2D DuplicateTextureInSupportedFormat(Texture2D source)
        {
            // Create a new Texture2D with a format that supports encoding
            Texture2D result = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);

            // Copy the pixels from the source texture to the new one
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(source, renderTexture);

            RenderTexture.active = renderTexture;
            result.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            result.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);
            return result;
        }

        public string ConvertTextureToBase64(Texture2D source)
        {
            try
            {
                byte[] imageData = source.EncodeToPNG();
                return Convert.ToBase64String(imageData);
            }
            catch
            {
                Texture2D textureInSupportedFormat = DuplicateTextureInSupportedFormat(source);
                byte[] imageData = textureInSupportedFormat.EncodeToPNG();
                return Convert.ToBase64String(imageData);
            }
        }

        public virtual void _TextPromptFromInput(TMP_InputField myText)
        {
            string holder = AICoreExtensions.SanitizeForJson(myText.text);
            _TextPrompt(holder);
        }

        [AICoreButton]
        public virtual void _TextPromptTest()
        {
            _TextPrompt(testPrompt);
        }

        [AICoreButton]
        public virtual void _TextPromptImageTest()
        {
            _TextPromptWithImage(testImagePrompt, testImage);
        }

        protected virtual IEnumerator DoLLMPrompt(string promptText = "Are you Online?",Texture2D image = null)
        {
            throw new NotImplementedException();
        }

        public virtual void _ClearHistory()
        {
            if(AIConversation)
            {
                AIConversation._ClearConversationHistory();
            }
        }

        public virtual void AddConversationHistory(string promptText,string response)
        {
            if(AIConversation)
            {
                AIConversation.AddConversation(promptText, response);
            }
        }

    }
}
