using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core3lb
{
    public class FakeRigidBody : MonoBehaviour
    {

        // Fake Rigidbody will react like a Rigidbody but is not one. It will go through objects, so best to set them as triggers.

        // Kinematic state
        public bool isKinematic = false;

        // Use Unity's gravity
        public bool useGravity = true;

        // Damping factors
        public float linearDrag = 0.1f;
        public float angularDrag = 0.05f;

        // Mass of the object
        public float mass = 1.0f;

        // Linear motion variables
        private Vector3 velocity = Vector3.zero;
        private Vector3 acceleration = Vector3.zero;

        // Angular motion variables
        private Vector3 angularVelocity = Vector3.zero;
        private Vector3 angularAcceleration = Vector3.zero;


        public virtual void _DoBlast(float amount = 300)
        {
            _AddForce(Vector3.forward * amount);
        }

        void FixedUpdate()
        {
            if (!isKinematic)
            {
                // Apply gravity if enabled
                if (useGravity)
                {
                    acceleration += Physics.gravity;
                }

                // Linear motion
                velocity += acceleration * Time.fixedDeltaTime;

                // Apply linear drag
                velocity *= Mathf.Clamp01(1.0f - linearDrag * Time.fixedDeltaTime);

                transform.position += velocity * Time.fixedDeltaTime;

                // Angular motion
                angularVelocity += angularAcceleration * Time.fixedDeltaTime;

                // Apply angular drag
                angularVelocity *= Mathf.Clamp01(1.0f - angularDrag * Time.fixedDeltaTime);

                Quaternion deltaRotation = Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);
                transform.rotation = transform.rotation * deltaRotation;

                // Reset accelerations for the next frame
                acceleration = Vector3.zero;
                angularAcceleration = Vector3.zero;
            }
        }

        /// <summary>
        /// Fake ExplosiveForce
        /// </summary>
        /// <param name="explosionForce"></param>
        /// <param name="explosionPosition"></param>
        /// <param name="explosionRadius"></param>
        /// <param name="upwardsModifier"></param>
        public virtual void _AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0f)
        {
            // Calculate the vector from the explosion position to the object's position
            Vector3 explosionToTarget = transform.position - explosionPosition;

            // Calculate the distance between the explosion position and the object
            float distance = explosionToTarget.magnitude;

            // If the object is outside the explosion radius, do nothing
            if (distance > explosionRadius)
                return;

            // Normalize the direction
            Vector3 forceDirection = explosionToTarget.normalized;

            // Apply upwards modifier
            if (upwardsModifier != 0f)
            {
                forceDirection.y += upwardsModifier;
                forceDirection = forceDirection.normalized;
            }

            // Calculate attenuation of the force based on distance
            float attenuation = 1 - (distance / explosionRadius);

            // Calculate the final force vector
            Vector3 force = forceDirection * explosionForce * attenuation;

            // Apply the force to the object
            _AddForce(force);
        }


        /// <summary>
        /// Adds Force Just like a RigidBody
        /// </summary>
        /// <param name="force"></param>
        /// <param name="mode"></param>
        public virtual void _AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
        {
            if (!isKinematic)
            {
                switch (mode)
                {
                    case ForceMode.Force:
                        // Adds a continuous force, considering mass (F = m * a)
                        acceleration += force / mass;
                        break;
                    case ForceMode.Acceleration:
                        // Adds a continuous acceleration, ignoring mass
                        acceleration += force;
                        break;
                    case ForceMode.Impulse:
                        // Adds an instant force impulse, considering mass
                        velocity += force / mass;
                        break;
                    case ForceMode.VelocityChange:
                        // Adds an instant velocity change, ignoring mass
                        velocity += force;
                        break;
                }
            }
        }

        // Add angular force (torque)
        public virtual void _AddTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
        {
            if (!isKinematic)
            {
                switch (mode)
                {
                    case ForceMode.Force:
                        // Applies a continuous torque, considering mass (t = I * a)
                        angularAcceleration += torque / mass;
                        break;
                    case ForceMode.Acceleration:
                        // Applies a continuous angular acceleration, ignoring mass
                        angularAcceleration += torque;
                        break;
                    case ForceMode.Impulse:
                        // Applies an instant torque impulse, considering mass
                        angularVelocity += torque / mass;
                        break;
                    case ForceMode.VelocityChange:
                        // Applies an instant angular velocity change, ignoring mass
                        angularVelocity += torque;
                        break;
                }
            }
        }

        // Set kinematic state
        public virtual void _SetKinematic(bool state)
        {
            isKinematic = state;
            if (isKinematic)
            {
                velocity = Vector3.zero;
                acceleration = Vector3.zero;
                angularVelocity = Vector3.zero;
                angularAcceleration = Vector3.zero;
            }
        }
    }
}