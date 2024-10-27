using UnityEngine;

namespace Core3lb
{
    public class SFXEvent : SFXBase
    {
        //[CoreHelpBox("Use for One Shot SFXes with Unity Events")]
        public bool followPosition;
        public bool alterPitch = false;
        [CoreShowIf("alterPitch")]
        [CoreMinMax(-3, 3)]
        public Vector2 randomPitch = Vector2.one;
        public bool alterVolume;
        [CoreShowIf("alterVolume")]
        [CoreMinMax(0, 1)]
        public Vector2 randomVolume = Vector2.one;

        [CoreHeader("Simple Settings")]
        //If you use a Source template you are not going to use these settings
        public bool isSpatialized;
        public float minDistance = 1;
        public float maxDistance = 500;
        [CoreHeader("Complex Settings")]

        [CoreEmphasize]
        public AudioSource sourceTemplate;

        [CoreToggleHeader("MultiClip")]
        [Space(10)]
        public bool useOtherClips;
        [CoreShowIf("useOtherClips")]
        public bool inSequence;
        [CoreShowIf("inSequence")]
        [CoreReadOnly]
        public int clipIndex = 0;
        [CoreShowIf("useOtherClips")]
        public AudioClip[] clipList;

        public virtual void Awake()
        {
            if (sourceTemplate == null && TryGetComponent(out AudioSource source))
            {
                sourceTemplate = source;
            }
            if (sourceTemplate != null)
            {
                sourceTemplate.enabled = false;
            }
        }

        public override void _Play()
        {
            AudioClip clip = null;
            if (useOtherClips)
            {
                if (inSequence)
                {
                    clip = clipList[clipIndex];
                    clipIndex++;
                    if (clipIndex >= clipList.Length)
                    {
                        clipIndex = 0;
                    }
                }
                else
                {
                    clip = clipList.RandomItem();
                }
            }
            else
            {
                if (overrideClip)
                {
                    clip = overrideClip;
                }
                else
                {
                    if (sourceTemplate != null)
                    {
                        clip = sourceTemplate.clip;
                    }
                }
            }
            PlayerInternalComplex(clip);
        }

        protected override void PlayerInternalComplex(AudioClip clip = null)
        {
            if(clip == null)
            {
                if(GroKitAudioManager.showDebugs)
                {
                    Debug.Log("SFXEVENT - No Clip Detected not Playing".ToColor(StringExtensions.ColorType.Orange), gameObject);
                }
                return;
            }
            if (onlyPlayIfActive)
            {
                if (!gameObject.activeInHierarchy)
                {
                    if (GroKitAudioManager.showDebugs)
                    {
                        Debug.Log("SFXEVENT - Not Playing Because Off and onlyPlayIfActive is true".ToColor(StringExtensions.ColorType.Orange), gameObject);
                    }
                    return;
                }
            }
            if (audioPosition == null)
            {
                audioPosition = transform;
            }

            float volume = 1;
            float pitch = 1;
            if (is2D)
            {
                if (alterVolume)
                {
                    volume = randomVolume.Randomize();
                }
                if (alterPitch)
                {
                    pitch = randomPitch.Randomize();

                }
                GroKitAudioManager.instance.PlaySFX2D(gameObject, clip, pitch, volume, overrideMixer);
                return;
            }

            if (sourceTemplate)
            {
                volume = sourceTemplate.volume;
                pitch = sourceTemplate.pitch;
                if (alterVolume)
                {
                    volume = randomVolume.Randomize();
                }
                if (alterPitch)
                {
                    pitch = randomPitch.Randomize();

                }
                GroKitAudioManager.instance.PlayAudio3DTemplate(gameObject, clip, audioPosition,followPosition, sourceTemplate, pitch, volume, overrideMixer);
            }
            else
            {
                if (alterVolume)
                {
                    volume = randomVolume.Randomize();
                }
                if (alterPitch)
                {
                    pitch = randomPitch.Randomize();
                }

                GroKitAudioManager.instance.PlayAudio3D(gameObject, clip, audioPosition, followPosition, pitch, volume, overrideMixer, minDistance, maxDistance, isSpatialized);
            }

        }
    }
}