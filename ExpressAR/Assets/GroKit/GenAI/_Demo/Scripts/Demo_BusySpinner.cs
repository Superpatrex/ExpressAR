using UnityEngine;

namespace AICore3lb.Demo
{
    public class Demo_BusySpinner : AICoreBehaviour
    {
        [AICoreRequired]
        public BaseAI targetAI;
        public GameObject busyToggler;

        public void Start()
        {
            targetAI.aiStarted.AddListener((string _) =>
            {
                busyToggler.SetActive(true);
            });
            targetAI.aiCompleted.AddListener((string _) =>
            {
                busyToggler.SetActive(false);
            });
            busyToggler.SetActive(false);
        }
    }
}
