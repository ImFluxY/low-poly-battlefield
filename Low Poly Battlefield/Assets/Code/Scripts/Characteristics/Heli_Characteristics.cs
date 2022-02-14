using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helicopter
{
    public class Heli_Characteristics : MonoBehaviour
    {
        #region Variables
        [Header("Lift Properties")]
        public float maxLiftForce = 10f;
        public HeliMain_Rotor mainRotor;
        [Space]

        [Header("Tail Rotor Properties")]
        public float tailForce = 2f;
        [Space]

        [Header("Cyclic Properties")]
        public float cyclicForce = 2f;
        public float cyclicForceMulitplier = 1000f;

        [Header("Auto Level Properties")]
        public float autoLevelForce = 2f;

        private Vector3 flatFwd;
        private float forwardDot;
        private Vector3 flatRight;
        private float rightDot;

        #endregion

        #region Builtin Methods
        #endregion

        #region Custom Methods
        public void UpdateCharacteristics(Rigidbody rb, Heli_Input_Controller input)
        {
            HandleLift(rb, input);
            HandleCyclic(rb, input);
            HandlePedals(rb, input);

            CalculateAngles();
            AutoLevel(rb);
        }

        protected virtual void HandleLift(Rigidbody rb, Heli_Input_Controller input)
        {
            //Debug.Log("Handling Lift");
            if(mainRotor)
            {
                Vector3 liftForce = transform.up * (Physics.gravity.magnitude + maxLiftForce) * rb.mass;
                float normalizedRPMs = mainRotor.CurrentRPMs / 500f;
                rb.AddForce(liftForce * Mathf.Pow(normalizedRPMs, 2f) * Mathf.Pow(input.StickyCollectiveInput, 2f), ForceMode.Force);
            }
        }

        protected virtual void HandleCyclic(Rigidbody rb, Heli_Input_Controller input)
        {
            //Debug.Log("Handling Cycle");
            float cyclicZForce = -input.CyclicInput.x * cyclicForce;
            rb.AddRelativeTorque(Vector3.forward * cyclicZForce, ForceMode.Acceleration);

            float cyclicXForce = input.CyclicInput.y * cyclicForce;
            rb.AddRelativeTorque(Vector3.right * cyclicXForce, ForceMode.Acceleration);

            //Apply force based off of the Dot Product values
            Vector3 forwardVec = flatFwd * forwardDot;
            Vector3 rightVec = flatRight * rightDot;
            Vector3 finalCyclicDir = Vector3.ClampMagnitude(forwardVec + rightVec, 1f) * (cyclicForce * cyclicForceMulitplier);
            //Debug.DrawRay(transform.position, finalCyclicDir, Color.green);
            rb.AddForce(finalCyclicDir, ForceMode.Force);
        }

        protected virtual void HandlePedals(Rigidbody rb, Heli_Input_Controller input)
        {
            //Debug.Log("Handling Pedals");
            rb.AddTorque(Vector3.up * input.PedalInput * tailForce, ForceMode.Acceleration);
        }

        private void CalculateAngles()
        {
            //Calculate Flat Forward
            flatFwd = transform.forward;
            flatFwd.y = 0f;
            flatFwd = flatFwd.normalized;
            Debug.DrawRay(transform.position, flatFwd, Color.blue);

            //Calculate Flat Right
            flatRight = transform.right;
            flatRight.y = 0f;
            flatRight = flatRight.normalized;
            Debug.DrawRay(transform.position, flatRight, Color.red);

            //Calculate Angles
            forwardDot = Vector3.Dot(transform.up, flatFwd);
            rightDot = Vector3.Dot(transform.up, flatRight);
            //Debug.Log(string.Format("Fwd: {0} - Right : {1}", forwardDot.ToString("0.0"), rightDot.ToString("0.0")));
        }

        private void AutoLevel(Rigidbody rb)
        {
            float rightForce = -forwardDot * autoLevelForce;
            float forwardForce = rightDot * autoLevelForce;

            rb.AddRelativeTorque(Vector3.right * rightForce, ForceMode.Acceleration);
            rb.AddRelativeTorque(Vector3.forward * forwardForce, ForceMode.Acceleration);
        }
        #endregion
    }
}
