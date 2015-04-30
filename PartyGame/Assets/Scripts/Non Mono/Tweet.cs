using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tweet
{
	public string ID = "";
	public string text = "";

	public int favouritesCount = 0;
	public int retweetCount = 0;

	public TwitterUser user;
	
	public Tweet(string iID, string iText, TwitterUser iUser)
	{
		ID = iID;
		text = iText;
		user = iUser;
	}

	public Tweet(string iID, string iText, TwitterUser iUser, int favouritesCount, int retweetCount)
	{
		ID = iID;
		text = iText;
		user = iUser;

		this.favouritesCount = favouritesCount;
		this.retweetCount = retweetCount;
	}
}
