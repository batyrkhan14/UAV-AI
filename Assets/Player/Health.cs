using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
	[SyncVar] [SerializeField] float health = 100;

	public float GetHealth ()
	{
		return health;
	}

	public float dyingTime = 0;

	public GameObject scrapsPrefab;
	public Material scrapMaterial;
	public Vector3 scrapVelocity;
	public float scrapLifetime = 20;
	public GameObject explosionPrefab;
	public float explosionObjectLifetime = 10;
	public GameObject sparksPrefab;

	ParticleSystem sparks;
	bool dying = false;

	void Start ()
	{
		if (sparksPrefab) {
			GameObject sparksObject = Instantiate (sparksPrefab, transform.position, transform.rotation);
			sparksObject.transform.parent = gameObject.transform;
			sparks = sparksObject.GetComponent<ParticleSystem> ();
		}
	}


	void OnCollisionEnter (Collision collision)
	{
		if (sparks) {
			for (int i = 0; i < collision.contacts.Length; ++i) {
				sparks.transform.position = collision.contacts [i].point;

				if(collision.relativeVelocity.magnitude > 0)
					sparks.transform.rotation = Quaternion.LookRotation (collision.relativeVelocity);

				sparks.Emit (10);
			}
		}

		if (isServer) {
			//if it is a damager it will call Damage() itself
			Damager damager = collision.gameObject.GetComponent<Damager> ();
			if (damager == null) {
				Damage (collision.relativeVelocity.magnitude * 1f);
			}
		}

	}

	// kill only through this method, otherwise you might get problems with "dying" flag
	public void Damage (float damagePoints)
	{
		if (!isServer || dying)
			return;

		health -= damagePoints;
		if (health <= 0) {
			//RpcDie ();
			Invoke ("Die", dyingTime);
			dying = true;
		}
	}

	void Die ()
	{
		RpcDie ();
	}

	[ClientRpc]
	void RpcDie ()
	{
		if (scrapsPrefab) {
			InstantiateScrap ();
		}

		if (explosionPrefab) {
			GameObject explosion = Instantiate (explosionPrefab, transform.position, transform.rotation);
			Destroy (explosion, explosionObjectLifetime);
		}

		if (isServer) {
			Destroy (gameObject);
		}
	}

	void InstantiateScrap ()
	{
		GameObject scraps = Instantiate (scrapsPrefab, transform.position, transform.rotation);
		foreach (Transform fragment in scraps.transform) {
			Rigidbody body = fragment.gameObject.AddComponent<Rigidbody> ();
			body.velocity = scrapVelocity;
			body.angularVelocity = Random.insideUnitSphere;
			body.drag = 0.3f;

			fragment.gameObject.AddComponent<BoxCollider> ();
			fragment.gameObject.layer = LayerMask.NameToLayer ("Scrap");

			if (scrapMaterial) {
				MeshRenderer meshRenderer = fragment.gameObject.GetComponent<MeshRenderer> ();
				meshRenderer.material = scrapMaterial;
			}
		}
		Destroy (scraps, scrapLifetime);
	}
}
