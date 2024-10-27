using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace AICore3lb
{
    public class BaseAI_ImageGen : BaseAI
    {
        [SerializeField]
        public int width = 1024;
        public int height = 1024;
        ////public TMP_Text outputText;
        //[SerializeField] private Renderer thisRenderer;
        public const string defaultShaderString = "Universal Render Pipeline/Lit";

        [Space(10)]
        [SerializeField]
        [Tooltip("If you want to override the material or else it will use a standard Material")]
        public Material templateMaterial;
        [AICoreEmphasize(true)]
        public string addToImagePrompt = "in Rembrandt style ";
        public string testPrompt = "Give me a portrait of a cat";
        public UnityEvent<Material> requestMaterial;
        public UnityEvent<Texture2D> textureOutput;
        public virtual void _ImagePrompt(string chg)
        {
            StartCoroutine(DoImagePromptRequest(chg));
        }


        public virtual void _ImagePromptFromInput(TMP_InputField myText)
        {
            _ImagePrompt(myText.text);
        }

        [AICoreButton]
        public virtual void _ImagePromptTest()
        {
            StartCoroutine(DoImagePromptRequest(testPrompt));
        }

        protected virtual IEnumerator DoImagePromptRequest(string promptText = "Make a Cat")
        {
            throw new NotImplementedException();
        }

        protected virtual IEnumerator DownloadAndApplyTexture(string url)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    Material material;
                    if (templateMaterial == null)
                    {
                       material = new Material(Shader.Find(defaultShaderString));
                    }
                    else
                    {
                       material = new Material(templateMaterial);
                    }

                    material.mainTexture = texture;
                    // Apply the new material to the cube
                    AISuccess(url);
                    requestMaterial.Invoke(material);
                    textureOutput.Invoke(texture);
                }
                else
                {
                    Debug.LogError("Failed to download texture. Error: " + uwr.error);
                    AIFailed("Failed to download texture. Error: " + uwr.error);
                }
            }
        }
    }
}
