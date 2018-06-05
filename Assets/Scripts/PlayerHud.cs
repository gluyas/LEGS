using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
	public Image PlayerPortrait;
	
	public Image BarPrimary;
	public Image BarSecondary;

	public float DecayTimeSecondary;
	[NonSerialized] public float Secondary;

	[NonSerialized] public Player TargetPlayer;
	
	private void Update()
	{
		float primary;
		if (TargetPlayer == null) primary = 0;
		else  				      primary = Mathf.Clamp01(TargetPlayer.Hp);
		
		BarPrimary.transform.localScale = new Vector2(primary, 1);
		
		Secondary = Mathf.Clamp(Secondary - Time.deltaTime / DecayTimeSecondary, primary, 1);
		BarSecondary.transform.localScale = new Vector2(Secondary, 1);
	}
}
