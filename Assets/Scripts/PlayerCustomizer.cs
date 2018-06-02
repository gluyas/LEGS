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
	public Color[] PlayerColors;

	[NonSerialized] private ShoeType _selectedShoe;

	[NonSerialized] private Color _selectedColor;

	[NonSerialized] private int i = 0;

	[NonSerialized] public bool IsReady;

	[NonSerialized] private PlayerInfo _currentPlayerInfo;

	private int CustomizerStage = 1;


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

		if (CustomizerStage == 1) 
			TitleText.text = "SELECT SHOE";

		if (CustomizerStage == 2) {
			TitleText.text = "SELECT COLOR";
			SelectionText.text = ("" + i);
		}




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
			if (CustomizerStage == 2) {   //      ******************* COLOR
				if (i == 0)
					i = 6;
				else 
					i--;
				SelectionText.text = ("" + i);
				_selectedColor = PlayerColors [i];
				changed = true;
	//			Debug.Log ("i: " + i);
	//			Debug.Log("COLOR: " + _selectedColor);

			}

		}
		
		if (Controller.RightBumper.WasPressed)
		{
			if (CustomizerStage == 1) {
				_selectedShoe = Shoe.NextType(_selectedShoe);
				changed = true;
			}
			if (CustomizerStage == 2) {   //      ******************* COLOR
				if (i == 6)
					i = 0;
				else 
					i++;
				SelectionText.text = ("" + i);
				_selectedColor = PlayerColors [i];
				changed = true;
//				Debug.Log ("i: " + i);
//				Debug.Log("COLOR: " + _selectedColor);
			}
		}





		// update the player info
		if (changed)
		{
			CurrentPlayerInfo.ShoeLeft = _selectedShoe;
			CurrentPlayerInfo.ShoeRight = _selectedShoe;
			if (CustomizerStage == 2)
				CurrentPlayerInfo.TeamColor = _selectedColor;
			PlayerModel.SetPlayerInfo(CurrentPlayerInfo);
		}

		if (Controller.Action1.WasPressed && CustomizerStage == 3)
		{
			IsReady = !IsReady;
		}
	}
}
