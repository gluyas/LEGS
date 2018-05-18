using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerCustomizer : MonoBehaviour {

	private static int elementLength = 5;

	// player config instance that this script sets the values of


	// Player's selection for the stage


	private bool isButtonPressed;

	public PlayerInfo CurrentPlayerInfo;

	public void Init(PlayerInfo playerInfo) {
		this.CurrentPlayerInfo = playerInfo;
		Debug.Log ("akjshf");
	}

	void Update () {
		//Debug.Log (CurrentPlayerInfo.LeftShoeType);

		if (!isButtonPressed) {
			if (CurrentPlayerInfo.Controller.LeftTrigger) {
				CurrentPlayerInfo.LeftShoeType--;
				isButtonPressed = true;
				Debug.Log (CurrentPlayerInfo.LeftShoeType);
			}
			else if (CurrentPlayerInfo.Controller.RightTrigger) {
				CurrentPlayerInfo.LeftShoeType++;
				isButtonPressed = true;
				Debug.Log (CurrentPlayerInfo.LeftShoeType);
			}

			CurrentPlayerInfo.LeftShoeType = (elementLength + CurrentPlayerInfo.LeftShoeType) % elementLength;

		} else if (!CurrentPlayerInfo.Controller.LeftTrigger && !CurrentPlayerInfo.Controller.RightTrigger) {
			isButtonPressed = false;
			//Debug.Log ("Foo");
		}

		//Debug.LogFormat ("{0}, {1}", CurrentPlayerInfo.Controller.LeftTrigger.Value, CurrentPlayerInfo.Controller.RightTrigger.Value);
	}
}

public class PlayerInfo {
	public int PlayerNum;
	public InputDevice Controller;
	public int LeftShoeType, RightShoeType;
	public Color TeamColor;
}

