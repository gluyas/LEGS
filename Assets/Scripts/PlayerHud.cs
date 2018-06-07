﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
	public GameObject[] ScoreCounters;
	public Image[] ScoreCounterFills;
	
	public Image PlayerPortrait;
	
	public Image BarPrimary;
	public Image BarSecondary;

	public float DecayTimeSecondary;
	[NonSerialized] public float Secondary;

	[NonSerialized] private PlayerInfo _player;
	
	private void Update()
	{
		float primary;
		if (_player.Instance == null) primary = 0;
		else  				    	  primary = Mathf.Clamp01(_player.Instance.Hp);
		
		BarPrimary.transform.localScale = new Vector2(primary, 1);
		
		Secondary = Mathf.Clamp(Secondary - Time.deltaTime / DecayTimeSecondary, primary, 1);
		BarSecondary.transform.localScale = new Vector2(Secondary, 1);
	}

	private void OnValidate()
	{
		ScoreCounterFills = new Image[ScoreCounters.Length];
		for (var i = 0; i < ScoreCounters.Length; i++)
		{
			ScoreCounterFills[i] = ScoreCounters[i].GetComponentInChildren<Image>();
		}
	}

	public void SetPlayerInfo(PlayerInfo player)
	{
		_player = player;
		BarPrimary.color = player.Team.Color;
		PlayerPortrait.color = player.Team.Color;
		foreach (var image in ScoreCounterFills) image.color = player.Team.Color;
	}

	public void FillScoreCounters(int score, int max, bool reverse = false)
	{
		for (var i = 0; i < ScoreCounters.Length; i++)
		{
			ScoreCounters[i].SetActive(i < max);
			if (reverse) ScoreCounterFills[i].enabled = i >= max - score;
			else         ScoreCounterFills[i].enabled = i <  score;
		}
	}
}
