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

	public GameObject deathEffect;
	public GameObject[] hitEffects;

	public Party[] parties;

	public List<PartyTweetData> partyRelatedTweets = new List<PartyTweetData>();

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();

		Party conservative = new Party ("Conservative", Color.blue, null, new string[] { "David Cameron" }, conservativePrefab);
		Party green = new Party ("Green", Color.green, null, new string[] { "Natalie Bennett" }, greenPrefab);
		Party labour = new Party ("Labour", Color.red, null, new string[] { "Ed Miliband", }, labourPrefab);
		Party libDems = new Party ("LibDem", new Color(1, 0.49803921568f, 0), new string[] { "Lib Dems", "Lib Dem" }, new string[] { "Nick Clegg" }, libDemPrefab);
		Party ukip = new Party ("Ukip", new Color(0.50196078431f, 0, 0.50196078431f), new string[] { "UK Independance Party" }, new string[] { "Nigel Farage" }, ukipPrefab);

		//Populate parties array with created parties.
		parties = new Party[] { conservative, green, labour, libDems, ukip };

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
}
