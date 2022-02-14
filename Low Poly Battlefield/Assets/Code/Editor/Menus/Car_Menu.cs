using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Car
{
    public class Car_Menu
    {
        [MenuItem("Vehicles/Setup New Car")]
        public static void BuildNewHCar()
        {
            //Create a new Helicopter Setup
            GameObject curHeli = new GameObject("New_Car", typeof(Car_Controller));

            //Create the COF object for the Helicopter
            GameObject curCOG = new GameObject("COG");
            curCOG.transform.SetParent(curHeli.transform);

            //Assign the COG to the curHeli
            Car_Controller curController = curHeli.GetComponent<Car_Controller>();
            curController.cog = curCOG.transform;

            //Create Groups
            GameObject audioGRP = new GameObject("Audio_GRP");
            GameObject wheelCollidersGRP = new GameObject("WheelsCollider_GRP");
            GameObject graphics_GRP = new GameObject("Graphics_GRP");
            GameObject collision_GRP = new GameObject("Collision_GRP");
            GameObject lights_GRP = new GameObject("Lights_GRP");
            GameObject lookAt = new GameObject("LookAt");
            GameObject engine = new GameObject("Engine", typeof(Car_Engine));

            audioGRP.transform.SetParent(curHeli.transform);
            wheelCollidersGRP.transform.SetParent(curHeli.transform);
            graphics_GRP.transform.SetParent(curHeli.transform);
            collision_GRP.transform.SetParent(curHeli.transform);
            lights_GRP.transform.SetParent(curHeli.transform);
            lookAt.transform.SetParent(curHeli.transform);
            engine.transform.SetParent(curHeli.transform);

            curController.engine = engine.GetComponent<Car_Engine>();

            //Select the new Helicopter
            Selection.activeObject = curHeli;
        }
    }

}