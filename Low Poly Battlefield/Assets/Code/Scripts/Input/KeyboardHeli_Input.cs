using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helicopter
{
    public class KeyboardHeli_Input : BaseHeli_Input
    {
        #region Variables
        [Header("Heli Keyboard Inputs")]
        #endregion

        #region Properties

        private float throttleInput = 0f;
        public float RawThrottleInput
        {
            get { return throttleInput;  }
        }

        protected float stickyThrottle = 0f;
        public float StickyThrottle
        {
            get { return stickyThrottle; }
        }

        private float collectiveInput = 0f;
        public float CollectiveInput
        {
            get { return collectiveInput; }
        }

        private float stickyCollectiveInput = 0f;
        public float StickyCollectiveInput
        {
            get { return stickyCollectiveInput; }
        }

        private Vector2 cyclicInput = Vector2.zero;
        public Vector2 CyclicInput
        {
            get { return cyclicInput; }
        }

        private float pedalInput = 0f;
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

        #region Custom Methods

        protected override void HandleInputs()
        {
            base.HandleInputs();

            //Input Methods
            HandleThrottle();
            HandlePedal();
            HandleCollective();
            HandleCyclic();
            HandleFireButton();

            //Utility Methods
            ClampInputs();
            HandleStickyThrottle();
            HandleStickyCollective();
        }

        protected virtual void HandleThrottle()
        {
            throttleInput = Input.GetAxis("Throttle");
        }

        protected virtual void HandleCollective()
        {
            collectiveInput = Input.GetAxis("Collective");
        }

        protected virtual void HandleCyclic()
        {
            cyclicInput.y = vertical;
            cyclicInput.x = horizontal;
        }

        protected virtual void HandlePedal()
        {
            pedalInput = Input.GetAxis("Pedal");
        }

        protected void ClampInputs()
        {
            throttleInput = Mathf.Clamp(throttleInput, -1f, 1f);
            collectiveInput = Mathf.Clamp(collectiveInput, -1f, 1f);
            cyclicInput = Vector2.ClampMagnitude(cyclicInput, 1f);
            pedalInput = Mathf.Clamp(pedalInput, -1f, 1f);
        }

        protected void HandleStickyThrottle()
        {
            stickyThrottle += RawThrottleInput * Time.deltaTime;
            stickyThrottle = Mathf.Clamp01(stickyThrottle);
            //Debug.Log(stickyThrottle);
        }

        protected void HandleStickyCollective()
        {
            stickyCollectiveInput += collectiveInput * Time.deltaTime;
            stickyCollectiveInput = Mathf.Clamp01(stickyCollectiveInput);
            //Debug.Log(stickyCollectiveInput);
        }

        protected void HandleFireButton()
        {
            fire = Input.GetButton("Fire1");
        }
        #endregion
    }
}
