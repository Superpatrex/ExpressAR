using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Core3lb
{
    public class FAST_KeyBoardEvent : MonoBehaviour
    {
        public Key eventKey = Key.F;
        public UnityEvent FastTestEvent;
        public BaseActivator targetActivator;
        public Key onKey = Key.Z;
        public Key offKey = Key.C;
        [CoreButton("Event ", true)]
        public void Event()
        {
            FastTestEvent.Invoke();
        }
        public void Update()
        {
            if(Keyboard.current[eventKey].isPressed)
            {
                Event();
            }

            if(Keyboard.current[onKey].isPressed)
            {
                OnActivator();
            }
            if (Keyboard.current[offKey].isPressed)
            {
                OffActivator();
            }
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
