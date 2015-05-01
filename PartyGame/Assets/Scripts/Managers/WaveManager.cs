using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
	public GameManager gameManager;

	public System.DateTime lastScannedTweet = new System.DateTime (1970, 1, 1);
	public System.DateTime highestThisRunthough = new System.DateTime(1970, 1, 1);

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SpawnWave()
	{
		//This is where you specify the search term for the tweets you want. Result type can be 'new', 'popular' or 'mixed'.
		StartCoroutine (GetTweets(gameManager.twitterManager.CreateSearchUserWWW ("#GE2015", "new"))); //TODO: Add hashtag selection via PlayerPrefs and GUI.
		StartCoroutine (SearchForUserTweets ());
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
		gameManager.partyManager.PopulatePartiesWithCount(Twitter.ParseSearchResults (parseValue));
		gameManager.partyManager.SpawnMPs (gameManager.partyManager.partyRelatedTweets.ToArray ());
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
			
			foreach (Party party in gameManager.partyManager.parties)
			{
				if (foundTweets[i].text.ToUpper ().Contains(party.name.ToUpper ()))
					SpawnUserMP (party);
				
				if (party.nameVariations != null)
				{
					foreach (string s in party.nameVariations)
					{
						if (foundTweets[i].text.ToUpper ().Contains(s.ToUpper ()))
							SpawnUserMP (party);
					}
				}
				
				if (party.keywords != null)
				{
					foreach (string s in party.keywords)
					{
						if (foundTweets[i].text.ToUpper ().Contains(s.ToUpper ()))
							SpawnUserMP (party);
					}
				}
			}
		}
		
		lastScannedTweet = highestThisRunthough;
	}

	/// <summary>
	/// Spawns a stronger MP as this should be called as a result of someone using the hashtag to spawn an MP.
	/// </summary>
	/// <param name="party">Party.</param>
	private void SpawnUserMP(Party party)
	{
		GameObject mp = gameManager.partyManager.SpawnMP (party);

		mp.transform.localScale = new Vector3 (2, 2, 2);
		mp.GetComponent<MP_Control> ().Health += 50;
	}

	IEnumerator SearchForUserTweets()
	{
		while (true)
		{
			StartCoroutine (GetUserTweets(gameManager.twitterManager.CreateSearchUserWWW ("#TESTPARTY", "new")));
			yield return new WaitForSeconds(30);
		}
	}
}
