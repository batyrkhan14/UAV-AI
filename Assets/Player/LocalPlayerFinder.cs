using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerFinder : MonoBehaviour {
	public GameObject localPlayer { get; private set; }

	void Update ()
	{
		if (localPlayer == null) {
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
			for (int i = 0; i < players.Length; ++i) {
				PlayerSetup playerSetup = players [i].GetComponent<PlayerSetup> ();
				if (playerSetup.isLocalPlayer) {
					localPlayer = players [i];
					break;
				}
			}
		}
	}
}
