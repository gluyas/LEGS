using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using InControl;


public class ModeButton : MonoBehaviour, ISelectHandler 
{

    private GameplayManager GM;

    private Text ModeTitle;
    private Text ModeDesc;
    public string ModeTitleText;
    public string ModeDescText;
    bool canBack = true;


    public void Start()
    {
        GM = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

    }

    public void OnSelect(BaseEventData eventData)
    {
        ModeTitle = transform.parent.Find("ModeTitle").gameObject.GetComponent<Text>();
        ModeDesc = transform.parent.Find("ModeDesc").gameObject.GetComponent<Text>();
        ModeTitle.text = ModeTitleText;
        ModeDesc.text = ModeDescText;
    }

    void Update()
    {
        if (!InputManager.Devices[0].Action2)
        {
            canBack = true;
        }

        if (InputManager.Devices[0].Action2 && canBack && this.gameObject.name == "Mode2")
        {
            GM.firstSelected = GM.LevelModeMenus[GM.LevelSelected].transform.parent.gameObject;
            GM.LevelModeMenus[GM.LevelSelected].SetActive(false);

			GM.LevelSelectMenu.transform.Find("LevelTitle").gameObject.GetComponent<Text>().text = "SELECT LEVEL";

            EventSystem.current.SetSelectedGameObject(GM.firstSelected);
            canBack = false;
        }
        else if (InputManager.Devices[0].Action2 && canBack && this.gameObject.name == "Ready")
        {
            GM.firstSelected = GM.LevelModeMenus[GM.LevelSelected].transform.Find("Mode1").gameObject;
            GM.ReadyMenu.SetActive(false);
            GM.LevelModeMenus[GM.LevelSelected].SetActive(true);

			GM.LevelSelectMenu.transform.Find("LevelTitle").gameObject.GetComponent<Text>().text = "SELECT MODE";

            EventSystem.current.SetSelectedGameObject(GM.firstSelected);
            canBack = false;
        }
		else if (InputManager.Devices[0].Action2 && canBack && this.gameObject.name == "Level Select")
		{
			GM.firstSelected = GM.MainMenu.transform.Find("PLAY").gameObject;
			GM.LevelSelectMenu.SetActive(false);
			GM.MainMenu.SetActive(true);
			EventSystem.current.SetSelectedGameObject(GM.firstSelected);
			canBack = false;
		}
		else if (InputManager.Devices[0].Action2 && canBack && this.gameObject.name == "BACK")
		{
			GM.firstSelected = GM.MainMenu.transform.Find("CREDITS").gameObject;
			GM.MainMenu.transform.parent.Find("Credits").gameObject.SetActive(false);
			GM.MainMenu.SetActive(true);
			EventSystem.current.SetSelectedGameObject(GM.firstSelected);
			canBack = false;
		}
        
    }



    




}
