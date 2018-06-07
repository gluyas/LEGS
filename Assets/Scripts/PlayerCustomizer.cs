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
	
	public GameObject SelectionMenu;
	public Text TitleText;
	public Text SelectionText;
	
	public Player PlayerModel;

	[NonSerialized] private PlayerInfo _currentPlayerInfo;
	public PlayerInfo CurrentPlayerInfo
	{
		get { return _currentPlayerInfo; }
		set
		{
			_currentPlayerInfo = value;
			_selectedTeam = FindIndex(value.Team, GameplayManager.Instance.PlayerTeams);
			_selectedCostume = FindIndex(value.Costume, GameplayManager.Instance.PlayerCostumes);
			
			NoControllerMenu.SetActive(false);	
			SelectionMenu.SetActive(true);

			SelectionText.color = value.Team.Color;
			
			PlayerModel.SetPlayerInfo(value);
		}
	}

	private InputDevice Controller
	{
		get { return CurrentPlayerInfo.Controller; }
	}

	private const int MenuIsReadyIndex = 2;
	private int _selectionMenu =  -1;
	public bool IsReady
	{
		get { return _selectionMenu >= MenuIsReadyIndex; }
		set { _selectionMenu = value ? MenuIsReadyIndex : 0; }
	}

	private static int FindIndex<T>(T elem, IList<T> list)
	{
		for (var i = 0; i < list.Count; i++) if (list[i].Equals(elem)) return i;
		return 0;
	}

	private static int Mod(int dividend, int divisor)
	{
		return (divisor + dividend % divisor) % divisor;
	}
	
	[NonSerialized] private int _selectedTeam;
	[NonSerialized] private int _selectedCostume;
	
	private void Update()
	{
		if (CurrentPlayerInfo == null)	// check ready to be used
		{
			NoControllerMenu.SetActive(true);	
			SelectionMenu.SetActive(false);	
			return;
		}

		var prevMenu = _selectionMenu;
		if (Controller.Action1.WasPressed) _selectionMenu++;
		if (Controller.Action2.WasPressed) _selectionMenu--;
		_selectionMenu = Mathf.Clamp(_selectionMenu, 0, MenuIsReadyIndex);
		
		var shift = 0;	// shift item selection
		if (Controller.LeftBumper.WasPressed)  shift--;
		if (Controller.RightBumper.WasPressed) shift++;
		if (shift != 0 || prevMenu != _selectionMenu)
		{
			switch (_selectionMenu)
			{
				case 0:
					TitleText.text = "SELECT OUTFIT";
					
					_selectedCostume = Mod(_selectedCostume + shift, GameplayManager.Instance.PlayerCostumes.Length);					
					CurrentPlayerInfo.Costume = GameplayManager.Instance.PlayerCostumes[_selectedCostume];
					
					SelectionText.text = CurrentPlayerInfo.Costume.Name;				
					break;
#if false	// remember to update MenuIsReadyIndex!
				case 1:									
					TitleText.text = "SELECT SHOE";
					
					var nextShoe = Controller.LeftTrigger ? CurrentPlayerInfo.ShoeLeft : CurrentPlayerInfo.ShoeRight;
					if      (shift > 0) nextShoe = Shoe.NextType(nextShoe ?? default(ShoeType));
					else if (shift < 0) nextShoe = Shoe.PrevType(nextShoe ?? default(ShoeType));

					// secret feature: hold trigger to change only that shoe :--)
					if (!Controller.RightTrigger) CurrentPlayerInfo.ShoeLeft = nextShoe;
					if (!Controller.LeftTrigger) CurrentPlayerInfo.ShoeRight = nextShoe;

					SelectionText.text = nextShoe.ToString();
					break;
	
				case 2:
#else
				case 1:
#endif		
					TitleText.text = "SELECT TEAM";
					
					_selectedTeam = Mod(_selectedTeam + shift, GameplayManager.Instance.PlayerTeams.Length);
					CurrentPlayerInfo.Team = GameplayManager.Instance.PlayerTeams[_selectedTeam];
					
					SelectionText.text = CurrentPlayerInfo.Team.Name;
					SelectionText.color = CurrentPlayerInfo.Team.Color;
					break;
				
				default:	// final case: player is ready
					TitleText.text = "PLAYER READY";
					SelectionText.text = "B: CANCEL";
					break;
			}
			
			// finally, refresh onscreen player avatar
			PlayerModel.SetPlayerInfo(CurrentPlayerInfo);
		}
	}
}
