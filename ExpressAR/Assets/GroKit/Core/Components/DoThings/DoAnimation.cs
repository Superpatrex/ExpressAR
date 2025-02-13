using UnityEngine;

namespace Core3lb
{
    public class DoAnimation : MonoBehaviour
    {
        public Animator myAnimate;


        [Header("Set Params On Start")]

        public string param;
        public bool setOnAwake;
        [CoreShowIf("setOnAwake")]
        public bool setBoolTrue;
        [CoreShowIf("setOnAwake")]
        public bool setBoolfalse;
        [CoreShowIf("setOnAwake")]
        public bool SetTrigger;

        private void Start()
        {
            if (myAnimate)
            {
                if (setOnAwake)
                {
                    if (setBoolTrue)
                    {
                        myAnimate.SetBool(param, true);
                    }
                    if (setBoolfalse)
                    {
                        myAnimate.SetBool(param, false);
                    }
                    if (SetTrigger)
                    {
                        myAnimate.SetTrigger(param);
                    }
                }
            }
            else
            {
                Debug.LogError("You Must Assign an Animator");
            }
        }

        public virtual void _SetBoolParam(bool chg)
        {
            myAnimate.SetBool(param, chg);
        }

        public virtual void _SetTriggerParma()
        {
            myAnimate.SetTrigger(param);
        }

        public virtual void _RunTrigger(string chg)
        {
            myAnimate.SetTrigger(chg);
        }

        public virtual void _SetBoolTrue(string chg)
        {
            myAnimate.SetBool(chg, true);
        }

        public virtual void _SetBoolFalse(string chg)
        {
            myAnimate.SetBool(chg, false);
        }
    }
}
