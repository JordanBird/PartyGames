using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
	public GameManager gameManager;

	public GameObject conservativePrefab;
	public GameObject greenPrefab;
	public GameObject labourPrefab;
	public GameObject libDemPrefab;
	public GameObject ukipPrefab;
	public GameObject snpPrefab;
	public GameObject plaidPrefab;

	public GameObject deathEffect;
	public GameObject[] hitEffects;

	public Party[] parties;

	public List<PartyTweetData> partyRelatedTweets = new List<PartyTweetData>();

	public Vector3 boundsCentre = Vector3.zero;
	public float maxBoundsDistace = 20;

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();

		Party conservative = new Party ("Conservative", new Color(0, 0.5294117647f, 0.86274509803f), null, new string[] { "David Cameron" }, conservativePrefab);
		Party green = new Party ("Green", new Color(0.41568627451f, 0.69019607843f, 0.13725490196f), null, new string[] { "Natalie Bennett" }, greenPrefab);
		Party labour = new Party ("Labour", new Color(0.86274509803f, 0.14117647058f, 0.12156862745f), null, new string[] { "Ed Miliband", }, labourPrefab);
		Party libDems = new Party ("LibDem", new Color(1, 0.49803921568f, 0), new string[] { "Lib Dems", "Lib Dem" }, new string[] { "Nick Clegg" }, libDemPrefab);
		Party ukip = new Party ("Ukip", new Color(0.50196078431f, 0, 0.50196078431f), new string[] { "UK Independance Party" }, new string[] { "Nigel Farage" }, ukipPrefab);
		Party snp = new Party ("SNP", new Color(1, 1, 0), new string[] { "Scottish National Party" }, new string[] { "Alex Salmond", "Nicola Sturgeon" }, snpPrefab);
		Party plaid = new Party ("Plaid", new Color(0, 0.50588235294f, 0.25882352941f), new string[] { "Plaid Cymru" }, new string[] { "Leanne Wood" }, plaidPrefab);

		//Populate parties array with created parties.
		parties = new Party[] { conservative, green, labour, libDems, ukip, snp, plaid };

		//Instantiate the GUI
		gameManager.guimMainGame.SetPartyScoringObjects (parties);
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	public void SpawnMPs()
	{
		for (int i = 0; i < partyRelatedTweets.Count; i++)
		{
			SpawnMP (partyRelatedTweets[i].party);
		}
	}

	public void PopulatePartiesWithCount(Tweet[] tweets)
	{
		partyRelatedTweets.Clear ();

		for (int i = 0; i < tweets.Length; i++)
		{
			foreach (Party party in parties)
			{
				if (tweets[i].text.ToUpper ().Contains(party.name.ToUpper ()))
					IncrementPartyCount(party, tweets[i]);
				
				if (party.nameVariations != null)
				{
					foreach (string s in party.nameVariations)
					{
						if (tweets[i].text.ToUpper ().Contains(s.ToUpper ()))
							IncrementPartyCount(party, tweets[i]);
					}
				}
				
				if (party.keywords != null)
				{
					foreach (string s in party.keywords)
					{
						if (tweets[i].text.ToUpper ().Contains(s.ToUpper ()))
							IncrementPartyCount(party, tweets[i]);
					}
				}
			}
		}
	}

	private void IncrementPartyCount(Party party, Tweet tweet)
	{
		party.count++;
		partyRelatedTweets.Add (new PartyTweetData (tweet, party));
	}

	public GameObject SpawnMP(Party party)
	{
		GameObject home = GameObject.Find(party.name + "Home");
		
		Vector3 position = Vector3.zero;
		
		if (home != null)
		{
			position = new Vector3(home.transform.position.x + Random.Range (-2f, 2f), home.transform.position.y - 2, home.transform.position.z + Random.Range (-2f, 2f));
		}
		
		GameObject mp = Instantiate (party.prefab, position, Quaternion.identity) as GameObject;
		mp.transform.parent = gameManager.dynamicObjectHolder.transform;

		party.mps.Add (mp);

		//Update GUI
		gameManager.guimMainGame.UpdateScoreCard (party);

		return mp;
	}

	public void RemoveMPFromParty(GameObject mp)
	{
		int partiesWithRemainingMPs = 0;

		foreach (Party p in parties)
		{
			p.mps.Remove (mp);

			if (p.mps.Count > 0)
				partiesWithRemainingMPs++;

			GameObject death = Instantiate (deathEffect, mp.transform.FindChild ("HeadTarget").transform.position, Quaternion.identity) as GameObject;
			death.transform.SetParent (gameManager.dynamicObjectHolder.transform);
			death.GetComponent<ParticleSystem>().startColor = GetParty (mp.GetComponent<MP_Control>().Party).colour;

			//Update GUI
			gameManager.guimMainGame.UpdateScoreCard (p);
		}

		if (partiesWithRemainingMPs < 2 && gameManager.waveManager.waveInProgress)
		{
			gameManager.waveManager.EndWave ();
			gameManager.waveManager.StartWave ();
		}
	}

	public void SpawnHitMarker(string party, Vector3 position)
	{
		foreach (Party p in parties)
		{
			if (party == p.name)
			{
				if (hitEffects.Length > 0)
				{
					GameObject g = Instantiate(hitEffects[Random.Range (0, hitEffects.Length)], position, Quaternion.identity) as GameObject;
					g.GetComponent<Renderer>().material.color = p.colour;
				}
			}
		}
	}

	public Party GetParty(string party)
	{
		foreach (Party p in parties)
		{
			if (p.name.ToUpper() == party.ToUpper ())
				return p;
		}

		return null;
	}

	public void ACTION_Order()
	{
		foreach (Party p in parties)
		{
			foreach (GameObject mp in p.mps)
			{
				mp.GetComponent<MP_Control>().Order ();
			}
		}
	}

	IEnumerator OutOfBoundsCheck()
	{
		while (true)
		{
			foreach (Party p in parties)
			{
				for (int i = 0; i < p.mps.Count; i++)
				{
					if (Vector3.Distance (p.mps[i].transform.position, boundsCentre) > maxBoundsDistace)
					{
						Debug.Log (p.mps[i] + " is out of bounds. Destroyed.");
						RemoveMPFromParty (p.mps[i]);
						i--;
					}
				}
			}

			yield return new WaitForSeconds(5);
		}
	}
}
