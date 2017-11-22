using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiLog : MonoBehaviour {

	public string debugText;

	void OnGUI()
	{
		GUI.Label (new Rect(10,10,400,400), debugText);
	}
}
