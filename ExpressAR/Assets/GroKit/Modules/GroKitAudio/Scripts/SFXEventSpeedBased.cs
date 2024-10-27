
using UnityEngine;

namespace Core3lb
{
    public class SFXEventSpeedBased : SFXEvent
    {
        [CoreHeader("Speed Audio Settings")]
        public AnimationCurve volumeCurve;
        public AnimationCurve pitchCurve;

        [Tooltip("At this speed pitch and volume will be max")]
        public float maxVelocity = 200f;
        public float maxVolume = 1f; // Set your desired maximum volume here
        public float maxPitch = 1f; // Set your desired maximum pitch here

        private Vector3 lastPosition;
        private Vector3 currentVelocity;

        [Header("-Debugs-")]
        [CoreReadOnly]
        public float lastMag;
        [CoreReadOnly]
        public float lastVolume;
        [CoreReadOnly]
        public float lastPitch;

        public override void Awake()
        {
            base.Awake();
            lastPosition = transform.position;
        }


        public void FixedUpdate()
        {
            VelocityTracking();
        }

        public void VelocityTracking()
        {
            currentVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
            lastPosition = transform.position;
        }

        protected virtual void PlayerInternalComplex(bool is2D = false, AudioClip clip = null)
        {
            PlayImpactAudio(currentVelocity.magnitude);
            if (audioPosition == null)
            {
                audioPosition = transform;
            }
            if (clip == null)
            {
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
                    clip = clipList.RandomItem();
                }
                else
                {
                    if (overrideClip)
                    {
                        clip = overrideClip;
                    }
                    else
                    {
                        if (sourceTemplate == null)
                        {
                            Debug.LogError("Failed to Play no Clip set to play", gameObject);
                            return;
                        }
                        else
                        {
                            clip = sourceTemplate.clip;
                            if (clip == null)
                            {
                                Debug.LogError("Failed to Play no Clip set to play", gameObject);
                                return;
                            }
                        }

                    }

                }

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
                GroKitAudioManager.instance.PlayAudio3DTemplate(gameObject, clip, audioPosition, followPosition, sourceTemplate, pitch, volume, overrideMixer);
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

                GroKitAudioManager.instance.PlayAudio3D(gameObject, clip, audioPosition, followPosition, pitch, volume, overrideMixer, minDistance, maxDistance);
            }

        }

        private void PlayImpactAudio(float velocity)
        {
            float clampedVelocity = Mathf.Clamp(velocity, 0, maxVelocity);
            float normalizedVelocity = clampedVelocity / maxVelocity;
            lastVolume = Mathf.Clamp(volumeCurve.Evaluate(normalizedVelocity) * maxVolume, 0, maxVolume);
            lastPitch = Mathf.Clamp(pitchCurve.Evaluate(normalizedVelocity) * maxPitch, 0, maxPitch);
            lastMag = velocity;
        }
    }
}