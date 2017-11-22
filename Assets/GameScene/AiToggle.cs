using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AiToggle : MonoBehaviour
{

	LocalPlayerFinder localPlayerFinder;
	public Toggle toggle;

	void Start ()
	{
		localPlayerFinder = GetComponent<LocalPlayerFinder> ();
	}

	void Update ()
	{
		if (localPlayerFinder.localPlayer) {
			localPlayerFinder.localPlayer.GetComponent<AI>().enabled = toggle.isOn;
		}
	}
}
