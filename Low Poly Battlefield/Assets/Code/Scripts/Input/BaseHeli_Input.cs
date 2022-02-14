using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helicopter
{
    public class BaseHeli_Input : MonoBehaviour
    {
        #region Variables
        [Header("Base Input Properties")]
        protected float vertical = 0f;
        protected float horizontal = 0f;
        #endregion

        #region BuiltIn Methods

        void Update()
        {
            HandleInputs();
        }

        #endregion

        #region Custom Methods

        protected virtual void HandleInputs()
        {
            vertical = Input.GetAxis("Mouse Y");
            horizontal = Input.GetAxis("Mouse X");
        }

        #endregion
    }

}