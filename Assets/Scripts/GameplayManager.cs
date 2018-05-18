using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class GameplayManager : MonoBehaviour 
{
	public static GameplayManager Instance { get; private set; }
	
	[NonSerialized] public List<PlayerInfo> Players = new List<PlayerInfo>();

	public GameObject[] ShoePrefabs; 
	
	public GameObject MainMenu;

	public GameObject PlayerCustomizationMenu;
	public PlayerCustomizer[] PlayerCustomizers;	// these should be contained within PlayerCystomizationMenu
	public Color[] PlayerColorsDefault;

	private void Start()
	{
		Instance = this;
		
		var n = 0;
		foreach (var device in InputManager.Devices) {	// set up default player configs
			var playerInfo = new PlayerInfo();
			
			playerInfo.Controller = device;
			playerInfo.PlayerNum = n;
			playerInfo.TeamColor = n < PlayerColorsDefault.Length ? PlayerColorsDefault[n] : PlayerColorsDefault[0];

			Players.Add(playerInfo);
			n++;
		}
	}

	// UI BUTTONS
	public void PlayButton() {
		MainMenu.SetActive(false);
		
		// enable player customisation and set up customizers.
		PlayerCustomizationMenu.SetActive(true);
		for (var i = 0; i < Mathf.Min(Players.Count, PlayerCustomizers.Length); i++)
		{
			PlayerCustomizers[i].CurrentPlayerInfo = Players[i];
		}
	}

	public void QuitButton() {
		if (InputManager.ActiveDevice.Action2)
			Application.Quit();
	}

	private void OnValidate()
	{
		foreach (var shoe in ShoePrefabs)
		{
			Debug.AssertFormat(shoe.GetComponent<Shoe>() != null, 
				"Shoe prefab {0} does not have a Shoe component", shoe);
		}
	}
}
