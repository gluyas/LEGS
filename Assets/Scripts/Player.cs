using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;

public class Player : MonoBehaviour
{
	private static int _playerCount = 0;
	
	[NonSerialized] public int PlayerId;
	[NonSerialized] public InputDevice Controller;

	public float LegSpeedMax = 3000;
	public float LegTorqueMax = 1000;
	
	public Leg LegRight;
	public Leg LegLeft;
	public Transform Body	// exposing this to improve future flexibility; ie changing player structure
	{
		get { return this.transform; }
	}

	private void Start ()
	{
		{	// ignore self collisions
			var collider = GetComponent<Collider2D>();	// TODO: make into Body's collider
			Physics2D.IgnoreCollision(collider, LegLeft.Collider);
			Physics2D.IgnoreCollision(collider, LegRight.Collider);
			Physics2D.IgnoreCollision(LegLeft.Collider, LegRight.Collider);
		}
		
		PlayerId = _playerCount++;
		{	// set the controller. currently only works the first time when loaded in a scene
			var controllers = InputManager.Devices.ToList();
			if (PlayerId < controllers.Count)
			{
				Controller = controllers[PlayerId];
			}
			else
			{
				Debug.LogFormat("Player{0} missing controller", PlayerId);
			}
		}
	}
	
	private void FixedUpdate () 
	{
		if (Controller != null)
		{
			UpdateLeg(LegLeft,  Controller.LeftStick);
			UpdateLeg(LegRight, Controller.RightStick);
		}
	}

	private void UpdateLeg(Leg leg, TwoAxisInputControl input)
	{
		var inputDir = input.Vector;
		var legDirWorldSpace = Quaternion.AngleAxis(-leg.Hinge.jointAngle, new Vector3(0, 0, 1)) * -Body.up;

		var motor = leg.Hinge.motor;
		motor.motorSpeed = LegSpeedMax / 360 * Vector2.SignedAngle(inputDir, legDirWorldSpace);
		motor.maxMotorTorque = LegTorqueMax * inputDir.magnitude;
		
		leg.Hinge.motor = motor;
		
		{	// debug stuff
			Color color;
			if (leg == LegLeft) color = Color.red;
			else 				color = Color.green;
			
			Debug.DrawRay(transform.position, inputDir, Color.Lerp(color, Color.grey, 0.5f));
			Debug.DrawRay(transform.position, legDirWorldSpace, color);
		}
	}
}
