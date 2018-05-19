using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class GameplayManager : MonoBehaviour 
{
	public static GameplayManager Instance { get; private set; }
	
	[NonSerialized] public List<PlayerInfo> Players = new List<PlayerInfo>();

	public GameObject PlayerPrefab;
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
	
	// GAMEPLAY FUNCTIONS

	public Player InstantiatePlayer(PlayerInfo playerInfo, Vector2 position)
	{	
		var instance = Instantiate(PlayerPrefab, position, Quaternion.identity);
		var player = instance.GetComponentInChildren<Player>();

		player.Controller = playerInfo.Controller;

		foreach (var renderer in instance.GetComponentsInChildren<SpriteRenderer>())
		{
			renderer.color = playerInfo.TeamColor;
		}

		player.LegLeft.EquipShoe(InstantiateShoe(playerInfo.ShoeLeft));
		player.LegRight.EquipShoe(InstantiateShoe(playerInfo.ShoeRight));
		
		return player;
	}

	public Shoe InstantiateShoe(ShoeType type, Vector2 position = default(Vector2))
	{
		foreach (var prefab in ShoePrefabs)
		{
			var prefabShoe = prefab.GetComponent<Shoe>();
			if (prefabShoe.Type == type)
			{
				var instance = Instantiate(prefab, position, Quaternion.identity);
				return instance.GetComponent<Shoe>();
			}
		}
		Debug.LogAssertionFormat("No Shoe prefab for shoe type {0}", type);
		return null;
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
		Debug.AssertFormat(PlayerPrefab.GetComponentInChildren<Player>() != null, 
			"Player prefab {0} does not have a Player component", PlayerPrefab);
		
		foreach (var shoe in ShoePrefabs)
		{
			Debug.AssertFormat(shoe.GetComponent<Shoe>() != null, 
				"Shoe prefab {0} does not have a Shoe component", shoe);
		}

		for (int i = 0; i < PlayerColorsDefault.Length; i++)
		{
			var color = PlayerColorsDefault[i];
			color.a = 1f;
			PlayerColorsDefault[i] = color;
		}
	}
}
