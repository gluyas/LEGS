using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class TestJoystick : MonoBehaviour {

	//INCONTROL
	[SerializeField] public int playerNum;
	
	public bool isButton = true;
	public bool leftJoystick = true;
	public string buttonName;

	private Vector3 startPos;
	private Transform thisTransform;
	private MeshRenderer mr;
	
	//INCONTROL
	private InputDevice joystick;

	void Start () 
	{
		thisTransform = transform;
		startPos = thisTransform.position;
		mr = thisTransform.GetComponent<MeshRenderer> ();
		
		//INCONTROL
		if(InputManager.Devices[playerNum] != null){
			joystick = InputManager.Devices[playerNum];
		}
	}

	void Update () 
	{
		if (isButton) 
		{
			mr.enabled = Input.GetButton (buttonName);
		} 

		else 
		{
			if (leftJoystick) 
			{
				Vector3 inputDirection = Vector3.zero;
				inputDirection.x = joystick.LeftStickX;
				inputDirection.y = joystick.LeftStickY;
				this.transform.position = startPos + inputDirection;
			} 

			else 
			{
				Vector3 inputDirection = Vector3.zero;
				inputDirection.x = joystick.RightStickX;
				inputDirection.y = joystick.RightStickY;
				thisTransform.position = startPos + inputDirection;
			}
		}
	}
}
