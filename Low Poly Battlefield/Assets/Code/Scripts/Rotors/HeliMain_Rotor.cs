using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helicopter
{
    public class HeliMain_Rotor : MonoBehaviour, I_Heli_Rotor
    {
        #region Variables
        #endregion

        #region Properties
        private float currentRPMs;
        public float CurrentRPMs
        {
            get { return currentRPMs; }
        }
        #endregion

        #region Builtin Methods
        // Start is called before the first frame update
        void Start()
        {

        }
        #endregion

        #region Interface Methods
        public void UpdateRotor(float dps)
        {
            //Debug.Log("Updating Main Rotor");
            currentRPMs = (dps / 360) * 60f;
            //Debug.Log(currentRPMs);
            transform.Rotate(Vector3.up, dps * Time.deltaTime * 0.5f);
        }
        #endregion
    }
}