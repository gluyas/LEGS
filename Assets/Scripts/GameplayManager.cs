using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class GameplayManager : MonoBehaviour {

	public GameObject CharacterControllerPrefab;

	public GameObject[] CharacterSelectionUISlots;

	public List<PlayerInfo> Players = new List<PlayerInfo> ();

	public int amountShoes;
	private string selectorText;
	public int stageSelector;


	// Use this for initialization
	void Start () {
		amountShoes = 2;

		int playerCount = 0;
		foreach (var device in InputManager.Devices) {
			var playerInfo = new PlayerInfo();
			playerInfo.Controller = device;
			playerInfo.PlayerNum = playerCount++;

			Players.Add(playerInfo);
		}

		foreach (var playerInfo in Players) {
			var charController = Instantiate (CharacterControllerPrefab);
			charController.GetComponent<CharController>().Init(playerInfo);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
