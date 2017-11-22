using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour {

	public float damagePoints = 1;


	void OnCollisionEnter(Collision collision)
	{
		Health health = collision.gameObject.GetComponent<Health> ();
		if (health) {
			health.Damage (damagePoints);
		}

		//Just in case health.OnCollisionEnter doesn't fire
		Health myhealth = GetComponent<Health> ();
		myhealth.Damage (myhealth.GetHealth());
	}
}
