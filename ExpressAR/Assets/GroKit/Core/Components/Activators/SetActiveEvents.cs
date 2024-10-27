using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core3lb
{
    public class ActiveStatesEvents : MonoBehaviour
    {
        [Tooltip("This is for Objects that are already in the scene turned ON start")]
        public bool skipFirst = true;
        bool didFirst = true;
        public UnityEvent onEnableEvent;
        public UnityEvent onDisableEvent;


        public void Awake()
        {
            if (skipFirst)
            {
                didFirst = false;
            }
        }

        public void OnEnable()
        {

            if (didFirst)
            {
                _RunOnEnable();
            }
            didFirst = true;
        }

        public void OnDisable()
        {
            _RunOnDisable();
        }

        public virtual void _RunOnEnable()
        {
            onEnableEvent.Invoke();

        }

        public virtual void _RunOnDisable()
        {
            onDisableEvent.Invoke();
        }
    }
}
