using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectGui : MonoBehaviour 
{
	MapSelect mapselect;
	public Dropdown dropdown;

	void Start ()
	{
		mapselect = GameObject.Find ("MapSelect").GetComponent<MapSelect> ();

		dropdown.onValueChanged.AddListener (OnDropdownValueChanged);

		dropdown.ClearOptions ();
		List<string> levelNames = new List<string> ();
		foreach (GameObject levelPrefab in mapselect.mapPrefabs) {
			levelNames.Add (levelPrefab.name);
		}
		dropdown.AddOptions (levelNames);

		mapselect.ChangeMap (dropdown.value);
	}

	public void OnDropdownValueChanged (int value)
	{
		mapselect.ChangeMap (value);
	}
}
