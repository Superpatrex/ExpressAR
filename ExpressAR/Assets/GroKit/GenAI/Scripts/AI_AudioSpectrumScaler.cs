using AICore3lb;
using UnityEngine;

namespace AICore3lb
{
    public class AI_AudioSpectrumScaler : AICoreBehaviour
    {
        public GameObject[] objectsToScale;
        [AICoreRequired]
        public AudioSource audioSource;
        public float scaleSpeed = 10.0f;
        public float maxScale = 2.0f;
        public float scaleDownSpeed = 1.0f; // Time in seconds to scale back to original size
        private Vector3[] originalScales;
        private float[] scaleDownTimers;
        private int spectrumSize;
        public FFTWindow whatFFT;

        public ParticleSystem[] particleEffects;
        public int maxBurst = 10;

        void Start()
        {
            originalScales = new Vector3[objectsToScale.Length];
            scaleDownTimers = new float[objectsToScale.Length];

            for (int i = 0; i < objectsToScale.Length; i++)
            {
                originalScales[i] = objectsToScale[i].transform.localScale;
            }

            // Calculate the nearest power of two for the spectrum size
            spectrumSize = Mathf.ClosestPowerOfTwo(64);

            // Initialize the particle systems 
            for (int i = 0; i < particleEffects.Length; i++)
            {
                ParticleSystem.EmissionModule emission = particleEffects[i].emission;
                emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 0) });
            }
        }


        void Update()
        {
            float[] spectrum = new float[spectrumSize];
            audioSource.GetSpectrumData(spectrum, 0, whatFFT);

            for (int i = 0; i < objectsToScale.Length; i++)
            {
                if (i < spectrum.Length)
                {
                    float scaleMultiplier = Mathf.Lerp(1.0f, maxScale, spectrum[i] * scaleSpeed);
                    Vector3 targetScale = originalScales[i] * scaleMultiplier;
                    objectsToScale[i].transform.localScale = Vector3.Lerp(objectsToScale[i].transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

                    if (scaleDownTimers[i] <= 0)
                    {
                        objectsToScale[i].transform.localScale = Vector3.Lerp(objectsToScale[i].transform.localScale, originalScales[i], Time.deltaTime * scaleDownSpeed);
                    }

                    // Trigger particle bursts
                    if (particleEffects != null && i < particleEffects.Length)
                    {
                        ParticleSystem.EmissionModule emission = particleEffects[i].emission;
                        int burstCount = Mathf.Min(maxBurst, Mathf.FloorToInt(spectrum[i] * 100));
                        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                        particleEffects[i].Emit(emitParams, burstCount);
                    }
                }

                // Reset or decrement the scale down timer
                if (objectsToScale[i].transform.localScale != originalScales[i])
                {
                    scaleDownTimers[i] = scaleDownSpeed; // Reset timer
                }
                else if (scaleDownTimers[i] > 0)
                {
                    scaleDownTimers[i] -= Time.deltaTime;
                }
            }
        }
    }
}