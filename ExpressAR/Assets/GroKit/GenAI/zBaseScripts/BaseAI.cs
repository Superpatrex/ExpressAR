using UnityEngine;
using UnityEngine.Events;
using CoreSecurity;

namespace AICore3lb
{
    public abstract class BaseAI : AICoreBehaviour
    {
        public bool overrideAPIKey;
        [AICoreHideIf("overrideAPIKey")]
        [SerializeField]
        [AICoreEmphasize(true)]
        protected SO_StringEncrypted apiKeySO;

        [AICoreShowIf("overrideAPIKey")]
        [AICoreEmphasize(true)]
        protected string apiKeyString;
        [Space(10)]
        public UnityEvent<string> aiStarted;
        //Request is Complete
        public UnityEvent<string> aiCompleted;
        //Request Failed
        public UnityEvent<string> aiFailed;

        [SerializeField]
        protected bool showDebugs;

        [TextArea(3, 6)]
        public string textOutput;

        protected virtual void AIStarted(string text)
        {
            aiStarted.Invoke(text);
        }

        protected virtual void AISuccess(string text)
        {
            textOutput = text;
            aiCompleted.Invoke(text);
        }

        protected virtual void AIFailed(string whyFail)
        {
            Debug.LogError("AI Failure: " + whyFail,gameObject);
            aiFailed.Invoke(whyFail);
        }

        public void _SetAPIKey(string key)
        {
            overrideAPIKey = true;
            apiKeyString = key;
        }

        public string apiKey
        {
            get
            {
                if(overrideAPIKey)
                {
                    return apiKeyString;
                }
                else
                {
                    return apiKeySO.GetString;
                }
            }
        }

        public bool CheckForAPIKey
        {
            get
            {
                return apiKey != null;
            }
        }

    }
}
