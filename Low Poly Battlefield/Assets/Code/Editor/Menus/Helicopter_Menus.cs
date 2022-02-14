using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Helicopter
{
    public class Helicopter_Menus
    {
        [MenuItem("Vehicles/Setup New Helicopter")]
        public static void BuildNewHelicopter()
        {
            //Create a new Helicopter Setup
            GameObject curHeli = new GameObject("New_Helicopter", typeof(Heli_Controller));

            //Create the COF object for the Helicopter
            GameObject curCOG = new GameObject("COG");
            curCOG.transform.SetParent(curHeli.transform);

            //Assign the COG to the curHeli
            Heli_Controller curController = curHeli.GetComponent<Heli_Controller>();
            curController.cog = curCOG.transform;

            //Create Groups
            GameObject audioGRP = new GameObject("Audio_GRP");
            GameObject graphics_GRP = new GameObject("Graphics_GRP");
            GameObject collision_GRP = new GameObject("Collision_GRP");

            audioGRP.transform.SetParent(curHeli.transform);
            graphics_GRP.transform.SetParent(curHeli.transform);
            collision_GRP.transform.SetParent(curHeli.transform);

            //Select the new Helicopter
            Selection.activeObject = curHeli;
        }
    }
}