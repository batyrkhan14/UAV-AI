using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public float roll { get; private set; }
	public float pitch { get; private set; }
	public float yaw { get; private set; }
	public bool fire { get; private set; }
	public bool toggleBrakes { get; private set; }

	public float inputRoll;
	public float inputPitch;
	public float inputYaw;
	public bool inputFire;
	public bool inputToggleBrakes;

	void Update ()
	{
		roll = 0;
		pitch = 0;
		yaw = 0;
		fire = false;
		toggleBrakes = false;

		roll = roll + Input.GetAxis ("Horizontal");
		pitch = pitch + Input.GetAxis ("Vertical");
		fire = fire || Input.GetButtonDown ("Fire");
		toggleBrakes = toggleBrakes || Input.GetButtonDown ("Toggle Brakes");
 		
		roll = roll + inputRoll;
		pitch = pitch + inputPitch;
		yaw = yaw + inputYaw;
		fire = fire || inputFire;
		toggleBrakes = toggleBrakes || inputToggleBrakes;

		inputRoll = 0;
		inputPitch = 0;
		inputYaw = 0;
		inputFire = false;
		inputToggleBrakes = false;

		roll = Mathf.Clamp (roll, -1, 1);
		pitch = Mathf.Clamp (pitch, -1, 1);
		yaw = Mathf.Clamp (yaw, -1, 1);
	}
}
