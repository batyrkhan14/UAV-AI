using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectGui : MonoBehaviour {

	LocalPlayerFinder localPlayerFinder;
	public GameObject teamSelectGui;

	void Start ()
	{
		localPlayerFinder = GetComponent<LocalPlayerFinder> ();
	}

	void Update ()
	{
		teamSelectGui.SetActive(localPlayerFinder.localPlayer == null);
	}
}
