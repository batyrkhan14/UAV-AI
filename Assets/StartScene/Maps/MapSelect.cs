using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelect : MonoBehaviour
{
	static MapSelect instance;
	void Awake ()
	{
		if (instance == null) {
			DontDestroyOnLoad (gameObject);
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}


	public GameObject[] mapPrefabs;

	public int currentIndex { get; private set; }
	public void ChangeMap (int index)
	{
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}
		Instantiate (mapPrefabs [index], transform);
		currentIndex = index;
	}
}
