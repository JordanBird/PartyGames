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

	// Use this for initialization
	void Start ()
	{
		//Find the Twitter Manager in the scene.
		twitterManager = FindObjectOfType<TwitterManager> ();

		dynamicObjectHolder = new GameObject ("Dynamic Object Holder");

		//Create parties to populate the array.
		Party conservative = new Party ("Conservative", Color.blue, null, new string[] { "David Cameron" }, conservativePrefab);
		Party green = new Party ("Green", Color.green, null, new string[] { "Natalie Bennett" }, greenPrefab);
		Party labour = new Party ("Labour", Color.red, null, new string[] { "Ed Miliband" }, labourPrefab);
		Party libDems = new Party ("Liberal Democrats", new Color(1, 0.49803921568f, 0), new string[] { "Lib Dems", "Lib Dem" }, new string[] { "Nick Clegg" }, libDemPrefab);
		Party ukip = new Party ("UKIP", new Color(0.50196078431f, 0, 0.50196078431f), new string[] { "UK Independance Party" }, new string[] { "Nigel Farage" }, ukipPrefab);

		//Populate parties array with created parties.
		parties = new Party[] { conservative, green, labour, libDems, ukip };

		//Jordan Says: Scraping here might be good for more data and key words: http://www.bbc.co.uk/news/election/2015/manifesto-guide

		//This is where you specify the search term for the tweets you want. Result type can be 'new', 'popular' or 'mixed'.
		StartCoroutine (GetTweets(twitterManager.CreateSearchUserWWW ("#GE2015", "new")));
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
			GameObject mp = Instantiate (partyRelatedTweets[i].party.prefab, new Vector3(0, 0, i * 2), Quaternion.identity) as GameObject;
		}
	}

	private void StartGame()
	{
		SpawnMPs ();
	}
}
