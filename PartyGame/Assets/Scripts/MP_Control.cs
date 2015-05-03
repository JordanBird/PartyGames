﻿using UnityEngine;
using System.Collections;

public class MP_Control : MonoBehaviour
{
	public string Party;
	private float Health = 100;

	private PunchForce[] punchForces;

	// Use this for initialization
	void Start ()
	{
		punchForces = GetComponentsInChildren<PunchForce> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void DealDamage(float amount)
	{
		Health -= amount;

		if (Health <= 0)
			Dead ();
	}

	public float GetHealth()
	{
		return Health;
	}

	public void SetHealth(float value)
	{
		Health = value;
	}

	public void Dead()
	{
		FindObjectOfType<GameManager> ().partyManager.RemoveMPFromParty (gameObject); //Search rather than cache to save space.

		Destroy (this.gameObject);
		// Maybe put increment a total of what ever party just got killed.
	}

	public void Order()
	{
		foreach (PunchForce pf in punchForces)
		{
			pf.Order();
		}
	}
}
