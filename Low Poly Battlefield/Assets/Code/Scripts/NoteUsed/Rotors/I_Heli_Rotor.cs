using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helicopter
{
    public interface I_Heli_Rotor
    {
        #region Methods
        void UpdateRotor(float dps);
        #endregion
    }
}