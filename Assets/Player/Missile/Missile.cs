using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Missile : NetworkBehaviour
{
	public TargetSeeker targetSeeker;
	public float speed = 300;
	public float maxTurn = 2;
	public float lifespan = 20;
	public float engineDelay = 0.2f;
	public float colliderDelay = 0.2f;
	bool engineOn = false;

	Health health;

	Rigidbody body;

	void Start ()
	{
		body = GetComponent<Rigidbody> ();
		health = GetComponent<Health> ();

		if (isServer) {
			Invoke ("TurnEngineOn", engineDelay);
			Invoke ("TurnColliderOn", colliderDelay);
		}
	}

	void TurnEngineOn()
	{
		if (isServer) {
			engineOn = true;
			Invoke ("Die", lifespan);
		}
	}

	void TurnColliderOn()
	{
		if (isServer) {
			GetComponent<Collider> ().enabled = true;
		}
	}

	void FixedUpdate ()
	{
		if (isServer && engineOn) {
			body.velocity = (transform.forward * speed);
			if (targetSeeker) {
				if (targetSeeker.target) {
					Vector3 wishDir = (targetSeeker.target.position - transform.position);
					Quaternion wishRot = Quaternion.LookRotation (wishDir);
					float turn = Mathf.Min (maxTurn, Quaternion.Angle (transform.rotation, wishRot));
					body.MoveRotation (Quaternion.RotateTowards (transform.rotation, wishRot, turn));
				}
			}
			health.scrapVelocity = transform.forward * speed;
		}
	}

	void Die ()
	{
		if(isServer)
			health.Damage (health.GetHealth());
	}
}
