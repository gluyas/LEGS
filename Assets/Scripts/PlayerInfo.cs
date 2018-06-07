using System;
using UnityEngine;
using InControl;
using UnityEngine.Events;

/// <summary>
/// Data container to represent player configuration 
/// </summary>
public class PlayerInfo {	
	[NonSerialized] public readonly PlayerDamageEvent OnDamageTaken = new PlayerDamageEvent();
	[NonSerialized] public readonly PlayerDamageEvent OnDamageDealt = new PlayerDamageEvent();
	[NonSerialized] public readonly PlayerDeathEvent  OnDeath = new PlayerDeathEvent();
	[NonSerialized] public readonly PlayerDeathEvent  OnKill = new PlayerDeathEvent();
	
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

	[NonSerialized] public bool IsActive;
	[NonSerialized] public int Kills;
	[NonSerialized] public int Deaths;
	[NonSerialized] public float DamageDealt;
	[NonSerialized] public float DamageReceived;
}

// boilerplate classes
public class PlayerDamageEvent : UnityEvent<PlayerInfo, PlayerInfo, float> {}

public class PlayerDeathEvent : UnityEvent<PlayerInfo, PlayerInfo> {}