using UnityEngine;
using System.Collections;

public class DetailSettings
{
	public int Intensity;
	public bool IsUsed;
	public string DetailName;
	public int Index;
	public bool Rare;
	public int ChanceToFind;
	public bool FluentCoverage;
	public int Smoothness;
	public float AlphamapValue;
	
	public float MinHeight;
	public float MaxHeight;
	public int MinimumHeight;
	public int MaximumHeight;
	
	public DetailSettings()
	{
		Intensity = 1;
		AlphamapValue = 0.8f;
		Smoothness = 4;
		MinHeight = 0;
		MaxHeight = 0;
	}
}
