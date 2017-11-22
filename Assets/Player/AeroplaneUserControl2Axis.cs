using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent (typeof(AeroplaneController))]
public class AeroplaneUserControl2Axis : MonoBehaviour
{
	AeroplaneController m_Aeroplane;

	PlayerInput playerInput;

	bool brakesInput = true;

	private void Awake ()
	{
		m_Aeroplane = GetComponent<AeroplaneController> ();
		playerInput = GetComponent<PlayerInput> ();
	}


	void Update()
	{
		if (playerInput.toggleBrakes)
			brakesInput = !brakesInput;
	}

	private void FixedUpdate ()
	{
		float throttle = brakesInput ? -1 : 1;

		m_Aeroplane.Move (playerInput.roll, playerInput.pitch, playerInput.yaw, throttle, brakesInput);
	}
}
