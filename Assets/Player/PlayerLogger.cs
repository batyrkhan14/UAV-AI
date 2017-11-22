using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.IO;

public class PlayerLogger : MonoBehaviour
{

	Transform playerTransform;
	PlayerSetup playerSetup;
	PlayerInput playerInput;
	StreamWriter file;

	public float logPeriod = 0.1f;
	float nextLog = 0;

	void Start ()
	{
		playerTransform = GetComponent<Transform> ();
		playerSetup = GetComponent<PlayerSetup> ();
		playerInput = GetComponent<PlayerInput> ();

		string logDirectory = "logs/" + NetworkManager.singleton.client.connection.hostId;
		Directory.CreateDirectory (logDirectory);

		string fileName = "player" + playerSetup.netId + "_log.txt";
		file = new StreamWriter (Path.Combine (logDirectory, fileName), false);
	}

	void OnDestroy ()
	{
		if(file != null)
			file.Close ();
	}

	void Update ()
	{
		if (file == null)
			return;

		if (Time.time > nextLog) {
			string info = string.Format ("{0}s P{1} {2} ", Time.time, playerSetup.netId, playerTransform.position);

			if (playerSetup.isLocalPlayer) {
				info += string.Format ("(R{0} P{1} B{2} F{3}) ",
					playerInput.roll,
					playerInput.pitch,
					playerInput.toggleBrakes,
					playerInput.fire);
			}

			file.WriteLine (info);
			file.Flush ();

			nextLog = Time.time + logPeriod; 
		}
	}
}
