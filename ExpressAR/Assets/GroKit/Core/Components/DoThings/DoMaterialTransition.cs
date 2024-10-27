using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace Core3lb
{
    public class DoMaterialTransition : MonoBehaviour
    {
        public Renderer[] renderers;
        public float transitionTime;
        public bool isMultiMat;
        [CoreShowIf("isMultiMat")]
        public int multiMatIndex;


        [CoreToggleHeader("Float Changes")]
        public bool useFloatChanges = true;
        [CoreShowIf("useFloatChanges")]
        public string materialProperty;
        [CoreShowIf("useFloatChanges")]
        public float setTo;
        [CoreShowIf("useFloatChanges")]
        public float transitionTo;
        [CoreShowIf("useFloatChanges")]
        [CoreReadOnly]
        public float matOriginalValue;

        [CoreToggleHeader("Color Changes")]
        public bool useColorChanges;
        [CoreShowIf("useColorChanges")]
        [Tooltip("If you don't want to use the Material.Color property")]
        public bool overrideMainColor;
        [CoreShowIf("useColorChanges")]
        public string overColorProperty = "_BaseColor";
        [CoreShowIf("useColorChanges")]
        public Color[] colors;
        [CoreShowIf("useColorChanges")]
        public int debugTransitionColorSet = 0;
        [HideInInspector]
        public Color originalColor;
        [Tooltip("For Alpha Change")]
        public AnimationCurve alphaFadeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        private void Awake()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = new Renderer[1];
                renderers[0] = gameObject.GetComponent<MeshRenderer>();
                if (renderers[0] == null)
                {
                    Debug.LogError("You must assign an array to " + gameObject);
                    return;
                }
            }
        }

        public string GetColorProperty
        {
            get
            {
                if (overrideMainColor)
                {
                    if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)
                    {
                        return "_BaseColor";
                    }
                    else
                    {
                        return "_Color";
                    }
                }
                return overColorProperty;
            }
           
        }

        private void Start()
        {
            if (materialProperty != null && materialProperty != "")
            {
                if (useFloatChanges)
                {
                    if (isMultiMat)
                    {
                        matOriginalValue = renderers[0].materials[multiMatIndex].GetFloat(materialProperty);
                    }
                    else
                    {
                        matOriginalValue = renderers[0].material.GetFloat(materialProperty);
                    }
                }
                if (useColorChanges)
                {
                    if (isMultiMat)
                    {
                        originalColor = renderers[0].materials[multiMatIndex].GetColor(overColorProperty);
                    }
                    else
                    {
                        originalColor = renderers[0].material.GetColor(overColorProperty);
                    }
                }
            }
        }

        [CoreButton]
        public virtual void _TransitionColor()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                StartCoroutine(TransitionColorCoroutine(renderers[i], colors[debugTransitionColorSet], transitionTime));
            }
        }

        public virtual void _TransitionColorTo(int index)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                StartCoroutine(TransitionColorCoroutine(renderers[i], colors[index], transitionTime));
            }
        }
        public virtual void _SetColorTo(int index)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (isMultiMat)
                {
                    renderers[i].materials[multiMatIndex].SetColor(GetColorProperty, colors[index]);
                }
                else
                {
                    renderers[i].material.SetColor(GetColorProperty, colors[index]);
                }
            }
        }

        [CoreButton]
        public virtual void _ResetColor()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (isMultiMat)
                {
                    renderers[i].materials[multiMatIndex].SetColor(GetColorProperty, originalColor);
                }
                else
                {
                    renderers[i].material.SetColor(GetColorProperty, originalColor);
                }
            }
        }


        public virtual void _TransitionTo(float value)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                StartCoroutine(TransitionFloatCoroutine(renderers[i], value, transitionTime));
            }
        }

        public virtual void _TransitionTime(float value)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                StartCoroutine(TransitionFloatCoroutine(renderers[i], transitionTo, value));
            }
        }

        public virtual void _SetAlphaTo(float alpha)
        {
            StartCoroutine(TransitionAlphaCoroutine(renderers, alpha, 0,alphaFadeCurve));
        }

        public virtual void _TransitionAlphaTo(float alpha)
        {
            StartCoroutine(TransitionAlphaCoroutine(renderers, alpha, transitionTime, alphaFadeCurve));
        }

        protected virtual IEnumerator TransitionColorCoroutine(Renderer renderer, Color targetColor, float duration)
        {
            Material mat = isMultiMat ? renderer.materials[multiMatIndex] : renderer.material;
            Color startColor = mat.GetColor(GetColorProperty);
            float elapsed = 0;

            while (elapsed < duration)
            {
                mat.SetColor(GetColorProperty, Color.Lerp(startColor, targetColor, elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }

            mat.SetColor(GetColorProperty, targetColor);
        }

        protected virtual IEnumerator TransitionFloatCoroutine(Renderer renderer, float targetValue, float duration)
        {
            Material mat = isMultiMat ? renderer.materials[multiMatIndex] : renderer.material;
            float startValue = mat.GetFloat(materialProperty);
            float elapsed = 0;

            while (elapsed < duration)
            {
                mat.SetFloat(materialProperty, Mathf.Lerp(startValue, targetValue, elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }

            mat.SetFloat(materialProperty, targetValue);
        }



        private IEnumerator TransitionAlphaCoroutine(Renderer[] renderers, float targetAlpha, float duration, AnimationCurve curve = null)
        {
            float elapsed = 0;
            Color[] startColors = new Color[renderers.Length];
            float[] startAlphas = new float[renderers.Length];

            // Store the initial colors and alphas of the renderers
            for (int i = 0; i < renderers.Length; i++)
            {
                startColors[i] = renderers[i].material.color;
                startAlphas[i] = startColors[i].a;
            }

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float evaluatedT = curve?.Evaluate(t) ?? t;

                for (int i = 0; i < renderers.Length; i++)
                {
                    float newAlpha = Mathf.Lerp(startAlphas[i], targetAlpha, evaluatedT);
                    Color newColor = new Color(startColors[i].r, startColors[i].g, startColors[i].b, newAlpha);
                    renderers[i].material.color = newColor;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Set final color
            for (int i = 0; i < renderers.Length; i++)
            {
                Color finalColor = new Color(startColors[i].r, startColors[i].g, startColors[i].b, targetAlpha);
                renderers[i].material.color = finalColor;
            }
        }

        public virtual void SetTo(float value)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (isMultiMat)
                {
                    renderers[i].materials[multiMatIndex].SetFloat(materialProperty, value);
                }
                else
                {
                    renderers[i].material.SetFloat(materialProperty, value);
                }
            }
        }

        public virtual void _SwitchTo(Material mat)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (isMultiMat)
                {
                    //renderers[i].materials[multiMatIndex] = mat;
                    var rendMats = renderers[i].materials;
                    rendMats[multiMatIndex] = mat;
                    renderers[i].materials = rendMats;
                }
                else
                {
                    renderers[i].material = mat;
                }
            }
        }
    }
}
