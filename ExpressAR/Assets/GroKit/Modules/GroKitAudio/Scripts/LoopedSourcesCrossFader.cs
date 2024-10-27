using UnityEngine;

namespace Core3lb
{
    public class LoopedSourcesCrossFader : MonoBehaviour
    {
        public float firstFadeTime = 1.0f;
        public float crossFadeTime = 1.0f;
        public LoopedSource[] myLoops;

        [SerializeField]
        [CoreReadOnly]
        private LoopedSource currentPlayingSource;
        [CoreReadOnly]
        public int currentIndex;

        public virtual void _PlayTrack(int index)
        {
            if(currentPlayingSource == null)
            {
                myLoops[index].fadeTime = firstFadeTime;
                myLoops[index]._PlayFadeIn();
            }
            else
            {
                if(currentPlayingSource == myLoops[index])
                {
                    return;
                }
                currentPlayingSource.fadeTime = crossFadeTime;
                currentPlayingSource._StopFadeOut();
                myLoops[index].fadeTime = crossFadeTime;
                myLoops[index]._PlayFadeIn();
                currentPlayingSource = myLoops[index];

            }
            currentIndex = index;
        }

        public virtual void Stop()
        {
            if(currentPlayingSource != null) 
            {
                currentPlayingSource._StopFadeOut();
            }
        }

        public virtual void StopNow()
        {
            if (currentPlayingSource != null)
            {
                currentPlayingSource.fadeTime = .06f;
                currentPlayingSource._StopFadeOut();
            }
        }
    }
}
