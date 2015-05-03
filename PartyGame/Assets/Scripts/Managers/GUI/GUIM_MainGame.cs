using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIM_MainGame : MonoBehaviour
{
	public Canvas mainGameCanvas;
	public GameObject partyScoringPrefab;

	private GameObject[] partyScores;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{

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
			panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(45, -45 + (i * -80));

			//Customise
			panel.transform.FindChild ("Image - Party Colour").GetComponent<Image>().color = parties[i].colour;
			panel.transform.FindChild ("Text - MPs").GetComponent<Text>().text = "0";
			panel.transform.FindChild ("Text - Wave Wins").GetComponent<Text>().text = "0";
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
