using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Core3lb
{
    public class EventOnStart : MonoBehaviour
    {

        [SerializeField]
        private UnityEvent startEvent;
        [SerializeField]

        [CoreToggleHeader("Show More Events")]
        public bool showMoreEvents;
        [CoreShowIf("showMoreEvents")]
        [SerializeField]
        private UnityEvent awakeEvent;
        [CoreShowIf("showMoreEvents")]
        [SerializeField]
        private UnityEvent delayStartEvent;
        [CoreShowIf("showMoreEvents")]
        public float delay = 0.2f;

        private void Awake()
        {
            awakeEvent.Invoke();
        }

        IEnumerator Start()
        {
            startEvent.Invoke();
            yield return new WaitForSeconds(delay);
            delayStartEvent.Invoke();
        }
    }
}
