﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class Party
{
	public string name = "";
	public Color colour = Color.white;

	public string[] nameVariations;
	public string[] keywords;

	public int count = 0;

	public GameObject prefab;

	public Party(string name)
	{
		this.name = name;
	}

	public Party(string name, Color colour)
	{
		this.name = name;
		this.colour = colour;
	}

	public Party(string name, Color colour, string[] nameVariations)
	{
		this.name = name;
		this.colour = colour;
		this.nameVariations = nameVariations;
	}

	public Party(string name, Color colour, string[] nameVariations, string[] keywords)
	{
		this.name = name;
		this.colour = colour;
		this.nameVariations = nameVariations;
		this.keywords = keywords;
	}

	public Party(string name, Color colour, string[] nameVariations, string[] keywords, GameObject prefab)
	{
		this.name = name;
		this.colour = colour;
		this.nameVariations = nameVariations;
		this.keywords = keywords;

		this.prefab = prefab;
	}

	public Party(string name, Color colour, string[] nameVariations, string[] keywords, int count)
	{
		this.name = name;
		this.colour = colour;
		this.nameVariations = nameVariations;
		this.keywords = keywords;

		this.count = count;
	}
}
