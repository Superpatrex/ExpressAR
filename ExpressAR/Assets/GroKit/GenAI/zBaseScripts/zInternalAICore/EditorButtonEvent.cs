using UnityEngine.Events;

namespace AICore3lb
{
    public class EditorButtonEvent : AICoreBehaviour
    {
        public UnityEvent myEvent;

        [AICoreButton]
        public void RunEvent()
        {
            myEvent.Invoke();
        }
    }

}
