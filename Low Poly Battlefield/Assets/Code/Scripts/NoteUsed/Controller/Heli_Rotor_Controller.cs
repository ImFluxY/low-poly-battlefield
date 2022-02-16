using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Helicopter
{
    public class Heli_Rotor_Controller : MonoBehaviour
    {
        #region Variables
        public float maxDps = 3000f;
        private List<I_Heli_Rotor> rotors;
        #endregion

        #region Builtin Methods
        private void Start()
        {
            rotors = GetComponentsInChildren<I_Heli_Rotor>().ToList<I_Heli_Rotor>();
        }
        #endregion

        #region Custom Methods
        public void UpdateRotors(Heli_Input_Controller input, float currentRPMs)
        {
            //Debug.Log("Updating Rotor controller");
            //Degrees per second calculation
            float dps = ((currentRPMs * 360f) / 60f);
            dps = Mathf.Clamp(dps, 0f, maxDps);
            
            //Update Rotors
            if(rotors.Count > 0)
            {
                foreach(var rotor in rotors)
                {
                    rotor.UpdateRotor(dps);
                }
            }
        }
        #endregion
    }
}