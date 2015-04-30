using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public TwitterManager twitterManager;

	public GameObject dynamicObjectHolder;

	public GameObject conservativePrefab;
	public GameObject greenPrefab;
	public GameObject labourPrefab;
	public GameObject libDemPrefab;
	public GameObject ukipPrefab;

	public List<PartyTweetData> partyRelatedTweets = new List<PartyTweetData>();
	public Party[] parties;

	public System.DateTime lastScannedTweet = new System.DateTime (1970, 1, 1);
	public System.DateTime highestThisRunthough = new System.DateTime(1970, 1, 1);

	// Use this for initialization
	void Start ()
	{
		//Find the Twitter Manager in the scene.
		twitterManager = FindObjectOfType<TwitterManager> ();

		dynamicObjectHolder = new GameObject ("Dynamic Object Holder");

		//Create parties to populate the array.
		Party conservative = new Party ("Conservative", Color.blue, null, new string[] { "David Cameron" }, conservativePrefab);
		Party green = new Party ("Green", Color.green, null, new string[] { "Natalie Bennett" }, greenPrefab);
		Party labour = new Party ("Labour", Color.red, null, new string[] { "Ed Miliband", }, labourPrefab);
		Party libDems = new Party ("LibDem", new Color(1, 0.49803921568f, 0), new string[] { "Lib Dems", "Lib Dem" }, new string[] { "Nick Clegg" }, libDemPrefab);
		Party ukip = new Party ("Ukip", new Color(0.50196078431f, 0, 0.50196078431f), new string[] { "UK Independance Party" }, new string[] { "Nigel Farage" }, ukipPrefab);

		//Populate parties array with created parties.
		parties = new Party[] { conservative, green, labour, libDems, ukip };

		//Jordan Says: Scraping here might be good for more data and key words: http://www.bbc.co.uk/news/election/2015/manifesto-guide

		//This is where you specify the search term for the tweets you want. Result type can be 'new', 'popular' or 'mixed'.
		StartCoroutine (GetTweets(twitterManager.CreateSearchUserWWW ("#GE2015", "new")));
		StartCoroutine (SearchForUserTweets ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	private IEnumerator GetTweets(WWW www)
	{
		yield return www;
		
		string parseValue = "";
		
		// check for errors
		if (www.error == null)
		{
			parseValue = www.text;
		}
		else
		{
			Debug.Log("WWW Error: "+ www.error);
		}
		
		//Success
		PopulatePartiesWithCount(Twitter.ParseSearchResults (parseValue));

		StartGame ();
	}

	public void SpawnMP(Party party)
	{
		GameObject home = GameObject.Find(party.name + "Home");
		
		Vector3 position = Vector3.zero;
		
		if (home != null)
		{
			position = new Vector3(home.transform.position.x + Random.Range (-2f, 2f), home.transform.position.y - 2, home.transform.position.z + Random.Range (-2f, 2f));
		}
		
		GameObject mp = Instantiate (party.prefab, position, Quaternion.identity) as GameObject;
		mp.transform.localScale = new Vector3 (2, 2, 2);
		mp.GetComponent<MP_Control> ().Health += 50;
	}

	private void PopulatePartiesWithCount(Tweet[] tweets)
	{
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

	private void SpawnMPs()
	{
		for (int i = 0; i < partyRelatedTweets.Count; i++)
		{
			GameObject home = GameObject.Find(partyRelatedTweets[i].party.name + "Home");

			Vector3 position = Vector3.zero;

			if (home != null)
			{
				position = new Vector3(home.transform.position.x + Random.Range (-2f, 2f), home.transform.position.y + Random.Range (-0.2f, 0.2f), home.transform.position.z + Random.Range (-2f, 2f));
			}

			GameObject mp = Instantiate (partyRelatedTweets[i].party.prefab, position, Quaternion.identity) as GameObject;
		}
	}

	private void StartGame()
	{
		SpawnMPs ();
	}

	IEnumerator SearchForUserTweets()
	{
		while (true)
		{
			StartCoroutine (GetUserTweets(twitterManager.CreateSearchUserWWW ("#TESTPARTY", "new")));
			yield return new WaitForSeconds(30);
		}
	}

	private IEnumerator GetUserTweets(WWW www)
	{
		yield return www;
		
		string parseValue = "";
		
		// check for errors
		if (www.error == null)
		{
			parseValue = www.text;
		}
		else
		{
			Debug.Log("WWW Error: "+ www.error);
		}
		
		//Success
		Tweet[] foundTweets = Twitter.ParseSearchResults (parseValue);

		highestThisRunthough = new System.DateTime (1970, 1, 1);

		for (int i = 0; i < foundTweets.Length; i++)
		{
			if (foundTweets[i].createdAt <= lastScannedTweet)
				continue;

			if (foundTweets[i].createdAt > highestThisRunthough)
				highestThisRunthough = foundTweets[i].createdAt;

			foreach (Party party in parties)
			{
				if (foundTweets[i].text.ToUpper ().Contains(party.name.ToUpper ()))
					SpawnMP (party);
				
				if (party.nameVariations != null)
				{
					foreach (string s in party.nameVariations)
					{
						if (foundTweets[i].text.ToUpper ().Contains(s.ToUpper ()))
							SpawnMP (party);
					}
				}
				
				if (party.keywords != null)
				{
					foreach (string s in party.keywords)
					{
						if (foundTweets[i].text.ToUpper ().Contains(s.ToUpper ()))
							SpawnMP (party);
					}
				}
			}
		}

		lastScannedTweet = highestThisRunthough;
	}
}
