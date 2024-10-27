using UnityEngine.Events;
using UnityEngine;


namespace AICore3lb
{
    public class LLM_Responder : MonoBehaviour
    {
        //This needs to remove Markup!
        [AICoreRequired]
        public BaseAI_SpeechToText AI_Listener;
        [AICoreRequired]
        public BaseAI_LLM aiAPI;
        [AICoreEmphasize]
        public BaseAI_TextToSpeech AI_Speaker;

        public UnityEvent onStartListening;
        public UnityEvent onStopListening;
        public UnityEvent processingComplete;


        public void Start()
        {
            SetupListeners();
        }

        protected virtual void SetupListeners()
        {
            if (AI_Listener)
            {
                AI_Listener.aiCompleted?.AddListener(ProcessAI);
            }
            aiAPI.aiCompleted?.AddListener(_AIRequestSuccess);
            aiAPI.aiFailed?.AddListener(_AIRequestFail);
        }

        //If your using STT Start Listening here
        [AICoreButton("Start Listening")]
        public virtual void _StartListening()
        {
            AI_Listener._StartListening();
            onStartListening.Invoke();
        }

        public virtual void _Speak(string whatToSay)
        {
            if (AI_Speaker)
            {
                AI_Speaker._Speak(whatToSay);
            }
        }

        public virtual void _StopSpeaking()
        {
            if (AI_Speaker)
            {
                AI_Speaker._StopAudio();
            }
            onStopListening.Invoke();
        }

        public virtual void ProcessAI(string text)
        {
            onStopListening.Invoke();
            aiAPI._TextPrompt(text);
        }

        public virtual void _AIRequestSuccess(string chg)
        {
            chg = chg.Replace("\n", " ");
            _Speak(chg);
            processingComplete.Invoke();
        }

        public virtual void _AIRequestFail(string chg)
        {
            //Do nothing for now with this error
            if(AI_Speaker)
            {
                AI_Speaker._Speak("I encountered an error please try again later");
            }

        }
    }
}
