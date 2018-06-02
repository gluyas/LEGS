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
}

[Serializable]
public class Team
{
	public Color Color;
	public String Name;
}