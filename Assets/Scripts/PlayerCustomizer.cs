using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

public class PlayerCustomizer : MonoBehaviour
{
	public Text ShoeSelectionText;
	
	[NonSerialized] public PlayerInfo CurrentPlayerInfo;
	private InputDevice Controller
	{
		get { return CurrentPlayerInfo.Controller; }
	}
	
	private bool _isLeftButtonHeld, _isRightButtonHeld;
	private ShoeType _selectedShoe;
	
	private void Update ()
	{
		if (CurrentPlayerInfo == null) return;

		ShoeSelectionText.text = _selectedShoe.ToString();

		if (!_isLeftButtonHeld)
		{
			if (Controller.LeftBumper)
			{
				_selectedShoe = Shoe.PrevType(_selectedShoe);
				_isLeftButtonHeld = true;
			}
		}
		else _isLeftButtonHeld = Controller.LeftBumper;
		
		if (!_isRightButtonHeld)
		{
			if (Controller.RightBumper)
			{
				_selectedShoe = Shoe.NextType(_selectedShoe);
				_isRightButtonHeld = true;
			}
		}
		else _isRightButtonHeld = Controller.RightBumper;
		
		// update the player info
		CurrentPlayerInfo.ShoeLeft  = _selectedShoe;
		CurrentPlayerInfo.ShoeRight = _selectedShoe;
	}
}
