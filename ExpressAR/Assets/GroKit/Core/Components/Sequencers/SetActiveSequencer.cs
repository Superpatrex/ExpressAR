using UnityEngine;
using UnityEngine.Events;

namespace Core3lb
{
    public class SetActiveSequencer : MonoBehaviour
    {
        public GameObject[] gameObjects;
        [Tooltip("Forces all objects off in the array")]
        public bool turnAllObjectsOffFirst;
        public bool loops = true;
        [Tooltip("Also Start Index")]
        public int currentIndex;

        public UnityEvent SequenceRun;

        public void _JumpToStep(int chg)
        {
            currentIndex = chg;
            RunSequence();
        }

        protected virtual void RunSequence()
        {
            SequenceRun.Invoke();
            if (turnAllObjectsOffFirst)
            {
                gameObjects.SetArrayGO(false);
            }
            gameObjects[currentIndex].SetActive(true);
        }

        [CoreButton]
        public void _StepForward()
        {
            currentIndex++;
            if (currentIndex >= gameObjects.Length)
            {
                if (!loops)
                {
                    return;
                }
                currentIndex = 0;
            }
            RunSequence();
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
                currentIndex = gameObjects.Length - 1;
            }
            RunSequence();
        }
    }
}
