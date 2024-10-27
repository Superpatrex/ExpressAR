using UnityEngine;
using UnityEngine.Events;

namespace Core3lb
{
    public class FAST_EventTest : MonoBehaviour
    {
        public UnityEvent FastTestEvent;
        public BaseActivator targetActivator;
        [CoreButton("Event ", true)]
        public void Event()
        {
            FastTestEvent.Invoke();
        }

        [CoreButton]
        public void OnActivator()
        {
            targetActivator._OnEvent();
        }

        [CoreButton]
        public void OffActivator()
        {
            targetActivator._OffEvent();
        }
    }
}
