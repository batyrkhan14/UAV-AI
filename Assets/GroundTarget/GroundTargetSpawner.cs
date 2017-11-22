using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class GroundTargetSpawner : NetworkBehaviour
{
	public int team;
	public GameObject groundTargetPrefab;

	public override void OnStartServer ()
	{

		GameObject[] groundTargetSpawnPointObjects = GameObject.FindGameObjectsWithTag ("GroundTargetSpawn");
		foreach (GameObject obj in groundTargetSpawnPointObjects) {
			SpawnPoint sp = obj.GetComponent<SpawnPoint> ();
			GameObject groundTarget = Instantiate (groundTargetPrefab, sp.transform.position, sp.transform.rotation);
			groundTarget.GetComponent<GroundTarget> ().team = sp.team;
			NetworkServer.Spawn (groundTarget);
		}
	}
}
