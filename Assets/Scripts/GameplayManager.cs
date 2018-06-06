using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using InControl;
using Random = UnityEngine.Random;


public class GameplayManager : MonoBehaviour 
{
	public static GameplayManager Instance { get; private set; }

	[NonSerialized] public List<PlayerInfo> Players = new List<PlayerInfo>();

	[SerializeField] [Header("Game Settings")]
	public float RespawnTime;

	public float ItemSpawnTimeMax;
	public float ItemSpawnTimeMin;
	public float ItemDespawnTime;
	public float ItemDespawnFlickerTime;
	
	[NonSerialized] private Transform[] _itemSpawns;
	
    [NonSerialized] public int LevelSelected;
	[NonSerialized] public String LevelName;
	[NonSerialized] public int ModeSelected;

    [Header("Menu Variables")]
    public String[] Levels;
    public GameObject[] LevelModeMenus;

    public GameObject MainMenu;
    public GameObject firstSelected;
    public GameObject ReadyMenu;

    public GameObject LevelSelectMenu;

    [Header("Player Variables")]
    public GameObject PlayerPrefab;
	public GameObject[] ShoePrefabs;

	public Costume[] PlayerCostumes;

    public GameObject PlayerCustomizationMenu;
    public PlayerCustomizer[] PlayerCustomizers;	// these should be contained within PlayerCustomizationMenu
	public Team[] PlayerTeams;

	private void Start()
	{
		Instance = this;
		DontDestroyOnLoad(this);
		
		var n = 0;
		foreach (var device in InputManager.Devices) {	// set up default player configs
			var playerInfo = new PlayerInfo
			{
				Controller = device,
				PlayerNum = n,
				Team = n < PlayerTeams.Length ? PlayerTeams[n] : PlayerTeams[PlayerTeams.Length - 1],
				Costume = PlayerCostumes[Random.Range(0, PlayerCostumes.Length)],
			};

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
			firstSelected = LevelSelectMenu.transform.Find("LEVEL1").gameObject;
			LevelSelectMenu.SetActive(true);
			EventSystem.current.SetSelectedGameObject(firstSelected);

		}

        //Debug.Log(EventSystem.current.currentSelectedGameObject);
    }



	// *********************** GAMEPLAY FUNCTIONS **************************

	// ********** LOAD LEVEL

	public IEnumerator LoadStage()
	{
		var load = SceneManager.LoadSceneAsync(LevelName);
		//var load = SceneManager.LoadSceneAsync("Scenes/TestArena");
		while (!load.isDone)
		{
			yield return null;
		}

		var spawns = GameObject.FindGameObjectsWithTag("Spawn").ToList();

		foreach (var playerInfo in Players)
		{
			Vector2 pos;
			if (spawns.Count > 0)
			{
				var i = Random.Range(0, spawns.Count);
				pos = spawns[i].transform.position;
				spawns.RemoveAt(i);
			}
			else pos = Vector2.zero;

			InstantiatePlayer(playerInfo, pos);
		}

		_itemSpawns = GameObject.FindGameObjectsWithTag("ItemSpawn")
			.Select(g => g.transform)
			.ToArray();
		StartCoroutine(DoItemSpawns());
	}

	private IEnumerator DoItemSpawns()
	{
		var time = Random.Range(ItemSpawnTimeMin, ItemSpawnTimeMax);
		yield return new WaitForSeconds(time);

		var pos = Vector2.zero;
		if (_itemSpawns.Length > 0) pos = _itemSpawns[Random.Range(0, _itemSpawns.Length)].position;

		Instantiate(ShoePrefabs[Random.Range(0, ShoePrefabs.Length)], pos, Quaternion.identity);
		
		StartCoroutine(DoItemSpawns());
	}

	
	public Player InstantiatePlayer(PlayerInfo playerInfo, Vector2 position = default(Vector2))
	{	
		var player = Instantiate(PlayerPrefab, position, Quaternion.identity).GetComponentInChildren<Player>();	
		player.SetPlayerInfo(playerInfo);
		
		player.OnDeath.AddListener((_, died) => StartCoroutine(RespawnPlayer(died.PlayerInfo, RespawnTime)));
		
		return player;
	}


