using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
	public Player Player;
	
	public GameObject BarPrimary;
	public GameObject BarSecondary;

	public float DecayTimeSecondary;
	[NonSerialized] public float Secondary;
	
	private void Update()
	{
		var primary = Mathf.Clamp01(Player.Hp);
		BarPrimary.transform.localScale = new Vector2(primary, 1);
		
		Secondary = Mathf.Clamp(Secondary - Time.deltaTime / DecayTimeSecondary, primary, 1);
		BarSecondary.transform.localScale = new Vector2(Secondary, 1);
	}
}
