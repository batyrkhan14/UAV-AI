using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class CustomNetworkManager : NetworkManager
{
	Dictionary<int, List<SpawnPoint>> spawnPoints = new Dictionary<int, List<SpawnPoint>> ();
	Dictionary<int, int> roundRobin = new Dictionary<int, int> ();

	bool isServer;

	public override void OnStartServer ()
	{
		isServer = true;

		spawnPoints.Clear ();
		roundRobin.Clear ();
		GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag ("Respawn");
		Debug.Log (spawnPointObjects.Length + " spawn points found");
		foreach(GameObject obj in spawnPointObjects) {
			SpawnPoint sp = obj.GetComponent<SpawnPoint> ();
			if (!spawnPoints.ContainsKey (sp.team)) {
				spawnPoints.Add (sp.team, new List<SpawnPoint> ());
				roundRobin.Add (sp.team, 0);
			}

			spawnPoints [sp.team].Add (sp);
		}
	}


	const int MapChangeMessage = MsgType.Highest + 1;
	public override void OnStartClient (NetworkClient client)
	{
		client.RegisterHandler (MapChangeMessage, OnMapChangeMessage);
	}
		
	public override void OnServerConnect (NetworkConnection conn)
	{
		MapSelect mapselect = GameObject.Find ("MapSelect").GetComponent<MapSelect> ();
		IntegerMessage msg = new IntegerMessage (mapselect.currentIndex);
		conn.Send (MapChangeMessage, msg);
	}

	void OnMapChangeMessage(NetworkMessage netmsg)
	{
		if (!isServer) {
			MapSelect mapselect = GameObject.Find ("MapSelect").GetComponent<MapSelect> ();
			IntegerMessage msg = netmsg.ReadMessage<IntegerMessage> ();
			mapselect.ChangeMap (msg.value);
		}
	}


	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
	{
		if (extraMessageReader == null)
			return;

		SpawnMessage spawnMessage = extraMessageReader.ReadMessage<SpawnMessage> ();
		int team = spawnMessage.team;

		if (!spawnPoints.ContainsKey (team))
			return;

		SpawnPoint sp = spawnPoints [team] [roundRobin [team]];
		roundRobin [team] += 1;
		if (roundRobin [team] >= spawnPoints [team].Count)
			roundRobin [team] = 0;

		GameObject player = Instantiate (playerPrefab, sp.transform.position, sp.transform.rotation);
		player.GetComponent<PlayerSetup> ().team = team;
		NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
	}
}
