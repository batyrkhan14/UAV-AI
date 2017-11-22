using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

	Transform target;
	LocalPlayerFinder localPlayerFinder;

	void Start ()
	{
		localPlayerFinder = GetComponent<LocalPlayerFinder> ();
	}

	void Update ()
	{
		if (target) {
			transform.position = target.position;
			transform.rotation = target.rotation;
		} else if (localPlayerFinder.localPlayer) {
			target = localPlayerFinder.localPlayer.transform.Find ("CameraTarget");
		}
	}
}
