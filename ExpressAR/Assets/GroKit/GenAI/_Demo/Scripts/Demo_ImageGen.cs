using AICore3lb;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AICore3lb.Demo
{
    public class Demo_ImageGen : AICoreBehaviour
    {
        public BaseAI_ImageGen imageGenerator;
        public Image spriteEffector;
        public TMP_InputField inputField;
        public void Start()
        {
            imageGenerator.textureOutput.AddListener(OnTextureStart);
        }

        public void OnTextureStart(Texture2D texture)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            spriteEffector.sprite = sprite;
        }

        public void _GenerateImage()
        {
            imageGenerator._ImagePrompt(inputField.text);
        }
    }

}