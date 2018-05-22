using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCustomizer : MonoBehaviour
{
	public GameObject NoControllerMenu;
	
	public GameObject ShoeSelectionMenu;
	public Text ShoeSelectionText;

	public Player PlayerModel;

	[NonSerialized] private PlayerInfo _currentPlayerInfo;
	public PlayerInfo CurrentPlayerInfo
	{
		get { return _currentPlayerInfo; }
		set
		{
			_currentPlayerInfo = value;
			PlayerModel.SetPlayerInfo(value);
		}
	}
	
	private InputDevice Controller
	{
		get { return CurrentPlayerInfo.Controller; }
	}
		
	[NonSerialized] private ShoeType _selectedShoe;

	[NonSerialized] public bool IsReady;
	
	private void Update ()
	{
		if (CurrentPlayerInfo == null)
		{
			NoControllerMenu.SetActive(true);	
			ShoeSelectionMenu.SetActive(false);	
			return;
		}
		else
		{
			NoControllerMenu.SetActive(false);	
			ShoeSelectionMenu.SetActive(true);	
		}

		if (!IsReady) ShoeSelectionText.text = _selectedShoe.ToString();
		else ShoeSelectionText.text = "READY";

		var changed = false;
		
		if (Controller.LeftBumper.WasPressed)
		{
			_selectedShoe = Shoe.PrevType(_selectedShoe);
			changed = true;
		}
		
		if (Controller.RightBumper.WasPressed)
		{
			_selectedShoe = Shoe.NextType(_selectedShoe);
			changed = true;
		}

		// update the player info
		if (changed)
		{
			CurrentPlayerInfo.ShoeLeft = _selectedShoe;
			CurrentPlayerInfo.ShoeRight = _selectedShoe;
			PlayerModel.SetPlayerInfo(CurrentPlayerInfo);
		}

		if (Controller.Action1.WasPressed)
		{
			IsReady = !IsReady;
		}
	}
}
