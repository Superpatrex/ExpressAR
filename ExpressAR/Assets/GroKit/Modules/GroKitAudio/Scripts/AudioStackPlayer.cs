using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core3lb
{
    public class AudioStackPlayer : SFXBase
    {
        [CoreRequired]
        public SOGroKitAudioStack audioClipData;

        [CoreHeader("Randomness")]
        public bool pickOneRandomly = false; //Pick one sound from the List And use it's delay
        public bool ignoreDelays = false;
        public bool useWeights = false;
        [HideInInspector] //Hides additional Settings
        public bool useOtherClips = true;

        public override void _PlayClip(AudioClip myClip)
        {
            Debug.LogError("Audio Stacks Cannot Play Clips");
            return;
        }

        public override void  _Play()
        {
            List<AudioClipData> clipStack = audioClipData.clipStack.ToList();
            if (pickOneRandomly)
            {
                if(useWeights)
                {
                    clipStack = audioClipData.PickOne().QuickToList();
                }
                else
                {
                    clipStack = audioClipData.clipStack.RandomItem().QuickToList();
                }
            }
            if (useWeights)
            {
                clipStack = audioClipData.GetWeightedList();
            }

            foreach (AudioClipData clipData in clipStack)
            {
                PlayAudioClips(clipData);
            }
        }

        public void _StopAllNow()
        {
            StopAllCoroutines();
        }


        private void PlayAudioClips(AudioClipData clipData)
        {
            if (clipData.myClips.Length == 0)
                return;

            AudioClip clipToPlay;
            clipToPlay = clipData.myClips[Random.Range(0, clipData.myClips.Length)];
            StartCoroutine(DelayedPlayOnShot(clipData, clipToPlay));
        }

        private IEnumerator DelayedPlayOnShot(AudioClipData clipData,AudioClip clipToPlay)
        {
            var delay = clipData.delay;
            if(ignoreDelays)
            {
                delay = 0;
            }
            var pitch = clipData.template.pitch;
            var volume = clipData.template.volume;
            if (clipData.alterations.alterPitch)
            {
                pitch = Random.Range((int)clipData.alterations.randomPitch.x, (int)clipData.alterations.randomPitch.y);
            }
            if (clipData.alterations.alterVolume)
            {
                volume = Random.Range((int)clipData.alterations.randomVolume.x, (int)clipData.alterations.randomVolume.y);
            }
            yield return new WaitForSeconds(delay);
            GroKitAudioManager.instance.PlayAudio3DTemplate(gameObject, clipToPlay, audioPosition, clipData.shouldFollow, clipData.template, pitch, volume);
        }
    }
}
