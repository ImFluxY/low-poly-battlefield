using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Car
{
    public enum InputType
    {
        Keyboard,
        PS4
    }

    [RequireComponent(typeof(KeyboardCar_Input), typeof(Car_Characteristics))]
    public class Car_Input_Controller : MonoBehaviour
    {
        #region Variables
        [Header("Input Properties")]
        public InputType inputType = InputType.Keyboard;

        private KeyboardCar_Input keyInput;

        private float acceleratorInput;
        public float AcceleratorInput
        {
            get { return acceleratorInput; }
        }

        private float directionInput;
        public float DirectionInput
        {
            get { return directionInput; }
        }

        private bool breaking;
        public bool Breaking
        {
            get { return breaking; }
        }
        #endregion

        #region BuiltIn Methods
        private void Start()
        {
            keyInput = GetComponent<KeyboardCar_Input>();

            if (keyInput)
                SetInputType(inputType);
        }

        private void Update()
        {
            switch (inputType)
            {
                case InputType.Keyboard:
                    acceleratorInput = keyInput.AcceleratorInput;
                    directionInput = keyInput.DirectionInput;
                    breaking = keyInput.Breaking;
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


