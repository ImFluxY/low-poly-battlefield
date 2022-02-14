//Pixel Studios 2016
//Weapon Customisation v1.2

using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {


	void Update()
	{
		//Makes the UI buttons a billboard type of button
		transform.rotation = Camera.main.transform.rotation;
	}


}