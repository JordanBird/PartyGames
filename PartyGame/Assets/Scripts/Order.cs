using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Order : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	 if (Input.GetKeyDown (KeyCode.Space)) {
			GetComponent<Text>().enabled = true;
		}

	if (Input.GetKeyUp (KeyCode.Space)) {
			GetComponent<Text>().enabled = false;
		}
	}
}
