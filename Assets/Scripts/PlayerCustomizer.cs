using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCustomizer : MonoBehaviour
{
	[Header("Public GameObjects")]
	public GameObject NoControllerMenu;
	public Text TitleText;
	public GameObject SelectionMenu;
	public Text SelectionText;
	public Player PlayerModel;

	[Header("To Customize")]
	[NonSerialized] private ShoeType _selectedShoe;

	[NonSerialized] private int _selectedTeam = 0;

	[NonSerialized] public bool IsReady;

	[NonSerialized] private PlayerInfo _currentPlayerInfo;

	private int CustomizerStage = 1;


	public PlayerInfo CurrentPlayerInfo
	{
		get { return _currentPlayerInfo; }
		set
		{
			_currentPlayerInfo = value;
			_selectedTeam = FindIndex(value.Team, GameplayManager.Instance.PlayerTeams);
			PlayerModel.SetPlayerInfo(value);
		}
	}

	private static int FindIndex<T>(T elem, IList<T> list)
	{
		for (var i = 0; i < list.Count; i++) if (list[i].Equals(elem)) return i;
		return 0;
	}
	
	private InputDevice Controller
	{
		get { return CurrentPlayerInfo.Controller; }
	}
		




	private void Update ()
	{


		if (CurrentPlayerInfo == null)
		{
			NoControllerMenu.SetActive(true);	
			SelectionMenu.SetActive(false);	
			return;
		}
		else
		{
			NoControllerMenu.SetActive(false);	
			SelectionMenu.SetActive(true);	
		}

		if (!IsReady) 
			SelectionText.text = _selectedShoe.ToString();

		else 
			SelectionText.text = "READY";

		bool changed = false;

		if (Controller.Action1.WasPressed) {
			if(CustomizerStage != 3)
				CustomizerStage++;
		}

		if (Controller.Action2.WasPressed) {
			if(CustomizerStage != 1)
				CustomizerStage--;
		}
		
		//      *******************  STAGES   ************************* 

		if (Controller.LeftBumper.WasPressed)
		{
			if (CustomizerStage == 1) {
				_selectedShoe = Shoe.PrevType(_selectedShoe);
				changed = true;
			}
			else if (CustomizerStage == 2)  //      ******************* COLOR
			{  
				_selectedTeam = (_selectedTeam - 1) % GameplayManager.Instance.PlayerTeams.Length;
				changed = true;
			}

		}
		
		if (Controller.RightBumper.WasPressed)
		{
			if (CustomizerStage == 1) {
				_selectedShoe = Shoe.NextType(_selectedShoe);
				changed = true;
			}
			else if (CustomizerStage == 2)  //      ******************* COLOR
			{  
				_selectedTeam = (_selectedTeam + 1) % GameplayManager.Instance.PlayerTeams.Length;
				changed = true;			
			}
		}

		// update the player info
		if (changed)
		{
			CurrentPlayerInfo.ShoeLeft = _selectedShoe;
			CurrentPlayerInfo.ShoeRight = _selectedShoe;

			CurrentPlayerInfo.Team = GameplayManager.Instance.PlayerTeams[_selectedTeam];
			
			PlayerModel.SetPlayerInfo(CurrentPlayerInfo);
		}

		if (CustomizerStage == 1)
		{
			TitleText.text = "SELECT SHOE";
		} 
		else if (CustomizerStage == 2) 
		{
			TitleText.text = "SELECT TEAM";
			SelectionText.text = CurrentPlayerInfo.Team.Name;
		}

	
		if (Controller.Action1.WasPressed && CustomizerStage == 3)
		{
			IsReady = !IsReady;
		}
	}
}
