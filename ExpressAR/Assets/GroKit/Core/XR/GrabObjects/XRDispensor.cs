using UnityEngine;

namespace Core3lb
{
    public class XRDispenser : XRInteract
    {
        [CoreHeader("Dispenser Settings")]
        [CoreRequired]
        public GroKitXRGrabObject objectToDispense;
        [Tooltip("Parent for Clean up")]
        public GameObject parent;

        public override void Interact()
        {
            interact.Invoke();
            SpawnAndForceGrabObject();
        }

        public virtual void SpawnAndForceGrabObject()
        {
            GroKitXRGrabObject currentObject = Instantiate(objectToDispense, transform.position, transform.rotation);
            if (parent != null)
            {
                currentObject.transform.parent = parent.transform;
            }
            currentObject.ForceGrab(lastUsedHand);
        }

    }
}
