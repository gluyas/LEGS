using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using NUnit.Framework;
using UnityEngine;

public class Player : MonoBehaviour
{
	private const float LegDeadzoneMagnitude = 0.05f;
	
	private static int _playerCount = 0;
	
	[NonSerialized] public int PlayerId;
	[NonSerialized] public InputDevice Controller;

#if false
	public float HeadTorqueMax;
	public float HeadStabilizerCurve;
	public float HeadTiltThetaMax;
	public Vector2 HeadMassOffset;
#endif

	public Collider2D[] ItemPickupZones;
	
	public float LegSpeedMax;
	public float LegTorqueMax;
	public float LegTorqueMin;
	public float LegSmoothAngle = 1;
	
	public Leg LegRight;
	public Leg LegLeft;
	public Rigidbody2D Head	// exposing this to improve future flexibility; ie changing player structure
	{
		get { return this.GetComponent<Rigidbody2D>(); }
	}
	
	private void Start ()
	{
		//Head.centerOfMass += HeadMassOffset;
		
		{	// ignore self collisions
			var collider = Head.GetComponent<Collider2D>();
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
		if (Controller == null) return;
		
		UpdateLeg(LegLeft,  Controller.LeftStick.Vector,  Controller.LeftTrigger);
		UpdateLeg(LegRight, Controller.RightStick.Vector, Controller.RightTrigger);	

		{	// pick up items
			var tryEquipLeft  = Controller.LeftBumper && !LegLeft.IsBumperHeld;	// disallow simultaneous equip
			var tryEquipRight = !tryEquipLeft && Controller.RightBumper && !LegRight.IsBumperHeld;

			if (tryEquipLeft || tryEquipRight)
			{
				var availableItems = new List<Shoe>();

				var itemFilter = new ContactFilter2D();
				itemFilter.SetLayerMask(LayerMask.GetMask("Pickups"));

				foreach (var zone in ItemPickupZones) // query pickup zones
				{
					var colliders = new Collider2D[5];
					var count = zone.OverlapCollider(itemFilter, colliders);

					for (int i = 0; i < count; i++)
					{
						var shoe = colliders[i].GetComponent<Shoe>();
						if (shoe.IsEquipped) continue;	// skip already equipped shoes

						availableItems.Add(shoe); // accumulate into list
					}
				}

				if (availableItems.Count > 0)
				{
					var closest = availableItems
						.OrderBy(s => Vector2.Distance(s.transform.position, Head.transform.position))
						.First();
					
					if (tryEquipLeft)	// only set hold status to true once they actually got an item
					{
						LegLeft.EquipShoe(closest);
						LegLeft.IsBumperHeld = true;
					}
					else
					{
						LegRight.EquipShoe(closest);
						LegRight.IsBumperHeld = true;
					}
				}
			}
			// only reset hold status to false here: allow player to pre-emptively hold equip button
			if (!Controller.LeftBumper)  LegLeft.IsBumperHeld  = false;
			if (!Controller.RightBumper) LegRight.IsBumperHeld = false;
		}

#if false
		{	// head stabilization
			// angular velocity (omega)
			var omega	    = Head.angularVelocity;
			var omegaTarget = 0;

			var omegaStabilizer = Mathf.Sign(omegaTarget - omega);
			
			// orientation (theta)
			var dirFacing  = Head.transform.up;
			var dirUpright = Vector2.up;
			
			if (Controller.LeftTrigger || Controller.RightTrigger)	// user head tilt
			{
				var rollDir = Controller.LeftTrigger.Value - Controller.RightTrigger.Value;
				dirUpright = Quaternion.Euler(0, 0, rollDir * HeadTiltThetaMax) * dirUpright;
			}
		
			var thetaStabilizer = Vector2.SignedAngle(dirFacing, dirUpright) / 180;
			
			// interpolate between stabilizers to create dynamic equilibrium
			omega = Mathf.Abs(omega);
			omega *= HeadStabilizerCurve;
			Head.AddTorque(HeadTorqueMax * Mathf.Lerp(thetaStabilizer, omegaStabilizer, omega / (omega + 1)));
		}
#endif
	}
	
	private void UpdateLeg(Leg leg, Vector2 joystick, InputControl trigger)
	{
		var legDirWorldSpace = Quaternion.AngleAxis(-leg.Hinge.jointAngle, new Vector3(0, 0, 1)) * -Head.transform.up;
		
		if (joystick.magnitude > LegDeadzoneMagnitude)
		{
			leg.CurrentInputDir = joystick.normalized;
		}
		joystick = leg.CurrentInputDir;
		
		{ 	// leg movement
			var motor = leg.Hinge.motor;

			var theta = Vector2.SignedAngle(joystick, legDirWorldSpace);
			var smoothingFactor = Mathf.Clamp01(Mathf.Abs(theta) / LegSmoothAngle);

			motor.motorSpeed = LegSpeedMax * Mathf.Sign(theta) * smoothingFactor;
			motor.maxMotorTorque = Mathf.Lerp(LegTorqueMin, LegTorqueMax, smoothingFactor);

			leg.Hinge.motor = motor;
	
#if false
		{	// debug stuff
			Color color;
			if (leg == LegLeft) color = Color.red;
			else 				color = Color.green;
			
			Debug.DrawRay(transform.position, inputDir, Color.Lerp(color, Color.grey, 0.5f));
			Debug.DrawRay(transform.position, legDirWorldSpace, color);
		}
#endif
		}

		var triggerHeld = trigger.IsPressed;
		var triggerDown = trigger.WasPressed;
		var triggerUp   = trigger.WasReleased;
		
		if (leg.HasShoe) {	// shoe abilities		
			switch (leg.CurrentShoe.Type)
			{
				case ShoeType.Debug:
					leg.CurrentShoe.transform.localEulerAngles = new Vector3(0, 0, trigger * 180);
					break;
			}
		}
	}
}
