using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {

	public GameObject playerExplosion;

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player")
		{
			Instantiate (playerExplosion, other.transform.position, other.transform.rotation);
		}
	}
}
