using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {

	public GameObject playerExplosion;

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player")
		{
			Instantiate (playerExplosion, gameObject.transform.position, gameObject.transform.rotation);
			Destroy (gameObject);
		}
	}
}
