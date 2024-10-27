using UnityEngine.Events;
using UnityEngine;
using System;

namespace Core3lb
{
    public abstract class BaseXRGrabObject : MonoBehaviour
    {
        [CoreReadOnly]
        //Bool to check if something is Grabbed
        public bool isGrabbed;
        //Allows for hovering objects that can be thrown
        public bool kinematicOnDrop = true; 
        [CoreReadOnly]
        public XRHand currentHand;

        [CoreReadOnly]
        public Rigidbody body;

        [HideInInspector]
        public bool isLocked = false;

        [CoreToggleHeader("Show Events")]
        public bool showEvents;
        [CoreShowIf("showEvents")]
        public UnityEvent onEnter; //Hover
        [CoreShowIf("showEvents")]
        public UnityEvent onExit; //UnHover
        [CoreShowIf("showEvents")]
        public UnityEvent onGrab; //Grab
        [CoreShowIf("showEvents")]
        public UnityEvent onDrop; //Ungrab

        [HideInInspector] //Attach these for Networking or internal starting Grab Which hand grabbed it!
        public UnityEvent onEnterInternal; //Grab
        [HideInInspector] //Attach these for drop or internal starting dropping
        public UnityEvent onExitInternal; //Ungrab What velocity was it ungrabbed
        [HideInInspector] //Attach these for Networking or internal starting Grab Which hand grabbed it!
        public UnityEvent onGrabInternalStart; //Grab
        [HideInInspector] //Attach these for drop or internal starting dropping
        public UnityEvent onDropInternalStart; //Ungrab What velocity was it ungrabbed

        //#pragma warning restore CS0067
        //private UnityEvent handForced;

        //this is for networking
        protected bool isOverridden = false;

        public enum eGrabEvents
        {
            Enter,
            Exit,
            Grab,
            Drop
        }

        //Main Events
        public abstract void ForceDrop();
        public abstract void ForceGrab(XRHand selectedHand);

        //write a get for body
        public Rigidbody myBody
        {
            get
            {
                body = gameObject.GetComponentIfNull<Rigidbody>(body);
                return body;
            }
        }

        public void _SetHand(XRHand hand)
        {
            currentHand = hand;
        }

        public void _SetKinematicOnDrop(bool to)
        {
            kinematicOnDrop = to;
        }

        //Event Functions
        public virtual void Grab()
        {
            CallXRGrabEvents(eGrabEvents.Grab);
        }

        public virtual void Drop()
        {
            CallXRGrabEvents(eGrabEvents.Drop);
        }

        public virtual void EnterEvent()
        {
            CallXRGrabEvents(eGrabEvents.Enter);
        }

        public virtual void ExitEvent()
        {
            CallXRGrabEvents(eGrabEvents.Exit);
        }


        public virtual void CallXRGrabEvents(eGrabEvents whichEvent, bool bypassOverride = false)
        {
            if (!bypassOverride)
            {
                if (isOverridden) //Networking Override
                {
                    return;
                }
            }
            switch (whichEvent)
            {
                case eGrabEvents.Enter:
                    ActualEnterEvent();
                    break;
                case eGrabEvents.Exit:
                    ActualExitEvent();
                    break;
                case eGrabEvents.Grab:
                    ActualGrab();
                    break;
                case eGrabEvents.Drop:
                    ActualDrop();
                    break;
                default:
                    break;
            }
        }

        public virtual void ActualGrab()
        {
            onGrab?.Invoke();
        }

        public virtual void ActualDrop()
        {
            onDrop?.Invoke();
        }

        public virtual void ActualEnterEvent()
        {
            onEnter?.Invoke();
        }

        public virtual void ActualExitEvent()
        {
            onExit?.Invoke();
        }
    }
}
