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

	void OnCollisionEnter(Collision other){
		if (other.transform.tag == "Fist" && other.gameObject.GetComponent<PunchForce>().Party != Party) {
//			Debug.Log (Party+other.gameObject.GetComponent<PunchForce>().Party);
				Parent.GetComponent<MP_Control> ().Punch(30);


		}
	}
}
