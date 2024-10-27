using UnityEngine;

namespace AICore3lb.Demo
{
    public class DemoRotator : MonoBehaviour
    {
        public Vector3 velocity;
        public bool randomize;
        public float randomSpeed;

        public void Awake()
        {
            if (randomize)
            {
                velocity = Random.insideUnitSphere * randomSpeed;
            }
        }

        public void FixedUpdate()
        {
            transform.Rotate(velocity);
        }
    }
}