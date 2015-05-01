using UnityEngine;
using System.Collections;

public class MP_Control : MonoBehaviour {
	
	public string Party;
	public float Health = 100;
	public float HitsPerSecond { get; private set; }
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
	{
		HitsPerSecond -= Time.deltaTime;
		if (HitsPerSecond < 0)
			HitsPerSecond = 0;
		
		if (Health <= 0)
			Die ();
		
	}
	
	void Die(){
		Destroy (this.gameObject);
		// Maybe put increment a total of what ever party just got killed.
		
	}
	
	public void Punch(float damage)
	{
		++HitsPerSecond;
		Health -= Mathf.Abs(damage);
	}
}
