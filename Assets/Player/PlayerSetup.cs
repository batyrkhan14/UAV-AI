using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
	Rigidbody rigidbodyComponent;
	AeroplaneController controller;
	Health health;

	[SyncVar]
	int _team;
	public int team {
		get {
			return _team;
		}
		set { 
			_team = value;
			GetComponent<TargetSeeker> ().team = _team;
		}
	}

	public void Start ()
	{
		rigidbodyComponent = GetComponent<Rigidbody> ();
		health = GetComponent<Health> ();
		controller = GetComponent<AeroplaneController> ();
		if (isLocalPlayer) {
			GetComponent<TargetSeeker> ().team = team;
		} else {
			GetComponent<AeroplaneUserControl2Axis> ().enabled = false;
			GetComponent<Radar> ().enabled = false;
			GetComponent<TargetSeeker> ().enabled = false;
		}
	}


	void Update ()
	{
		health.scrapVelocity = transform.forward * controller.ForwardSpeed;
		//Debug.Log (1.0f/Time.deltaTime);
		if (isLocalPlayer && !isServer) {
			//CmdPrintTransformDiff (transform.position, transform.rotation, rigidbodyComponent.velocity);
		}
	}


	[Command (channel = 1)]
	void CmdPrintTransformDiff (Vector3 pos, Quaternion rot, Vector3 vel)
	{
		float posdiff = (pos - transform.position).magnitude;
		float rotdiff = Quaternion.Angle (rot, transform.rotation);
		float veldiff = (vel - rigidbodyComponent.velocity).magnitude;
		Debug.Log (posdiff + " " + rotdiff + " " + veldiff);
	}
}
