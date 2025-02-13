using UnityEngine;

namespace Core3lb
{
    public class ActivatorSequencer : MonoBehaviour
    {
        public BaseActivator[] Activators;
        public bool runTheirOffEvents;
        public bool loops = true;
        [Tooltip("Set Only if you need to")]
        public int currentIndex;

        public void _JumpToStep(int chg)
        {
            currentIndex = chg;
            RunActivator();
        }

        protected void RunActivator()
        {
            if (runTheirOffEvents)
            {
                foreach (var item in Activators)
                {
                    item._OffEvent();
                }
            }
            Activators[currentIndex]._OnEvent();
        }

        [CoreButton]
        public void _StepForward()
        {
            currentIndex++;
            if (currentIndex >= Activators.Length)
            {
                if (!loops)
                {
                    return;
                }
                currentIndex = 0;
            }
            RunActivator();
        }

        [CoreButton]
        public void _StepBack()
        {
            currentIndex--;
            if (currentIndex == -1)
            {
                if (!loops)
                {
                    return;
                }
                currentIndex = Activators.Length - 1;
            }
            RunActivator();
        }
    }
}
