//Pixel Studios 2016
//Weapon Customisation v1.2

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSelection : MonoBehaviour {

	//List of all weapons you can choose from
	[Header("All your customisable weapons")]
	public List<GameObject> Weapons = new List<GameObject>();

	//Checks the current weapon selected
	private int currentWeapon;

	//Checks if the weapon selecting GUI should show
	private bool isSelecting;

	//An int variable to check if you have reached the limit of the weapons list
	private int limitCheck;

	//A bool to either hide or show the "Next" button
	private bool showNext;

	//A bool to either hide or show the "Previous" button
	private bool showPrevious;

	// Use this for initialization
	void Start () {

		changeWeapon();
	
	}
	
	// Update is called once per frame
	void Update () {

		//Handles the GUI next and previous buttons
		if(isSelecting)
		{
			limitCheck = Weapons.Count;
			limitCheck -= 1;

			if(currentWeapon <= 0)
			{
				currentWeapon = 0;
				showPrevious = false;
			}
			else
			{
				showPrevious = true;
			}

			if(currentWeapon == limitCheck)
			{
				showNext = false;
			}
			else
			{
				showNext = true;
			}
		}
	
	}

	void OnGUI()
	{
		//Checks if you have pressed the button
		if(!isSelecting)
		{
			if(GUI.Button(new Rect(250, 0, 200, 50), "Weapons"))
			{
				isSelecting = true;
			}
		}
		else
		{
			//Hides the weapon selection menu
			if(GUI.Button(new Rect(250, 0, 200, 50), "Back"))
			{
				isSelecting = false;
			}

			//Displays current weapon's name
			GUI.Box(new Rect(250, 100, 200, 100), Weapons[currentWeapon].name);


			if(showPrevious)
			{
				if(GUI.Button(new Rect(250, 200, 50, 50), "<"))
				{
					currentWeapon -= 1;
					changeWeapon();
				}
			}

			if(showNext)
			{
				if(GUI.Button(new Rect(400, 200, 50, 50), ">"))
				{
					currentWeapon += 1;
					changeWeapon();
				}
			}
		}
	}


	//Changes the actual weapon object
	void changeWeapon()
	{
		foreach(GameObject weapons in Weapons)
		{
			weapons.SetActive(false);
		}

		Weapons[currentWeapon].SetActive(true);
	}
}
