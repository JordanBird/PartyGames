using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIM_MainGame : MonoBehaviour
{
	public Canvas mainGameCanvas;
	public GameObject partyScoringPrefab;
	public GameObject tickerItemPrefab;

	private GameObject[] partyScores;

	private string[] tickerItems;
	private List<GameObject> activeTickerItems = new List<GameObject>();

	private float tickerSpeed = 35f;
	private float newsTickerSpacing = 200;

	private int lastTickerItem = 0;

	// Use this for initialization
	void Start ()
	{
		string file = Resources.Load<TextAsset> ("Ticker Items").text;
		string[] lines = file.Split (new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);

		List<string> items = new List<string> ();

		for (int i = 0; i < lines.Length; i++)
		{
			if (!lines[i].StartsWith ("<!") && lines[i] != null && lines[i] != "")
				items.Add(lines[i]);
		}

		tickerItems = items.ToArray ();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void FixedUpdate()
	{
		float width = mainGameCanvas.pixelRect.width;
		float height = mainGameCanvas.pixelRect.height;

		while (activeTickerItems.Count < 5)
		{
			RectTransform tickerPanel = mainGameCanvas.transform.FindChild ("Panel - Ticker").GetComponent<RectTransform>();

			GameObject item = Instantiate (tickerItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			item.transform.SetParent (tickerPanel.transform, false);

			int index = Random.Range (0, tickerItems.Length);

			while (index == lastTickerItem && tickerItems.Length > 1)
			{
				index = Random.Range (0, tickerItems.Length);
			}

			lastTickerItem = index;

			item.GetComponent<Text>().text = tickerItems[index];

			Debug.Log(mainGameCanvas.pixelRect.ToString ());

			float itemWidth  = item.GetComponent<RectTransform>().rect.width;

			if (activeTickerItems.Count == 0)
			{
				item.GetComponent<RectTransform>().anchoredPosition = new Vector2((width / 2) + itemWidth / 2, 0);
			}
			else
			{
				RectTransform previousItem = activeTickerItems[activeTickerItems.Count - 1].GetComponent<RectTransform>();
				item.GetComponent<RectTransform>().anchoredPosition = new Vector2(previousItem.anchoredPosition.x + itemWidth + (newsTickerSpacing * activeTickerItems.Count), 0);
			}

			activeTickerItems.Add (item);
		}

		for (int i = 0; i < activeTickerItems.Count; i++)
		{
			if (i == 0)
				activeTickerItems[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(activeTickerItems[i].GetComponent<RectTransform>().anchoredPosition.x - (tickerSpeed * Time.deltaTime), activeTickerItems[i].GetComponent<RectTransform>().anchoredPosition.y);
			else
			{
				float xPosPrevious = activeTickerItems[i - 1].GetComponent<RectTransform>().anchoredPosition.x;
				float widthPrevious = activeTickerItems[i - 1].GetComponent<RectTransform>().rect.width / 2;
				float widthSelf = activeTickerItems[i].GetComponent<RectTransform>().rect.width / 2;
				float speedIncrease = (tickerSpeed * Time.deltaTime);
				float spacing = newsTickerSpacing;

				activeTickerItems[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosPrevious + widthPrevious + widthSelf + speedIncrease + spacing, activeTickerItems[i - 1].GetComponent<RectTransform>().anchoredPosition.y);
			}

			if (activeTickerItems[i].GetComponent<RectTransform>().anchoredPosition.x < -(width + activeTickerItems[i].GetComponent<RectTransform>().rect.width / 2))
			{
				Destroy (activeTickerItems[i]);
				activeTickerItems.RemoveAt (i);
				i--;
			}
		}
	}

	public void SetPartyScoringObjects(Party[] parties)
	{
		partyScores = new GameObject[parties.Length];

		for (int i = 0; i < parties.Length; i++)
		{
			//Spawn
			GameObject panel = Instantiate (partyScoringPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			panel.transform.SetParent (mainGameCanvas.transform, false);

			//Position
			panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(45, -45 + (i * -70));

			//Customise
			panel.transform.FindChild ("Image - Party Colour").GetComponent<Image>().color = parties[i].colour;
			panel.transform.FindChild ("Text - MPs").GetComponent<Text>().text = "0";
			panel.transform.FindChild ("Text - Wave Wins").GetComponent<Text>().text = "0";
			panel.transform.FindChild ("Text - Party").GetComponent<Text>().text = parties[i].name;
			panel.name = "Party Score: " + parties[i].name;

			partyScores[i] = panel;
		}
	}

	public void UpdateScoreCard(Party party)
	{
		foreach (GameObject g in partyScores)
		{
			if (g.name == "Party Score: " + party.name)
			{
				g.transform.FindChild ("Text - MPs").GetComponent<Text>().text = party.mps.Count.ToString ();
				g.transform.FindChild ("Text - Wave Wins").GetComponent<Text>().text = party.winCount.ToString ();

				return;
			}
		}
	}

	public void UpdateTimeLeft(string timeLeft)
	{
		mainGameCanvas.transform.FindChild ("Text - Wave Timer").GetComponent<Text> ().text = timeLeft + " Left";
	}

	public void UpdateWaveNumber(string wave)
	{
		mainGameCanvas.transform.FindChild ("Text - Wave #").GetComponent<Text> ().text = "Wave: " + wave;
	}
}