	public IEnumerator RespawnPlayer(PlayerInfo player, float time)
	{				
		var spawns = GameObject.FindGameObjectsWithTag("Spawn").ToList();
		Vector2 pos;
		if (spawns.Count == 0) pos = Vector2.zero;
		else 				   pos = spawns[Random.Range(0, spawns.Count)].transform.position;
		
		yield return new WaitForSeconds(time);
		InstantiatePlayer(player, pos);
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








    // *********************** UI BUTTONS **************************
    



	public void Quit()
    {
		Application.Quit();
    }


    // ********** LEVEL SELECT

	public void SetLevelName(String name) {
		LevelName = name;
	}

    public void SelectBeach()
    {
        LevelSelected = 0;
        firstSelected = LevelModeMenus[LevelSelected].transform.Find("Mode1").gameObject;
        LevelModeMenus[LevelSelected].SetActive(true);

		LevelSelectMenu.transform.Find("LevelTitle").gameObject.GetComponent<Text>().text = "SELECT MODE";

        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void SelectDojo()
    {
        LevelSelected = 1;
        firstSelected = LevelModeMenus[LevelSelected].transform.Find("Mode1").gameObject;
        LevelModeMenus[LevelSelected].SetActive(true);

		LevelSelectMenu.transform.Find("LevelTitle").gameObject.GetComponent<Text>().text = "SELECT MODE";

        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void SelectCity()
    {
        LevelSelected = 2;
        firstSelected = LevelModeMenus[LevelSelected].transform.Find("Mode1").gameObject;
        LevelModeMenus[LevelSelected].SetActive(true);

		LevelSelectMenu.transform.Find("LevelTitle").gameObject.GetComponent<Text>().text = "SELECT MODE";

        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void SelectMouth()
    {
        LevelSelected = 3;
        firstSelected = LevelModeMenus[LevelSelected].transform.Find("Mode1").gameObject;
        LevelModeMenus[LevelSelected].SetActive(true);

		LevelSelectMenu.transform.Find("LevelTitle").gameObject.GetComponent<Text>().text = "SELECT MODE";

        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    // ********** MODE SELECT

    public void SelectMode1()
    {
        ModeSelected = 1;
        firstSelected = ReadyMenu.transform.Find("START").gameObject;
        ReadyMenu.SetActive(true);

		LevelSelectMenu.transform.Find("LevelTitle").gameObject.GetComponent<Text>().text = "";

        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void SelectMode2()
    {
        ModeSelected = 2;
        firstSelected = ReadyMenu.transform.Find("START").gameObject;
        ReadyMenu.SetActive(true);

		LevelSelectMenu.transform.Find("LevelTitle").gameObject.GetComponent<Text>().text = "";

        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void SelectMode3()
    {
        ModeSelected = 3;
        firstSelected = ReadyMenu.transform.Find("START").gameObject;
        ReadyMenu.SetActive(true);

		LevelSelectMenu.transform.Find("LevelTitle").gameObject.GetComponent<Text>().text = "";

        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

	public void StartButton()
	{
		StartCoroutine(LoadStage());
		LevelSelectMenu.SetActive(false);
	}


	// SCRIPT VALIDATION

	private void OnValidate()
	{	
		Debug.AssertFormat(PlayerPrefab.GetComponentInChildren<Player>() != null, 
			"Player prefab {0} does not have a Player component", PlayerPrefab);
		
		foreach (var shoe in ShoePrefabs)
		{
			Debug.AssertFormat(shoe.GetComponent<Shoe>() != null, 
				"Shoe prefab {0} does not have a Shoe component", shoe);
		}

		for (int i = 0; i < PlayerTeams.Length; i++)
		{
			var team = PlayerTeams[i];
			team.Color.a = 1f;
		}
	}
}
