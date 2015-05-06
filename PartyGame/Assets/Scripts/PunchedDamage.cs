using UnityEngine;
using System.Collections;

public class PunchedDamage : MonoBehaviour {

	public GameObject Parent;
	public string Party;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if (other.transform.tag == "Fist" && other.gameObject.GetComponent<PunchForce>().Party != Party)
		{
			//Debug.Log (Party+other.gameObject.GetComponent<PunchForce>().Party);
			Parent.GetComponent<MP_Control> ().DealDamage (Random.Range (1, 4));
			Parent.GetComponent<MP_Control> ().HitsPerSecond++;

			//Spawn a hit marker.
			FindObjectOfType<PartyManager>().SpawnHitMarker (Party, transform.position);
		}
	}
}
