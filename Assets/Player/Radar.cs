using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
	//some elements in following lists can be null
	public List<GameObject> players = new List<GameObject> ();
	public List<GameObject> groundTargets;
	public List<GameObject> missiles = new List<GameObject> ();
	public float maxDistance = 2000;


	void Start ()
	{
		groundTargets = new List<GameObject>(GameObject.FindGameObjectsWithTag ("GroundTarget"));
	}

	void Update ()
	{
		if (Time.frameCount % 6 == 0) {

			GameObject[] allplayers = GameObject.FindGameObjectsWithTag ("Player");
			players.Clear ();
			foreach (GameObject player in allplayers) {
				PlayerSetup playerSetup = player.GetComponent<PlayerSetup> ();
				if (!playerSetup.isLocalPlayer
				    && Vector3.Distance (transform.position, player.transform.position) < maxDistance) {
					players.Add (player);
				}
			}
		}

		if (Time.frameCount % 2 == 0) {
			GameObject[] allmissiles = GameObject.FindGameObjectsWithTag ("Missile");
			missiles.Clear ();
			foreach (GameObject missile in allmissiles) {
				if (Vector3.Distance (transform.position, missile.transform.position) < maxDistance) {
					missiles.Add (missile);
				}
			}
		}
	}
}
