using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AICore3lb.Demo
{
    public class Demo_LLMChatManager : AICoreBehaviour
    {
        [SerializeField]
        private ScrollRect _scrollRectComponent; //your scroll rect component
        [SerializeField]
        RectTransform _container; //content transform of the scrollrect

        public GameObject userBubble;
        public GameObject assistantBubble;

        public UnityEvent userBubbleCreated;
        public UnityEvent assistantBubbleCreated;


        public List<GameObject> bubbles;

        public void Awake()
        {
            bubbles = new List<GameObject>();   
        }


        //Start Chat
        //EndChat


        public void AddNewBubble(GameObject chatBubblePrefab)
        {
            GameObject g = Instantiate(chatBubblePrefab, _container, false);
            RectTransform r = g.GetComponent<RectTransform>();
            StopAllCoroutines();
            StartCoroutine(LerpToChild(r));
            bubbles.Add(g);
        }

        public void ClearBubbles()
        {
            foreach (var item in bubbles)
            {
                Destroy(item);
            }
            bubbles.Clear();
            Canvas.ForceUpdateCanvases();
        }

        public void _AddAssistantBubble(string text)
        {
            GameObject g = Instantiate(assistantBubble, _container, false);
            RectTransform r = g.GetComponent<RectTransform>();
            TextMeshProUGUI chatText = g.GetComponentInChildren<TextMeshProUGUI>();
            chatText.text = text;
            StopAllCoroutines();
            StartCoroutine(LerpToChild(r));
            assistantBubbleCreated.Invoke();
            bubbles.Add(g);
        }

        public void _AddUserBubble(string text)
        {
            GameObject g = Instantiate(userBubble, _container, false);
            RectTransform r = g.GetComponent<RectTransform>();
            TextMeshProUGUI chatText = g.GetComponentInChildren<TextMeshProUGUI>();
            chatText.text = text;
            StopAllCoroutines();
            userBubbleCreated.Invoke();
            StartCoroutine(LerpToChild(r));
            bubbles.Add(g);
        }

        private IEnumerator LerpToChild(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();
            yield return new WaitForEndOfFrame();
            float elapsedTime = 0;
            float waitTime = 1f;
            float d = _scrollRectComponent.verticalNormalizedPosition;
            while (elapsedTime < waitTime)
            {
                _scrollRectComponent.verticalNormalizedPosition = Mathf.Lerp(d, 0, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;

                yield return null;
            }
            Canvas.ForceUpdateCanvases();
            yield return null;
        }
    }
}
