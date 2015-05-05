using UnityEngine;
using System.Collections;

public class MP_Control : MonoBehaviour
{
	public string Party;
	private float Health = 100;
	public float HitsPerSecond { get; set; }

	private PunchForce[] punchForces;

	private Vector3 oldScale = Vector3.one;

	// Use this for initialization
	void Start ()
	{
		punchForces = GetComponentsInChildren<PunchForce> ();

		StartCoroutine (ScaleSpawn ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		HitsPerSecond -= Time.deltaTime;
		if (HitsPerSecond < 0)
			HitsPerSecond = 0;
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
		try
		{
			foreach (PunchForce pf in punchForces)
			{
				pf.Order();
			}
		}
		catch {}
	}

	public void AddForce(Vector3 force)
	{
		foreach (PunchForce pf in punchForces)
		{
			pf.AddForce (force);
		}
	}

	IEnumerator ScaleSpawn()
	{
		oldScale = transform.localScale;
		transform.localScale = Vector3.one * 0.01f;

		Vector3 jump = oldScale * 0.1f;

		float progress = 0;

		while (transform.localScale != oldScale + jump)
		{
			progress += 0.1f;
			transform.localScale = Vector3.Lerp(transform.localScale, oldScale + jump, progress);

			yield return new WaitForSeconds(0.01f);
		}

		progress = 0;
		while (transform.localScale != oldScale)
		{
			progress += 0.1f;
			transform.localScale = Vector3.Lerp(transform.localScale, oldScale, progress);
			
			yield return new WaitForSeconds(0.01f);
		}

		foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
		{
			rb.isKinematic = false;
		}
	}
}
