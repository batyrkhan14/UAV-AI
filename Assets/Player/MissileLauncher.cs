using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class MissileLauncher : NetworkBehaviour
{

	public GameObject missilePrefab;
	public float missilePushoutSpeed = 15;

	Transform missilePlaceholders;
	AeroplaneController controller;
	PlayerInput playerInput;
	PlayerSetup playerSetup;

	void Start ()
	{
		controller = GetComponent<AeroplaneController> ();
		playerInput = GetComponent<PlayerInput> ();
		playerSetup = GetComponent<PlayerSetup> ();
		missilePlaceholders = transform.Find ("MissilePlaceholders");
	}


	void Update ()
	{
		if (!isLocalPlayer)
			return;

		if (playerInput.fire) {
			if (missilePlaceholders.childCount > 0) {
				GameObject placeholder = missilePlaceholders.GetChild (0).gameObject;
				CmdLaunch (placeholder.transform.position, placeholder.transform.rotation);
			}
		}
	}


	[Command]
	void CmdLaunch(Vector3 position, Quaternion rotation)
	{
		if (missilePlaceholders.childCount > 0) {
			RpcDestroyMissilePlaceholder ();

			GameObject missile = Instantiate (missilePrefab, position, rotation);
			missile.GetComponent<TargetSeeker> ().team = playerSetup.team;
			missile.GetComponent<Rigidbody> ().velocity = transform.forward * controller.ForwardSpeed - transform.up * missilePushoutSpeed;
			NetworkServer.Spawn (missile);
		}
	}

	[ClientRpc]
	void RpcDestroyMissilePlaceholder()
	{
		GameObject placeholder = missilePlaceholders.GetChild (0).gameObject;
		Destroy (placeholder);
	}
}
