using AICore3lb;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AICore3lb.Demo
{
    public class Demo_Vision : AICoreBehaviour
    {
        public BaseAI_LLM visionLLM;
        public TMP_InputField inputField;
        public string defaultPrompt = "What do you see?";
        public bool useCamera;

        public RawImage myRender;
        public Texture2D defaultTexture;
        private WebCamTexture webCamTexture;
        private Texture2D capturedTexture;


        //This is a simple application for using a webcam to work on mobile devices will require permission rewriting
        public void _StartWebCam()
        {
            if (webCamTexture == null)
            {
                myRender.texture = null;
                webCamTexture = new WebCamTexture();
                ////forcing everything to 640/480 to fit into the square in the UI
                //int width = 640; // Example width for 4:3 aspect ratio
                //int height = 480; // Example height for 4:3 aspect ratio
                //webCamTexture.requestedWidth = width;
                //webCamTexture.requestedHeight = height;
                webCamTexture.Play();

                // Clone the material and assign it to the renderer
                Material clonedMaterial = new Material(myRender.material);
                myRender.material = clonedMaterial;

                // Assign the webcam texture to the cloned material
                myRender.material.mainTexture = webCamTexture;
            }
            else
            {
                webCamTexture.Stop();
                Destroy(webCamTexture);
                webCamTexture = null;
                _StartWebCam();
            }
        }

        public void StopCamera()
        {
            webCamTexture.Stop();
            myRender.texture = defaultTexture;
        }

        public void ToggleCameraMode()
        {
            useCamera = !useCamera;
            if (useCamera)
            {
                _StartWebCam();
            }
            else
            {
                StopCamera();
            }
        }

        public void _DoVisionPrompt()
        {
            if (useCamera)
            {
                StartCoroutine(CaptureWebcamTexture(inputField.text));
            }
            else
            {
                visionLLM._TextPromptWithImage(inputField.text, defaultTexture);
            }
        }

        IEnumerator CaptureWebcamTexture(string currentText = "")
        {

            if (webCamTexture.isPlaying)
            {
                // Create a new Texture2D with the same dimensions as the webcam texture
                capturedTexture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
                // Wait until the end of the frame before capturing to ensure all webcam updates are processed
                yield return new WaitForEndOfFrame();
                // Apply the pixels from the webcam texture to the new Texture2D
                capturedTexture.SetPixels(webCamTexture.GetPixels());
                capturedTexture.Apply();
                if (string.IsNullOrEmpty(currentText))
                {
                    if (inputField)
                    {
                        if (string.IsNullOrEmpty(inputField.text))
                        {
                            visionLLM._TextPromptWithImage(inputField.text, capturedTexture);
                        }
                    }
                    else
                    {
                        visionLLM._TextPromptWithImage(defaultPrompt, capturedTexture);
                    }
                }
                else
                {
                    visionLLM._TextPromptWithImage(currentText, capturedTexture);
                }
            }
        }
    }
}