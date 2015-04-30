using UnityEngine;
using System.Collections;

public class MP_Control : MonoBehaviour {

	public string Party;
	public float Health = 100;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Health <= 0)
			Dead ();

	}

	void Dead(){
		Destroy (this.gameObject);
		// Maybe put increment a total of what ever party just got killed.

	}
}
