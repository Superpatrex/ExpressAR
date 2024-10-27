using System;
using System.Collections.Generic;
using UnityEngine;

namespace AICore3lb
{
    public class BaseAI_Conversation : AICoreBehaviour
    {

        public bool overrideSystemPrompt;
        [TextArea]
        [AICoreShowIf("overrideSystemPrompt")]
        public string systemPromptOverride = "You are a helpful Assistant";
        public int maxHistory = 15;
        [Tooltip("These will not be cleared on history")]
        public List<conversationData> systemConversationList;
        public List<conversationData> conversationList;

        [Serializable]
        public struct conversationData
        {
            public string user;
            public string assistant;

            // Constructor to easily create conversationData instances
            public conversationData(string userMessage, string assistantMessage)
            {
                user = userMessage;
                assistant = assistantMessage;
            }
        }

        // Method to add a new conversation piece to the list
        public void AddConversation(string userMessage, string assistantMessage)
        {
            userMessage = AICoreExtensions.SanitizeForJson(userMessage);
            assistantMessage = AICoreExtensions.SanitizeForJson(assistantMessage);
            conversationData newConversation = new conversationData(userMessage, assistantMessage);
            conversationList.Add(newConversation);
            if (conversationList.Count >= maxHistory)
            {
                conversationList.RemoveAt(0); // Remove the oldest conversation at the top of the list
            }
        }

        [AICoreButton]
        public void _ClearConversationHistory()
        {
            conversationList.Clear();
        }
    }
}
