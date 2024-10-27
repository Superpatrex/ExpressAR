using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core3lb
{
    public class PCInputEvent : BaseInputXREvent
    {
        [CoreHeader("PC Settings")]
        [CoreHideIf("useMouseButton")]
        public Key keyboardKey = Key.V;
        public bool useMouseButton;
        [CoreShowIf("useMouseButton")]
        public int mouseButton = 0;

        //Touch events can be added later
        public override bool GetInput()
        {
            if (useMouseButton)
            {
                switch (mouseButton)
                {
                    case 0:
                        return Mouse.current.leftButton.isPressed;   // Left mouse button
                    case 1:
                        return Mouse.current.rightButton.isPressed;  // Right mouse button
                    case 2:
                        return Mouse.current.middleButton.isPressed; // Middle mouse button
                    case 3:
                        return Mouse.current.forwardButton.isPressed; // Mouse button 4 (usually "Forward" on some mice)
                    case 4:
                        return Mouse.current.backButton.isPressed;   // Mouse button 5 (usually "Back" on some mice)
                    default:
                        Debug.LogWarning("Invalid mouse button index");
                        return false;
                }
            }
            else
            {
                Debug.LogError(Keyboard.current[keyboardKey].isPressed);
                return Keyboard.current[keyboardKey].isPressed;
            }
        }
    }
}
