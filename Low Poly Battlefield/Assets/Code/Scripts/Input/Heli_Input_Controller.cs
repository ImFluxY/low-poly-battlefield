using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helicopter
{
    public enum InputType
    {
        Keyboard,
        PS4
    }

    [RequireComponent(typeof(KeyboardHeli_Input))]
    public class Heli_Input_Controller : MonoBehaviour
    {
        #region Variables
        [Header("Input Properties")]
        public InputType inputType = InputType.Keyboard;

        private KeyboardHeli_Input keyInput;

        private float throttleInput;
        public float ThrottleInput
        {
            get { return throttleInput; }
        }

        private float stickyThrottle;
        public float StickyThrottle
        {
            get { return stickyThrottle; }
        }

        private float collectiveInput;
        public float CollectiveInput
        {
            get { return collectiveInput; }
        }

        private float stickyCollectiveInput;
        public float StickyCollectiveInput
        {
            get { return stickyCollectiveInput; }
        }

        private Vector2 cyclicInput;
        public Vector2 CyclicInput
        {
            get { return cyclicInput; }
        }

        private float pedalInput;
        public float PedalInput
        {
            get { return pedalInput; }
        }

        protected bool fire = false;
        public bool Fire
        {
            get { return fire; }
        }
        #endregion

        #region BuiltIn Methods
        private void Start()
        {
            keyInput = GetComponent<KeyboardHeli_Input>();

            if(keyInput)
                SetInputType(inputType);
        }

        private void Update()
        {
            switch(inputType)
            {
                case InputType.Keyboard:
                    throttleInput = keyInput.RawThrottleInput;
                    collectiveInput = keyInput.CollectiveInput;
                    stickyCollectiveInput = keyInput.StickyCollectiveInput;
                    cyclicInput = keyInput.CyclicInput;
                    pedalInput = keyInput.PedalInput;
                    stickyThrottle = keyInput.StickyThrottle;
                    fire = keyInput.Fire;
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Custom Methods
        void SetInputType(InputType type)
        {
            if (type == InputType.Keyboard && keyInput)
            {
                keyInput.enabled = true;
            }
        }
        #endregion
    }
}
