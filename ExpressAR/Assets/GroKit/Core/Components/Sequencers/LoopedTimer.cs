
using System;
using UnityEngine;

namespace Core3lb
{
    public class LoopedTimer : TimedEventBase
    {
        [CoreHeader("Looped Timer Settings")]
        public bool infiniteLoops;
        [CoreHideIf("infiniteLoops")]
        public int maxLoops = 2;
        [SerializeField]
        [CoreReadOnly]
        int currentLoop;


        public override void _Start()
        {
            base._Start();
        }
        public override void TimerReached()
        {
            GetNewInterval();
            if (infiniteLoops)
            {
                timer = 0;
            }
            else
            {
                currentLoop++;
                if (maxLoops <= currentLoop)
                {
                    _Stop();
                }
                timer = 0;
            }
        }

        public override void _Reset()
        {
            currentLoop = 0;
            base._Reset();
        }
    }
}
