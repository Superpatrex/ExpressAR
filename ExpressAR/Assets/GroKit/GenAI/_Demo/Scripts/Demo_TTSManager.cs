using AICore3lb;
using TMPro;
using UnityEngine;

namespace AICore3lb.Demo
{
    public class Demo_TTSManager : MonoBehaviour
    {
        public BaseAI_TextToSpeech TTS_Speaker;

        public TMP_InputField inputField;
        public TMP_Text outputField;
        public TTS_SaveToFile fileSaver;

        public void _StopSpeaking()
        {
            TTS_Speaker._StopAudio();
        }

        public void _SpeakBubble()
        {
            _StopSpeaking();
            TTS_Speaker._Speak(outputField.text);
        }

        public void _SaveBubbleToFile()
        {
            _StopSpeaking();
            fileSaver.targetTTS = TTS_Speaker;
            fileSaver.enabled = true;
            fileSaver.whatToSay = outputField.text;
            fileSaver.SpeakAndSaveAudio();
        }

        public void _SpeakFromInputField()
        {
            _StopSpeaking();
            outputField.text = inputField.text;
            TTS_Speaker._Speak(outputField.text);
        }
    }
}