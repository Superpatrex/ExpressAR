using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Core3lb
{
    public class DoMovement : MonoBehaviour
    {
        [Tooltip("Target for that moves")]
        [CoreEmphasize]
        public Transform objectToMove;
        public bool isLocal;
        [CoreEmphasize]
        [CoreToggleHeader("Use Time")]
        public bool useTime = true;
        [CoreShowIf("useTime")]
        [Tooltip("In seconds")]
        public float moveTime = 1;
        [CoreShowIf("useTime")]
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1); //Defaults to a Linear Curve
        [CoreHideIf("useTime")]
        public float moveSpeed = 2;
        [Space]
        public UnityEvent tweenDone;
        Coroutine moveTween;
        Coroutine rotateTween;

        [CoreToggleHeader("Test Movement")]
        public bool debugMovement = false;
        [CoreShowIf("debugMovement")]
        public UnityEvent testMovement;

        public Vector3 GetPosition
        {
            get
            {
                if (isLocal)
                {
                    return objectToMove.localPosition;
                }
                else
                {
                    return objectToMove.position;
                }
            }
        }


        public virtual void Start()
        {
            objectToMove = gameObject.GetComponentIfNull<Transform>(objectToMove);
        }

        public virtual void _ObjectMoveTo(Transform WhereTo)
        {
            if (moveTween != null)
            {
                StopCoroutine(moveTween);

            }
            moveTween = StartCoroutine(MoveToTarget(GetPosition, WhereTo.position));
        }

        public virtual void _TeleportObject(Transform WhereTo)
        {
            if (moveTween != null)
            {
                StopCoroutine(moveTween);

            }
            objectToMove.transform.position = WhereTo.position;
        }

        public virtual void _TeleportObjectWithRotation(Transform WhereTo)
        {
            if (moveTween != null)
            {
                StopCoroutine(moveTween);

            }
            objectToMove.transform.position = WhereTo.position;
            objectToMove.transform.rotation = WhereTo.rotation;
        }


        public virtual void _ChangeMoveSpeed(float chg)
        {
            moveTime = chg;
        }

        [CoreButton]
        protected void TestMovement()
        {
            testMovement.Invoke();
        }


        public virtual void InternalMove(Vector3 where)
        {
            if (moveTween != null)
            {
                StopCoroutine(moveTween);

            }
            moveTween = StartCoroutine(MoveToTarget(GetPosition, where));
        }


        //############### EVENT FUNCTIONS
        //###############################################################
        public virtual void _MoveByX(float x)
        {
            InternalMove(GetPosition + Vector3.right * x);
        }

        public virtual void _MoveByY(float y)
        {
            InternalMove(GetPosition + Vector3.up * y);
        }

        public virtual void _MoveByZ(float z)
        {
            InternalMove(GetPosition + Vector3.forward * z);
        }

        public virtual void _MoveToX(float x)
        {
            Vector3 holder = GetPosition;
            holder.x = x;
            InternalMove(holder);
        }

        public virtual void _MoveToY(float y)
        {
            Vector3 holder = GetPosition;
            holder.y = y;
            InternalMove(holder);
        }

        public virtual void _MoveToZ(float z)
        {
            Vector3 holder = GetPosition;
            holder.z = z;
            InternalMove(holder);
        }

        protected virtual IEnumerator MoveToTarget(Vector3 startPosition, Vector3 targetPosition)
        {
            Vector3 currentPosition;
            if (useTime)
            {
                float elapsedTime = 0f;
                while (elapsedTime < moveTime)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / moveTime;
                    currentPosition = Vector3.Lerp(startPosition, targetPosition, curve.Evaluate(t));
                    if (isLocal)
                    {
                        objectToMove.transform.localPosition = currentPosition;
                    }
                    else
                    {
                        objectToMove.transform.position = currentPosition;
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                while (Vector3.Distance(isLocal ? objectToMove.transform.localPosition : objectToMove.transform.position, targetPosition) > 0.01f)
                {
                    currentPosition = Vector3.MoveTowards(isLocal ? objectToMove.transform.localPosition : objectToMove.transform.position, targetPosition, (moveSpeed * 5) * Time.deltaTime);
                    if (isLocal)
                    {
                        objectToMove.transform.localPosition = currentPosition;
                    }
                    else
                    {
                        objectToMove.transform.position = currentPosition;
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            tweenDone.Invoke();
        }
    }
}
