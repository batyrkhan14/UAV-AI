
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarVisualizer : MonoBehaviour
{
	public float markDistance = 2f;

	public Mesh markMesh;
	public Material markMaterial;

	public Material lockOnMaterial;

	LocalPlayerFinder localPlayerFinder;

	void Start ()
	{
		localPlayerFinder = GetComponent<LocalPlayerFinder> ();
	}

	void Update ()
	{
		GameObject localPlayer = localPlayerFinder.localPlayer;
		if (localPlayer) {
			Radar radar = localPlayer.GetComponent<Radar> ();
			TargetSeeker targetSeeker = localPlayer.GetComponent<TargetSeeker> ();

			foreach (GameObject obj in radar.players) {
				if(obj)
					DrawMark (obj.transform.position, markDistance, markMesh, markMaterial);
			}
			foreach (GameObject obj in radar.groundTargets) {
				if(obj)
					DrawMark (obj.transform.position, markDistance, markMesh, markMaterial);
			}
			foreach (GameObject obj in radar.missiles) {
				if(obj)
					DrawMark (obj.transform.position, markDistance, markMesh, markMaterial);
			}


			if (targetSeeker.target) {
				DrawMark (targetSeeker.target.position, markDistance * 1.2f, markMesh, lockOnMaterial);
			}
		}
	}

	void DrawMark (Vector3 position, float radius, Mesh mesh, Material material)
	{
		Vector3 direction = (position - transform.position).normalized;
		Vector3 markPos = transform.position + direction * radius;
		Quaternion markRot = Quaternion.LookRotation (direction);

		Graphics.DrawMesh (mesh, markPos, markRot, material, 0, null, 0, null, false, false, false); 
	}
}

