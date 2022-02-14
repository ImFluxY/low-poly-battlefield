using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Car
{
    public class KeyboardCar_Input : BaseCar_Input
    {
        #region Variables
        #endregion

        #region Properties
        private float acceleratorInput = 0f;
        public float AcceleratorInput
        {
            get { return acceleratorInput; }
        }

        private float directionInput = 0f;
        public float DirectionInput
        {
            get { return directionInput; }
        }

        private bool breaking = false;
        public bool Breaking
        {
            get { return breaking; }
        }
        #endregion

        #region Custom Methods

        protected override void HandleInputs()
        {
            base.HandleInputs();

            HandleAccelerator();
            HandleDirection();
            HandleBreak();
        }

        protected virtual void HandleAccelerator()
        {
            acceleratorInput = vertical;
            //Debug.Log("Accelerator : " + acceleratorInput);
        }

        protected virtual void HandleDirection()
        {
            directionInput = horizontal;
            //Debug.Log("Direction : " + directionInput);
        }

        protected virtual void HandleBreak()
        {
            breaking = Input.GetButton("Jump");
            //Debug.Log("Breaking : " + breaking);
        }

        #endregion
    }
}
