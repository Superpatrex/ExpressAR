using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AICore3lb
{
    public class STT_KeyWordEvent : MonoBehaviour
    {
        [AICoreRequired]
        public BaseAI_SpeechToText voiceSystem;
        public bool containsKeyword;
        [AICoreShowIf("containsKeyword")]
        public string[] extraKeywords;
        public string keyWord;
        [AICoreReadOnly]
        public string extraText;


        public bool usePartialTranscribe;
        public UnityEvent onKeyWordHeard;

        public UnityEvent onFailedKeyword;

        [TextArea]
        public string lastTranscript;

        public string testString;

        [AICoreButton]
        public void TestProcessText()
        {
            ProcessText(testString);
        }

        public void Awake()
        {
            if (voiceSystem)
            {
                if (usePartialTranscribe)
                {
                    voiceSystem.aiCompleted.AddListener(ProcessText);
                }
                else
                {
                    voiceSystem.aiCompleted.AddListener(ProcessText);
                }
            }
        }

        public void ProcessText(string text)
        {
            lastTranscript = text;
            string holder = text;

            if (containsKeyword)
            {
                List<String> testWords = extraKeywords.ToList();
                testWords.Add(keyWord);
                if (ContainsAny(holder, testWords.ToArray()))
                {
                    onKeyWordHeard.Invoke();
                    return;
                }
            }
            else
            {
                if (holder.StartsWith(keyWord, StringComparison.OrdinalIgnoreCase))
                {
                    onKeyWordHeard.Invoke();
                    return;
                }
            }
            onFailedKeyword.Invoke();
        }

        public bool ContainsAny(string input, string[] selectedWords)
        {
            return selectedWords.Any(word => input.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
