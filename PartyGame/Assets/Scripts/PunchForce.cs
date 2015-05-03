using UnityEngine;
using System.Collections;


public class PunchForce : MonoBehaviour {

	[SerializeField]GameObject Target;
	Vector3 Dir;
	public GameObject Home;
	float Dis;
	Vector3 Head;
	Rigidbody Rb;
	MP_Control mp;
	float PunchDelay = 0;
	public string Party = "";
	public string TarParty;

	// Use this for initialization
	void Start () {
		Rb = GetComponent<Rigidbody> ();
		mp = GetComponentInParent<MP_Control> ();

		Home = GameObject.Find (mp.Party + "Home");
	}
	
	// Update is called once per frame
	void Update () {
	
		Rb.AddForce (Vector3.up * 50);
		PunchDelay += 1 * Time.deltaTime;
	
		if (PunchDelay >= Random.Range(1.0f,6.0f)) {
			//This switch will sometimes cuase it to target it's self I left it in since it makes them do a jump that amuses me.
			switch(Random.Range (0,5)){
				case 0:
				TarParty = "Conservative";
				break;
				case 1:
				TarParty = "Labour";
				break;
				case 2:
				TarParty = "Green";
				break;
				case 3:
				TarParty = "LibDem";
				break;
				case 4:
				TarParty = "Ukip";
				break;

			}
			GameObject[] MPList;

			MPList = GameObject.FindGameObjectsWithTag(TarParty);

			if (MPList.Length > 0)
			{
				Target = MPList[Random.Range(0,MPList.Length)];
				foreach (Transform child in Target.GetComponentInChildren<Transform>()) {
					if (child.tag == "Target"){
						Target = child.gameObject;}
				} 
				//Head = Target.transform.position - transform.position;
				//Dis = Target.transform.position.magnitude;
				//Dir = Head / Dis;
				//Rb.AddForce (Dir * 50500);
				Rb.AddForce((Target.transform.position - transform.position).normalized * 20000);
				PunchDelay = 0;
			}
		}

//		if (Input.GetKeyDown (KeyCode.Space)) {
//
//			//Head = Home.transform.position - transform.position;
//			//Dis = Home.transform.position.magnitude;
//			//Dir = Head / Dis;
//			//Rb.AddForce (Dir * 50500);
//			Rb.AddForce((Home.transform.position - transform.position).normalized * 30000);
//		}
	}

	public void Order()
	{
		Rb.AddForce((Home.transform.position - transform.position).normalized * 30000);
	}
}
