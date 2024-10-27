using TMPro;
using UnityEngine;

namespace AICore3lb
{
    public class STT_SelectMicDropDown : AICoreBehaviour
    {
        [AICoreRequired]
        public BaseAI_SpeechToText targetAI;
        [AICoreRequired]
        public TMP_Dropdown dropDown;

        public void Start()
        {
            if (dropDown != null)
            {
                // Select the microphone device (by default the first one) but
                // also populate the dropdown with all available devices
                targetAI.forceInputDevice = true;
                targetAI.inputDevice = Microphone.devices[0];
                dropDown.options.Clear();
                foreach (var device in Microphone.devices)
                {
                    dropDown.options.Add(new TMP_Dropdown.OptionData(device));
                }
                dropDown.value = 0;
                dropDown.onValueChanged.AddListener(OnDeviceChanged);
            }
        }

        private void OnDeviceChanged(int index)
        {
            targetAI._ChangeMic(Microphone.devices[index]);
        }
    }
}

