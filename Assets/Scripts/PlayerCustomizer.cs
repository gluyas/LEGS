using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCustomizer : MonoBehaviour
{
	public Text ShoeSelectionText;
	
	[NonSerialized] public PlayerInfo CurrentPlayerInfo;
	private InputDevice Controller
	{
		get { return CurrentPlayerInfo.Controller; }
	}
		
	[NonSerialized] private ShoeType _selectedShoe;

	[NonSerialized] public bool IsReady;
	
	private void Update ()
	{
		if (CurrentPlayerInfo == null) return;

		ShoeSelectionText.text = _selectedShoe.ToString();

		if (Controller.LeftBumper.WasPressed)
		{
			_selectedShoe = Shoe.PrevType(_selectedShoe);
		}
		
		if (Controller.RightBumper.WasPressed)
		{
			_selectedShoe = Shoe.NextType(_selectedShoe);
		}

		// update the player info
		CurrentPlayerInfo.ShoeLeft  = _selectedShoe;
		CurrentPlayerInfo.ShoeRight = _selectedShoe;

		if (Controller.Action1.WasPressed)
		{
			IsReady = !IsReady;
		}
	}
}
