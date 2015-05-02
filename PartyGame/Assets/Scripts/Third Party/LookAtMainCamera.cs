using UnityEngine;
using System.Collections;

public class LookAtMainCamera : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (Look ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	IEnumerator Look()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.001f);

			transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
		}
	}
}
