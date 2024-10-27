using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core3lb
{
    public class DEBUG_BaseInputOverride : MonoBehaviour
    {
        public Key key = Key.V;
        public bool controlON;
        public bool getFromChildrenOnly;
        public bool autoGet;

        // Update is called once per frame
        public BaseInputXREvent toControl;
        public List<BaseInputXREvent> events;

        public void Awake()
        {
            if (controlON)
            {
                Debug.LogError("DEBUG_BaseInputOverride is on and ACTIVE", gameObject);
            }
        }
        void Update()
        {
            if (!controlON)
            {
                return;
            }
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                Debug.Log($"Key {key} was pressed.");
                ControlInputs();
            }
        }

        public void ControlInputs()
        {
            if (autoGet)
            {
                if (getFromChildrenOnly)
                {
                    events = GetComponentsInChildren<BaseInputXREvent>().ToList();
                }
                else
                {
                    events = FindObjectsOfType<BaseInputXREvent>().ToList();
                }
            }
            if (toControl)
            {
                toControl.inputProcessor.DEBUG_OverrideInputs();
            }
            foreach (BaseInputXREvent e in events)
            {
                e.inputProcessor.DEBUG_OverrideInputs();
            }
        }

    }
}
