using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helicopter
{
    [RequireComponent(typeof(Heli_Input_Controller), typeof(KeyboardHeli_Input))]
    public class Heli_Controller : Heli_Base_RBController
    {
        #region Variables
        [Header("Controlling")]
        public bool isControlled;

        [Header("Helicopter Properties")]
        public List<Heli_Engine> engines = new List<Heli_Engine>();

        [Header("Helicopter Rotors")]
        public Heli_Rotor_Controller rotorCtrl;

        private Heli_Input_Controller input;
        private Heli_Characteristics characteristics;
        #endregion

        #region Builtin Methods
        public override void Start()
        {
            base.Start();

            characteristics = GetComponent<Heli_Characteristics>();
        }
        #endregion

        #region Custom Methods
        protected override void HandlePhysics()
        {
            if (!isControlled)
                return;

            input = GetComponent<Heli_Input_Controller>();
            if(input)
            {
                HandleEngines();
                HandleRotors();
                HandleCharacteristics();
            }
        }

        #endregion

        #region Helicopter Controle Methods
        protected virtual void HandleEngines()
        {
            for (int i = 0; i < engines.Count; i++)
            {
                engines[i].UpdateEngine(input.StickyThrottle);
                float finalPower = engines[i].CurrentHP;
            }
        }

        protected virtual void HandleRotors()
        {
            if(rotorCtrl && engines.Count > 0)
            {
                rotorCtrl.UpdateRotors(input, engines[0].CurrentRPM);
            }
        }

        protected virtual void HandleCharacteristics()
        {
            if(characteristics)
            {
                characteristics.UpdateCharacteristics(rb, input);
            }
        }
        #endregion
    }
}
