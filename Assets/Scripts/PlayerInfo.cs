using System;
using UnityEngine;
using InControl;

/// <summary>
/// Data container to represent player configuration 
/// </summary>
public class PlayerInfo {
	public int PlayerNum;
	public InputDevice Controller;
	public ShoeType ShoeLeft, ShoeRight;
	public Team Team;
	public Costume Costume;

	[NonSerialized] public Player Instance;
	
	[NonSerialized] public int Kills;
	[NonSerialized] public int Deaths;
	[NonSerialized] public float DamageDealt;
	[NonSerialized] public float DamageReceived;
}

[Serializable]
public class Team
{
	public Color Color;
	public String Name;
	
	[NonSerialized] public int Kills;
	[NonSerialized] public int Deaths;
	[NonSerialized] public float DamageDealt;
	[NonSerialized] public float DamageReceived;
}