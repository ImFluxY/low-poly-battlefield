using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helicopter
{
    public class HeliTail_Rotor : MonoBehaviour, I_Heli_Rotor
    {
        #region Variables
        [Header("Tail Rotor Properties")]
        public float rotationSpeedModifier = 1.5f;
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
            //Debug.Log("Updating Tail Rotor");
            transform.Rotate(Vector3.right, dps * rotationSpeedModifier);
        }
        #endregion
    }
}
