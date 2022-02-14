using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Car
{
    public class Car_Engine : MonoBehaviour
    {
        #region Variables

        [Header("Power Properties")]
        public float minRPM = 500;
        public float maxRPM = 8000;
        public float[] gears;
        public float[] gearChangeSpeed;
        public float powerDelay = 2f;
        public AnimationCurve enginePower = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        public float smoothTime = 0.2f;
        [Space]

        [Header("Informations")]
        public int gearNum = 1;
        public bool playPauseSmoke = false, hasFinished;
        public float engineRPM;
        public bool reverse = false;
        public float totalPower;
        public bool engineLerp = false;
        public float engineLerpValue, wheelsRPM;
        #endregion

        #region Properties
        private float currentHP;
        public float CurrentHP
        {
            get { return currentHP; }

        }

        private float currentRPM;
        public float CurrentRPM
        {
            get { return currentRPM; }

        }

        private float currentKmH;
        public float CurrentKmH
        {
            get { return currentKmH; }

        }
        #endregion

        #region Custom Methods
        public void UpdateEngine(WheelCollider[] wheels, Car_Characteristics characteristics, Car_Input_Controller input, Car_Controller controller)
        {
            calculateEnginePower(wheels, characteristics, input, controller);
        }

        private float velocity = 0.0f;

        private void calculateEnginePower(WheelCollider[] wheels, Car_Characteristics characteristics, Car_Input_Controller input, Car_Controller controller)
        {
            lerpEngine();
            wheelRPM(wheels);

            if (engineRPM >= maxRPM)
            {
                setEngineLerp(maxRPM - 1000);
            }
            if (!engineLerp)
            {
                engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * characteristics.finalDrive * (gears[gearNum])), ref velocity, smoothTime);
                totalPower = enginePower.Evaluate(engineRPM) * (gears[gearNum]) * characteristics.finalDrive * input.AcceleratorInput;
            }

            shifter(characteristics, controller);
        }

        private void wheelRPM(WheelCollider[] wheels)
        {
            float sum = 0;
            int R = 0;
            for (int i = 0; i < wheels.Length; i++)
            {
                sum += wheels[i].rpm;
                R++;
            }
            wheelsRPM = (R != 0) ? sum / R : 0;

            if (wheelsRPM < 0 && !reverse)
            {
                reverse = true;
            }
            else if (wheelsRPM > 0 && reverse)
            {
                reverse = false;
            }
        }

        private bool checkGears(Car_Controller controller)
        {
            if (controller.KPH >= gearChangeSpeed[gearNum]) return true;
            else return false;
        }

        private void shifter(Car_Characteristics characteristics, Car_Controller controller)
        {
            //Debug.Log("Shifter");

            if (!characteristics.isGrounded()) return;
            //automatic
            if (engineRPM > maxRPM && gearNum < gears.Length - 1 && !reverse && checkGears(controller))
            {
                //Debug.Log("Gear ++");
                gearNum++;
                return;
            }
            if (engineRPM < minRPM && gearNum > 0)
            {
                //Debug.Log("Gear --");
                gearNum--;
            }

        }

        private void setEngineLerp(float num)
        {
            engineLerp = true;
            engineLerpValue = num;
        }

        public float lerpSmoothTime = 4;
        public void lerpEngine()
        {
            if (engineLerp)
            {
                engineRPM = Mathf.Lerp(engineRPM, engineLerpValue, lerpSmoothTime * Time.deltaTime);
                //engineRPM = Mathf.SmoothDamp(engineRPM,engineLerpValue,ref velocity, lerpSmoothTime * Time.deltaTime );
                engineLerp = engineRPM <= engineLerpValue + 100 ? false : true;
            }
        }
        #endregion
    }
}
