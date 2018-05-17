using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		DontDestroyOnLoad(this);
		
		var n = 0;
		foreach (var device in InputManager.Devices) {	// set up default player configs
			var playerInfo = new PlayerInfo();
			
			playerInfo.Controller = device;
			playerInfo.PlayerNum = n;
			playerInfo.TeamColor = n < PlayerColorsDefault.Length ? PlayerColorsDefault[n] : PlayerColorsDefault[0];

			Players.Add(playerInfo);
			n++;
		}
		
		for (var i = 0; i < Mathf.Min(Players.Count, PlayerCustomizers.Length); i++)
		{
			PlayerCustomizers[i].CurrentPlayerInfo = Players[i];
		}
	}

	private void Update()
	{
		if (PlayerCustomizers.All(c => c.IsReady || c.CurrentPlayerInfo == null))
		{
			foreach (var c in PlayerCustomizers) c.IsReady = false;

			PlayerCustomizationMenu.SetActive(false);
			StartCoroutine(LoadStage());
		}
	}

	// GAMEPLAY FUNCTIONS
		
	public IEnumerator LoadStage()
	{
		var load = SceneManager.LoadSceneAsync("Scenes/TestArena");
		while (!load.isDone)
		{
			yield return null;
		}

		foreach (var playerInfo in Players)
		{
			InstantiatePlayer(playerInfo);
		}
	}

	public Player InstantiatePlayer(PlayerInfo playerInfo, Vector2 position = default(Vector2))
	{	
		var player = Instantiate(PlayerPrefab, position, Quaternion.identity).GetComponentInChildren<Player>();	
		player.SetPlayerInfo(playerInfo);
		
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
	public void Quit() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
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
