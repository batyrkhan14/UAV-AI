using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeamSelect : NetworkBehaviour
{
	public void Spawn (int team)
	{
		SpawnMessage spawnMessage = new SpawnMessage();
		spawnMessage.team = team;
		ClientScene.AddPlayer (NetworkManager.singleton.client.connection, 0, spawnMessage);
	}
}

class SpawnMessage : MessageBase
{
	public int team;
}