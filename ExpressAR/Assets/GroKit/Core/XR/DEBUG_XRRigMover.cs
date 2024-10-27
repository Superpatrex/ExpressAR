using UnityEngine;
using UnityEngine.InputSystem;
namespace Core3lb
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    public class DEBUG_XRRigMover : MonoBehaviour
    {
        [CoreEmphasize]
        public Transform whatToMove;
        public float speed = 3;
        public float verticalSpeed = 3;
        public float rotationSpeed = 50; // Speed of rotation
        public bool allowInBuilds = false;
        public bool headBased = true;

        public void Awake()
        {
            whatToMove = gameObject.GetComponentIfNull<Transform>(whatToMove);
        }

        void Update()
        {
            if (Application.isEditor || allowInBuilds)
            {
                // Initialize movement variables
                float moveX = 0f;
                float moveZ = 0f;

                // Check input for lateral (strafe) movement
                if (Keyboard.current.aKey.isPressed)
                    moveX = -1f;
                else if (Keyboard.current.dKey.isPressed)
                    moveX = 1f;

                // Check input for forward/backward movement
                if (Keyboard.current.wKey.isPressed)
                    moveZ = 1f;
                else if (Keyboard.current.sKey.isPressed)
                    moveZ = -1f;

                // Get the head camera's forward and right direction
                Transform headTransform = gameObject.transform;
                if (headBased)
                {
                    headTransform = XRPlayerController.instance.headCamera.transform;
                }
                Vector3 headForward = headTransform.forward;
                Vector3 headRight = headTransform.right;

                // Calculate the movement direction relative to the head's orientation
                Vector3 moveDirection = (headRight * moveX + headForward * moveZ).normalized;

                // Adjust movement direction for vertical movements using keys Q and E
                if (Keyboard.current.qKey.isPressed)
                {
                    moveDirection.y = -verticalSpeed;
                }
                else if (Keyboard.current.eKey.isPressed)
                {
                    moveDirection.y = verticalSpeed;
                }
                else
                {
                    // If no vertical movement, reset y component to zero
                    moveDirection.y = 0;
                }

                // Translate the object based on the calculated direction
                whatToMove.Translate(moveDirection * speed * Time.deltaTime, Space.World);
                // Handle rotation
                if (Keyboard.current.zKey.isPressed)
                {
                    whatToMove.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
                }
                else if (Keyboard.current.cKey.isPressed)
                {
                    whatToMove.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
                }
            }
        }
    }
}
