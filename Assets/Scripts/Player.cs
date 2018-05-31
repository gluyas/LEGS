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

	[NonSerialized] public float Hp = 1;

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

	public float AttackChargeTime;
	public float AttackChargeThreshold;
	public float AttackChargeWindUpArc;

	public float AttackDirectionSensitivity;
	
	public float AttackMovementFactorMax;
	public float AttackMovementFactorMin;	
	public float AttackArcMax;
	public float AttackArcMin;
	
	public float AttackDamageMax;
	public float AttackDamageMin;
	
	public float AttackRecoveryTime;
	
	public Leg LegRight;
	public Leg LegLeft;
	public Rigidbody2D Head	// exposing this to improve future flexibility; ie changing player structure
	{
		get { return this.GetComponent<Rigidbody2D>(); }
	}

	public void SetPlayerInfo(PlayerInfo playerInfo)
	{
		Controller = playerInfo.Controller;

		Head.GetComponent<SpriteRenderer>().color = playerInfo.TeamColor;
		LegLeft.GetComponent<SpriteRenderer>().color = playerInfo.TeamColor;
		LegRight.GetComponent<SpriteRenderer>().color = playerInfo.TeamColor;

		Shoe oldShoe;
		
		oldShoe = LegLeft.CurrentShoe;
		LegLeft.EquipShoe(GameplayManager.Instance.InstantiateShoe(playerInfo.ShoeLeft));
		if (oldShoe != null) Destroy(oldShoe.gameObject);

		oldShoe = LegRight.CurrentShoe;
		LegRight.EquipShoe(GameplayManager.Instance.InstantiateShoe(playerInfo.ShoeRight));
		if (oldShoe != null) Destroy(oldShoe.gameObject);
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

		if (GameplayManager.Instance == null)
		{
			PlayerId = _playerCount++;
			{
				// set the controller. currently only works the first time when loaded in a scene
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
	}
	
	private void FixedUpdate () 
	{
		if (Controller == null) return;
		
		UpdateLeg(LegLeft,  Controller.LeftStick.Vector,  Controller.LeftTrigger,  Controller.LeftBumper);
		UpdateLeg(LegRight, Controller.RightStick.Vector, Controller.RightTrigger, Controller.RightBumper);	

		{	// pick up items
			var tryEquipLeft  = Controller.LeftBumper && !LegLeft.TryEquip;	// disallow simultaneous equip
			var tryEquipRight = !tryEquipLeft && Controller.RightBumper && !LegRight.TryEquip;

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
					Leg targetLeg;
					if (tryEquipLeft) targetLeg = LegLeft;
					else              targetLeg = LegRight;

					var targetPos = (Vector2) 
						(targetLeg.transform.position + targetLeg.transform.rotation * targetLeg.ShoePosOffset);
					
					var closest = availableItems
						.OrderBy(shoe => Vector2.Distance(shoe.transform.position, targetPos))
						.First();
					
					targetLeg.EquipShoe(closest);
					targetLeg.TryEquip = true;
				}
			}
			// only reset hold status to false here: allow player to pre-emptively hold equip button
			if (!Controller.LeftBumper)  LegLeft.TryEquip  = false;
			if (!Controller.RightBumper) LegRight.TryEquip = false;
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
	
	private void UpdateLeg(Leg leg, Vector2 joystick, InputControl trigger, InputControl bumper)
	{
		var legDirWorldSpace = (Vector2)
			(Quaternion.AngleAxis(-leg.Hinge.jointAngle, new Vector3(0, 0, 1)) * -Head.transform.up);
		
		if (joystick.magnitude > LegDeadzoneMagnitude)
		{
			leg.LastInputDirection = joystick.normalized;
		}
		joystick = leg.LastInputDirection;
		
		{   // leg movement		
			if (leg.AttackButtonHeld)
			{
				leg.AttackButtonHeld = bumper.IsPressed;
			}
			else if (bumper.IsPressed && !leg.IsAttacking && !leg.IsAttackRecovering)
			{
				if (!leg.IsAttackCharging)	// if this is first frame of attack charge
				{
					leg.AttackRotation = -Mathf.Sign(leg.Rigidbody.angularVelocity);
					leg.AttackTargetDirection = joystick;
				}				
				leg.AttackCharge += Time.deltaTime / AttackChargeTime;

				var theta = Vector2.SignedAngle(leg.AttackTargetDirection, joystick);
				var buffer = -theta * leg.AttackRotation;

				if (buffer <= 0)
				{
					leg.AttackTargetDirection = joystick;
				}
				else if (buffer > AttackDirectionSensitivity)
				{
					leg.AttackTargetDirection = joystick;
					leg.AttackRotation *= -1;
				}
#if true
				{	// debug draw
					Debug.DrawRay(Head.transform.position, joystick, Color.blue);
					Debug.DrawRay(Head.transform.position, leg.AttackTargetDirection, Color.cyan);
					
					var arc = -leg.AttackRotation * Mathf.Lerp(AttackArcMin, AttackArcMax, leg.AttackCharge);
					Debug.DrawRay(Head.transform.position,
						Quaternion.AngleAxis(arc, Vector3.forward) * legDirWorldSpace, Color.magenta);
				}
#endif
			}
			
			if (leg.IsAttackCharging && (!bumper.IsPressed || leg.AttackCharge >= 1))
			{
				if (leg.AttackCharge < AttackChargeThreshold)
				{
					leg.AttackCharge = 0;
					leg.AttackDamage = 0;
					leg.AttackRotation = 0;
				}
				else
				{
					leg.AttackCharge = Mathf.Clamp01(
						(leg.AttackCharge - AttackChargeThreshold) / (1 - AttackChargeThreshold));
					
					var arc = -leg.AttackRotation * Mathf.Lerp(AttackArcMin, AttackArcMax, leg.AttackCharge);
					leg.AttackTargetDirection = Quaternion.AngleAxis(arc, Vector3.forward) * legDirWorldSpace;
				
					leg.AttackDamage = Mathf.Lerp(AttackDamageMax, AttackDamageMin, leg.AttackCharge);
				
					leg.AttackButtonHeld = bumper.IsPressed;
					leg.AttackRecovery = AttackRecoveryTime;
				}
			}

			var motor = leg.Hinge.motor;	
			
			if (leg.IsAttacking)			// attack movement
			{				
				var wishDir        = leg.AttackTargetDirection;
				var theta          = Vector2.SignedAngle(wishDir, legDirWorldSpace);
				var movementFactor = Mathf.Lerp(AttackMovementFactorMin, AttackMovementFactorMax, leg.AttackCharge);

				motor.motorSpeed     = LegSpeedMax * leg.AttackRotation * movementFactor;
				motor.maxMotorTorque = LegTorqueMax * movementFactor;
			
				if (Mathf.Sign(theta) * leg.AttackRotation < 0)	// reached/overshot target: end attack
				{
					leg.AttackCharge = 0;
					leg.AttackDamage = 0;
					leg.AttackRotation = 0;
				}
			}
			else  							// regular movement
			{
				Vector2 wishDir;
				if (leg.IsAttackRecovering)
				{
					wishDir = leg.AttackTargetDirection;
					leg.AttackRecovery -= Time.deltaTime;
				}
				else
				{
					wishDir = Quaternion.AngleAxis(
						leg.AttackRotation * leg.AttackCharge * AttackChargeWindUpArc, Vector3.forward) * joystick;
				}				
				var theta           = Vector2.SignedAngle(wishDir, legDirWorldSpace);		
				var smoothingFactor = Mathf.Clamp01(Mathf.Abs(theta) / LegSmoothAngle);

				motor.motorSpeed = LegSpeedMax * Mathf.Sign(theta) * smoothingFactor;
				motor.maxMotorTorque = Mathf.Lerp(LegTorqueMin, LegTorqueMax, smoothingFactor);
			}
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
				
				
				case ShoeType.Gun:
					Debug.Assert(leg.CurrentShoe is GunShoe);
					var gun = leg.CurrentShoe as GunShoe;

					if (triggerHeld)
					{
						gun.Charge += Time.deltaTime / Mathf.Lerp(gun.ChargeTimeMax, gun.ChargeTimeMin, trigger);
					}
					if (triggerUp || gun.Charge >= 1)
					{
						gun.Charge = Mathf.Clamp01(gun.Charge);
						if (gun.Charge < gun.ChargeThreshold)
						{
							// fizzle
						}
						else
						{
							var power = Mathf.Pow(
								(gun.Charge - gun.ChargeThreshold) / (1 - gun.ChargeThreshold), gun.ChargeExponent);
							
							leg.EquipShoe(null);	// drop the shoe: send it flying
							leg.GetComponent<Rigidbody2D>();
							
							leg.Rigidbody.AddForce(-legDirWorldSpace *
								Mathf.Lerp(gun.KnockbackForceMin, gun.KnockbackForceMax, power), ForceMode2D.Impulse);

							gun.Rigidbody.velocity += legDirWorldSpace *
								Mathf.Lerp(gun.ProjectileSpeedMin, gun.ProjectileSpeedMax, power);
						}
						gun.Charge = 0;
					}
					gun.GetComponent<Renderer>().material.color = gun.ChargeColor.Evaluate(gun.Charge);
					break;
				
				
				case ShoeType.Rocket:
					Debug.Assert(leg.CurrentShoe is RocketShoe);
					var rocket = leg.CurrentShoe as RocketShoe;

					if (triggerHeld)
					{
						if (rocket.Fuel <= 0) break;
						
						rocket.Fuel -= Time.deltaTime /
						    Mathf.Lerp(rocket.BurnTimeMax, rocket.BurnTimeMin, trigger);

						var efficiency = Mathf.Clamp01(rocket.Fuel / rocket.FuelEfficiencyFalloff);
						leg.Rigidbody.AddForce(-legDirWorldSpace * efficiency *
							Mathf.Lerp(rocket.ForceMin, rocket.ForceMax, trigger));
					}
					else
					{
						rocket.Fuel += Time.deltaTime / rocket.RegenerationTime;
					}
					rocket.Fuel = Mathf.Clamp01(rocket.Fuel);
					rocket.GetComponent<Renderer>().material.color = rocket.FuelLevelColor.Evaluate(rocket.Fuel);
					break;
			}
		}
	}
}
