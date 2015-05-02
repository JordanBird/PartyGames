using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
	public GameManager gameManager;

	public System.DateTime lastScannedTweet = new System.DateTime (1970, 1, 1);
	public System.DateTime highestThisRunthough = new System.DateTime(1970, 1, 1);

	public float timeLimit = 240;
	public float timeLeft = 0;
	public int wave = 0;

	public bool waveInProgress = false;

	// Use this for initialization
	void Start ()
	{
		gameManager = FindObjectOfType<GameManager> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (waveInProgress)
		{
			timeLeft -= Time.deltaTime;
			
			if (timeLeft <= 0)
			{
				Timeout ();
				StartWave ();
			}
		}
	}

	public void StartWave()
	{
		//This is where you specify the search term for the tweets you want. Result type can be 'new', 'popular' or 'mixed'.
		StartCoroutine (GetTweets(gameManager.twitterManager.CreateSearchUserWWW ("#GE2015", "new"))); //TODO: Add hashtag selection via PlayerPrefs and GUI.
		StartCoroutine (SearchForUserTweets ());
	}

	public void SpawnWave(string tweets)
	{
		wave++;
		timeLeft = timeLimit;
		waveInProgress = true;

		gameManager.partyManager.PopulatePartiesWithCount(Twitter.ParseSearchResults (tweets));
		gameManager.partyManager.SpawnMPs ();

		StartCoroutine (DEBUG_SimulateUserTweets ());
	}

	public void EndWave()
	{
		StopCoroutine (SearchForUserTweets());
		StopCoroutine (DEBUG_SimulateUserTweets());

		waveInProgress = false;

		foreach (Party p in gameManager.partyManager.parties)
		{
			if (p.mps.Count > 0)
			{
				p.winCount++;

				foreach (GameObject g in p.mps)
				{
					Destroy (g);
				}

				p.mps.Clear ();

				//Update GUI
				gameManager.guimMainGame.UpdateScoreCard (p);
			}
		}
	}

	public void Timeout()
	{
		foreach (Party p in gameManager.partyManager.parties)
		{
			if (p.mps.Count > 0)
			{
				foreach (GameObject g in p.mps)
				{
					Destroy (g);
				}
				
				p.mps.Clear ();
			}
		}
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
		SpawnWave (parseValue);
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
		mp.GetComponent<MP_Control> ().SetHealth (150);

		//Update GUI
		gameManager.guimMainGame.UpdateScoreCard (party);
	}

	IEnumerator SearchForUserTweets()
	{
		while (waveInProgress)
		{
			StartCoroutine (GetUserTweets(gameManager.twitterManager.CreateSearchUserWWW ("#TESTPARTY", "new")));
			yield return new WaitForSeconds(30);
		}
	}

	public IEnumerator DEBUG_SimulateUserTweets()
	{
		while (waveInProgress)
		{
			if (gameManager.partyManager.parties.Length > 0)
			{
				for (int i = 0; i < Random.Range (0, 5); i++)
				{
					Party party = gameManager.partyManager.parties [Random.Range (0, gameManager.partyManager.parties.Length)];
					
					SpawnUserMP (party);
				}
			}

			yield return new WaitForSeconds(30);
		}

	}
}
