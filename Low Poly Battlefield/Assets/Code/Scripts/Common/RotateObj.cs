using UnityEngine;
using System.Collections;

public class RotateObj : MonoBehaviour {

	[Header("Rotation speed on the X axis")]
	//How fast you're rotating the gun on the X axis
	public float RotationXSpeed = 200;

	[Header("Rotation speed on the Y axis")]
	//How fast you're rotating the gun on the Y axis
	public float RotationYSpeed = 50;

	//Rotation Mode
	[System.Serializable]
	public enum rotateModes
	{
		X,
		Y,
		XY
	}

	[Header("Rotation Mode")]
	public rotateModes weaponRotationModes;

	// Update is called once per frame
	void Update () 
	{
		//Checks if you can only rotate on the X axis
		if (weaponRotationModes == rotateModes.X)
		{
			if(Input.GetMouseButton(1))
			{
				transform.Rotate(0, -(Input.GetAxis("Mouse X") * RotationXSpeed * Time.deltaTime), 0, Space.World);
			}
		}

		//Checks if you can only rotate on the Y axis
		if (weaponRotationModes == rotateModes.Y)
		{
			if(Input.GetMouseButton(1))
			{
				transform.Rotate((Input.GetAxis("Mouse Y") * RotationYSpeed * Time.deltaTime), 0, 0, Space.World);
			}
		}

		//Checks if you can rotate on both X and Y axis
		if (weaponRotationModes == rotateModes.XY)
		{
			if (Input.GetMouseButton(1))
			{
				transform.Rotate((Input.GetAxis("Mouse Y") * RotationYSpeed * Time.deltaTime), -(Input.GetAxis("Mouse X") * RotationXSpeed * Time.deltaTime), 0, Space.World);
			}
		}
	}
}
