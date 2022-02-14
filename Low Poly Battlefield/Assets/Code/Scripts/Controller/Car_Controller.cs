using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Car
{
    [RequireComponent(typeof(Car_Input_Controller), typeof(KeyboardCar_Input))]
    public class Car_Controller : Car_Base_RBController
    {
        [Header("Controlling")]
        public bool isControlled;

        #region Variables
        [Header("Car Properties")]
        public Car_Engine engine;
        public float KPH;

        private Car_Input_Controller input;
        private Car_Characteristics characteristics;
        #endregion

        #region Builtin Methods
        public override void Start()
        {
            base.Start();

            characteristics = GetComponent<Car_Characteristics>();
        }
        #endregion

        #region Custom Methods
        protected override void HandlePhysics()
        {
            if(!isControlled)
                return;

            input = GetComponent<Car_Input_Controller>();
            if (input)
            {
                HandleEngines();
                HandleCharacteristics();
            }
        }

        #endregion

        #region Car Controle Methods
        protected virtual void HandleEngines()
        {
            KPH = rb.velocity.magnitude * 3.6f;
        }

        protected virtual void HandleCharacteristics()
        {
            if (characteristics)
            {
                characteristics.UpdateCharacteristics(input, this, rb);
            }
        }
        #endregion
    }
}