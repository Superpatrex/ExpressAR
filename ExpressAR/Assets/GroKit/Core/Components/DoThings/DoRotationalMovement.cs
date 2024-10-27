using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Core3lb
{
    public class DoRotationalMovement : MonoBehaviour
    {
        [Tooltip("Target that moves")]
        public Transform objectToRotate;
        public bool isLocal;
        [CoreToggleHeader("Time Based")]
        public bool useTime = true;
        [Tooltip("In seconds")]
        [CoreShowIf("useTime")]
        public float rotateTime = 1;
        [CoreShowIf("useTime")]
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1); // Defaults to a Linear Curve
        [CoreHideIf("useTime")]
        public float rotateSpeed = 2;
        [Space]
        public UnityEvent tweenDone;
        Coroutine rotateTween;

        [CoreToggleHeader("Test Event")]
        [SerializeField]
        private bool debugMovement = false;
        [CoreShowIf("debugMovement")]
        [SerializeField]
        private UnityEvent testMovement;

        // Keep track of cumulative Rotation
        private float currentAngleX = 0f;
        private float currentAngleY = 0f;
        private float currentAngleZ = 0f;

        public virtual void Start()
        {
            objectToRotate = gameObject.GetComponent<Transform>();
            //
            ResetCurrentAngles();
        }

        /// <summary>
        /// Resets the current angles restarting the counter
        /// </summary>
        public void ResetCurrentAngles()
        {
            Vector3 initialAngles = isLocal ? objectToRotate.localEulerAngles : objectToRotate.eulerAngles;
            currentAngleX = initialAngles.x;
            currentAngleY = initialAngles.y;
            currentAngleZ = initialAngles.z;
        }

        /// <summary>
        /// Change the rotation speed and time to this value
        /// </summary>
        /// <param name="chg"></param>
        public virtual void _ChangeRotateSpeedTime(float chg)
        {
            rotateTime = chg;
            rotateSpeed = chg;
        }


        /// <summary>
        /// Shorthand for getting local or global values
        /// </summary>
        public Quaternion GetRotationFromObject
        {
            get
            {
                if (isLocal)
                {
                    return objectToRotate.localRotation;
                }
                else
                {
                    return objectToRotate.rotation;
                }
            }
        }

        [CoreButton("Do Test Event")]
        protected void TestMovement()
        {
            testMovement.Invoke();
        }

        // Rotation
        public virtual void InternalRotateAngle(float startAngle, float targetAngle, Vector3 axis)
        {
            if (rotateTween != null)
            {
                StopCoroutine(rotateTween);
            }
            rotateTween = StartCoroutine(RotateToTargetAngle(startAngle, targetAngle, axis));
        }

        // ######################## EVENT FUNCTIONS
        // #######################################################################

        public virtual void _ObjectRotateTo(Transform targetRotation)
        {
            if (rotateTween != null)
            {
                StopCoroutine(rotateTween);
            }
            rotateTween = StartCoroutine(RotateToTarget(objectToRotate.rotation, targetRotation.rotation));
        }

        public virtual void _ObjectSnapRotateTo(Transform targetRotation)
        {
            objectToRotate.rotation = targetRotation.rotation;
        }

        public virtual void _RotateByX(float angle)
        {
            float startAngle = currentAngleX;
            float targetAngle = startAngle + angle;
            InternalRotateAngle(startAngle, targetAngle, Vector3.right);
        }

        public virtual void _RotateByY(float angle)
        {
            float startAngle = currentAngleY;
            float targetAngle = startAngle + angle;
            InternalRotateAngle(startAngle, targetAngle, Vector3.up);
        }

        public virtual void _RotateByZ(float angle)
        {
            float startAngle = currentAngleZ;
            float targetAngle = startAngle + angle;
            InternalRotateAngle(startAngle, targetAngle, Vector3.forward);
        }

        // World Rotate To
        public virtual void _RotateToX(float angle)
        {
            float startAngle = currentAngleX;
            float targetAngle = angle;
            InternalRotateAngle(startAngle, targetAngle, Vector3.right);
        }

        public virtual void _RotateToY(float angle)
        {
            float startAngle = currentAngleY;
            float targetAngle = angle;
            InternalRotateAngle(startAngle, targetAngle, Vector3.up);
        }

        public virtual void _RotateToZ(float angle)
        {
            float startAngle = currentAngleZ;
            float targetAngle = angle;
            InternalRotateAngle(startAngle, targetAngle, Vector3.forward);
        }

        protected virtual IEnumerator RotateToTarget(Quaternion startRotation, Quaternion targetRotation)
        {
            float elapsedTime = 0f;
            if (!useTime)
            {
                while (Quaternion.Angle(isLocal ? objectToRotate.localRotation : objectToRotate.rotation, targetRotation) > 0.01f)
                {
                    Quaternion holder = Quaternion.RotateTowards(GetRotationFromObject, targetRotation, rotateSpeed * 10 * Time.deltaTime);
                    if (isLocal)
                    {
                        objectToRotate.localRotation = holder;
                    }
                    else
                    {
                        objectToRotate.rotation = holder;
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                while (elapsedTime < rotateTime)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / rotateTime;
                    Quaternion currentRotation = Quaternion.Lerp(startRotation, targetRotation, curve.Evaluate(t));
                    if (isLocal)
                    {
                        objectToRotate.localRotation = currentRotation;
                    }
                    else
                    {
                        objectToRotate.rotation = currentRotation;
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            tweenDone.Invoke();
        }

        protected virtual IEnumerator RotateToTargetAngle(float startAngle, float targetAngle, Vector3 axis)
        {
            float elapsedTime = 0f;
            float currentAngle = startAngle;

            if (!useTime)
            {
                while (Mathf.Abs(targetAngle - currentAngle) > 0.01f)
                {
                    currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotateSpeed * 10 * Time.deltaTime);
                    ApplyRotation(currentAngle, axis);
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                while (elapsedTime < rotateTime)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / rotateTime;
                    currentAngle = Mathf.Lerp(startAngle, targetAngle, curve.Evaluate(t));
                    ApplyRotation(currentAngle, axis);
                    yield return new WaitForFixedUpdate();
                }
            }
            tweenDone.Invoke();
        }

        private void ApplyRotation(float currentAngle, Vector3 whatVector)
        {
            if (whatVector == Vector3.right)
            {
                currentAngleX = currentAngle;
            }
            if (whatVector == Vector3.up)
            {
                currentAngleY = currentAngle;
            }
            if (whatVector == Vector3.forward)
            {
                currentAngleZ = currentAngle;
            }
            Quaternion rotation = Quaternion.Euler(currentAngleX, currentAngleY, currentAngleZ);
            if (isLocal)
            {
                objectToRotate.localRotation = rotation;
            }
            else
            {
                objectToRotate.rotation = rotation;
            }
        }
    }
}
