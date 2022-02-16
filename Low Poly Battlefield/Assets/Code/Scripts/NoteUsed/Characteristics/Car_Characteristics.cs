using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Car
{
    public class Car_Characteristics : MonoBehaviour
    {
        public enum DriveType
        {
            frontWheelDrive,
            rearWheelDrive,
            allWheelDrive
        }

        #region Variables
        [Header("Car Properties")]
        public DriveType driveType = DriveType.frontWheelDrive;
        public GameObject[] wheelMesh = new GameObject[4];
        public WheelCollider[] wheelCollider = new WheelCollider[4];
        private WheelFrictionCurve forwardFriction, sidewaysFriction;
        public float handBrakeFrictionMultiplier = 2f;
        public float finalDrive = 3.4f;
        [Range(0.5f, 1.5f)] public float grip = 1;
        public float wheelRadius = 6, downForceValue = 10f, driftFactor, lastValue;

        [Header("Acceleration Properties")]
        public Car_Engine engine;
        [Space]

        [Header("Direction Properties")]
        public float maxSteerAngle = 45;
        private float steerAngle;
        [Space]

        [Header("Break Properties")]
        public float breakPower;
        private float currentBreakForce;

        [Header("Light Properties")]
        public GameObject FrontLights;
        public GameObject RearLights;
        #endregion

        #region Builtin Methods
        private void Update()
        {
            HandleLights();
        }
        #endregion

        #region Custom Methods
        public void UpdateCharacteristics(Car_Input_Controller input, Car_Controller controller, Rigidbody rb)
        {
            DownForce(rb);
            HandleCarMovement(input, controller);
            HandleDirection(input);
            HandleBreak(input, controller);
            UpdateWheels();
            HandleTraction(input, controller);
        }

        public bool isGrounded()
        {
            if (wheelCollider[0].isGrounded && wheelCollider[1].isGrounded && wheelCollider[2].isGrounded && wheelCollider[3].isGrounded)
                return true;
            else
                return false;
        }

        protected virtual void HandleCarMovement(Car_Input_Controller input, Car_Controller controller)
        {
            if(engine)
            {
                engine.UpdateEngine(wheelCollider, this, input, controller);
                
                switch(driveType)
                {
                    case DriveType.frontWheelDrive:
                        wheelCollider[0].motorTorque = engine.totalPower / 2;
                        wheelCollider[1].motorTorque = engine.totalPower / 2;

                        for (int i = 0; i < wheelCollider.Length; i++)
                        {
                            wheelCollider[i].brakeTorque = breakPower;
                        }
                        break;

                    case DriveType.rearWheelDrive:
                        wheelCollider[2].motorTorque = engine.totalPower / 2;
                        wheelCollider[3].motorTorque = engine.totalPower / 2;

                        for (int i = 0; i < wheelCollider.Length; i++)
                        {
                            wheelCollider[i].brakeTorque = breakPower;
                        }
                        break;

                    case DriveType.allWheelDrive:
                        for (int i = 0; i < wheelCollider.Length; i++)
                        {
                            wheelCollider[i].motorTorque = engine.totalPower / 4;
                            wheelCollider[i].brakeTorque = breakPower;
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        protected virtual void HandleDirection(Car_Input_Controller input)
        {
            //acerman steering formula
            //steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;

            if (input.DirectionInput > 0)
            {
                //rear tracks size is set to 1.5f       wheel base has been set to 2.55f
                wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (wheelRadius + (1.5f / 2))) * input.DirectionInput;
                wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (wheelRadius - (1.5f / 2))) * input.DirectionInput;
            }
            else if (input.DirectionInput < 0)
            {
                wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (wheelRadius - (1.5f / 2))) * input.DirectionInput;
                wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (wheelRadius + (1.5f / 2))) * input.DirectionInput;
                //transform.Rotate(Vector3.up * steerHelping);
            }
            else
            {
                wheelCollider[0].steerAngle = 0;
                wheelCollider[1].steerAngle = 0;
            }
        }

        protected virtual void HandleBreak(Car_Input_Controller input, Car_Controller controller)
        {
            if (input.AcceleratorInput < 0)
            {
                breakPower = (controller.KPH >= 25) ? 500 : 0;
                RearLights.SetActive(true);
            }
            else
            {
                RearLights.SetActive(false);
            }
        }

        protected virtual void HandleTraction(Car_Input_Controller input, Car_Controller controller)
        {
            //tine it takes to go from normal drive to drift 
            float driftSmothFactor = 1 * Time.deltaTime;

            //if(IM.handbrake){
            sidewaysFriction = wheelCollider[0].sidewaysFriction;
            forwardFriction = wheelCollider[0].forwardFriction;

            float velocity = 0;

            if (input.Breaking)
                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                    Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBrakeFrictionMultiplier, ref velocity, driftSmothFactor);
            else
                forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                    ((controller.KPH * handBrakeFrictionMultiplier) / 300) + grip;


            for (int i = 0; i < wheelCollider.Length; i++)
            {
                wheelCollider[i].sidewaysFriction = sidewaysFriction;
                wheelCollider[i].forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;
            //extra grip for the front wheels
            if (input.Breaking) {
                for (int i = 0; i < 2; i++)
                {
                    wheelCollider[i].sidewaysFriction = sidewaysFriction;
                    wheelCollider[i].forwardFriction = forwardFriction;
                }

            }
            /*
            else{

                forwardFriction = wheels[0].forwardFriction;
                sidewaysFriction = wheels[0].sidewaysFriction;

                forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = 
                    ((KPH * handBrakeFrictionMultiplier) / 300) + grip;

                for (int i = 0; i < wheels.Length; i++) {
                    wheels [i].forwardFriction = forwardFriction;
                    wheels [i].sidewaysFriction = sidewaysFriction;

                }

            }
            */

            //checks the amount of slip to control the drift
            for (int i = 2; i < wheelCollider.Length; i++)
            {

                WheelHit wheelHit;

                wheelCollider[i].GetGroundHit(out wheelHit);

                //smoke
                /*
                if (wheelHit.sidewaysSlip >= 0.3f || wheelHit.sidewaysSlip <= -0.3f || wheelHit.forwardSlip >= .3f || wheelHit.forwardSlip <= -0.3f)
                    playPauseSmoke = true;
                else
                    playPauseSmoke = false;
                */


                if (wheelHit.sidewaysSlip < 0) driftFactor = (1 + -input.DirectionInput) * Mathf.Abs(wheelHit.sidewaysSlip);

                if (wheelHit.sidewaysSlip > 0) driftFactor = (1 + input.DirectionInput) * Mathf.Abs(wheelHit.sidewaysSlip);
            }

        }

        private void DownForce(Rigidbody rb)
        {
            rb.AddForce(-transform.up * downForceValue * rb.velocity.magnitude);
        }

        protected virtual void UpdateWheels()
        {
            for (int i = 0; i < wheelCollider.Length; i++)
            {
                UpdateSingleWheel(wheelCollider[i], wheelMesh[i].transform);
            }
        }

        private void UpdateSingleWheel(WheelCollider wheel_col, Transform wheel_t)
        {
            Vector3 pos;
            Quaternion rot;
            wheel_col.GetWorldPose(out pos, out rot);
            wheel_t.position = pos;
            wheel_t.rotation = rot;
        }

        private void HandleLights()
        {
            if(Input.GetButtonDown("CarLight"))
            {
                FrontLights.SetActive(!FrontLights.activeSelf);
            }
        }

        #endregion
    }
}


