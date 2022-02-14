using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Seperate : MonoBehaviour {

	[System.Serializable]
	public class customisable
	{
		//What position your holder will lerp to when seperated
		public Vector3 customisable_Position;

		//Your holder for all your attachments
		public Transform customisable_Transform;

		[HideInInspector]
		public Vector3 original_Position;
	}

	public customisable customisable_Part;

	//Checks if the weapon is seperated
	private bool seperated;

	//Sound that plays when you press "Seperate"
	public AudioClip seperate_Sound;

	//A list of your UI buttons which controls which part of your weapon you're customising
	public List<GameObject> UI_Buttons = new List<GameObject>();

	//A list of how many attachment holders you have
	public List<customisable> cus_Parts = new List<customisable>();

	// Use this for initialization
	void Start () {

		//Assigns the attachments holders original position
		foreach(customisable parts in cus_Parts)
		{
			parts.original_Position = parts.customisable_Transform.localPosition;
		}

		//Deactivates the UI buttons
		foreach(GameObject buttons in UI_Buttons)
		{
			buttons.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {

		//Lerps the attachment holders if the gun is seperated or not
		foreach(customisable parts in cus_Parts)
		{
			if(seperated)
			{
				parts.customisable_Transform.localPosition = Vector3.Lerp(parts.customisable_Transform.localPosition, parts.customisable_Position, 10 * Time.deltaTime);
			}
			else
			{
				parts.customisable_Transform.localPosition = Vector3.Lerp(parts.customisable_Transform.localPosition, parts.original_Position, 10 * Time.deltaTime);
			}
		}
	}


	void OnGUI()
	{
		//Checks if the gun is seperated, to show either "Seperate" or "Unite" button
		if(!seperated)
		{
			if(GUI.Button(new Rect(0,0,200,50), "Seperate"))
			{
				seperated = true;

				foreach(GameObject buttons in UI_Buttons)
				{
					buttons.SetActive(true);
				}

				//Plays the seperate/unite sound
				GetComponent<AudioSource>().PlayOneShot(seperate_Sound);
			}
		}

		if(seperated)
		{
			if(GUI.Button(new Rect(0,0,200,50), "Unite"))
			{
				seperated = false;

				foreach(GameObject buttons in UI_Buttons)
				{
					buttons.SetActive(false);
				}

				//Plays the seperate/unite sound
				GetComponent<AudioSource>().PlayOneShot(seperate_Sound);
			}
		}
	}
}
