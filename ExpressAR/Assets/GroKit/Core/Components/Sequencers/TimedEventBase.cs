using UnityEngine;
using UnityEngine.Events;

namespace Core3lb
{
    public class TimedEventBase : MonoBehaviour
    {
        //[FancyHeader("Test")]
        public bool isActive;
        public float currentInterval = 1;
        public bool useRandomInterval;
        [CoreShowIf("useRandomInterval")]
        public Vector2 randomIntervals;
        //CSW - random interval shouldn't be in BASE. Also, it wasn't doing anything.
        [CoreReadOnly]
        public float timer;


        public UnityEvent complete;
        public bool runEventOnStart;
        [CoreShowIf("runEventOnStart")]
        public UnityEvent start;
        public bool runEventOnReset;
        [CoreShowIf("runEventOnReset")]
        public UnityEvent reset;

        public virtual void Awake()
        {
            if(useRandomInterval)
            {
                GetNewInterval();
            }
        }

        public virtual void FixedUpdate()
        {
            if (isActive)
            {
                if (EvaluateTime())
                {
                    _RunEvent();
                    TimerReached();
                }
            }
        }

        protected virtual void GetNewInterval()
        {
            if (useRandomInterval)
            {
                currentInterval = randomIntervals.Randomize();
            }
        }

        public virtual void _RunEvent()
        {
            complete.Invoke();
        }

        public virtual void TimerReached()
        {
            GetNewInterval();
            _Stop();
        }

        public virtual bool EvaluateTime()
        {
            timer += Time.deltaTime;
            return timer > currentInterval;
        }

        [CoreButton]
        public virtual void _Start()
        {
            start.Invoke();
            isActive = true;
        }

        [CoreButton]
        public virtual void _Stop()
        {
            isActive = false;
        }

        [CoreButton]
        public virtual void _Reset()
        {
            if(runEventOnReset)
            {
                reset.Invoke();
            }
            GetNewInterval();
            timer = 0;
        }

        public virtual void _RestartTimer()
        {
            _Reset();
            _Start();
        }

        public virtual void _StopAndReset()
        {
            _Stop();
            _Reset();
        }

    }
}
