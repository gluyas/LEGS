﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
//using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	private const float LegDeadzoneMagnitude = 0.05f;
	
	private static int _playerCount = 0;

	[NonSerialized] public readonly PlayerDamageEvent OnDamageTaken = new PlayerDamageEvent();
	[NonSerialized] public readonly PlayerDamageEvent OnDamageDealt = new PlayerDamageEvent();
	[NonSerialized] public readonly PlayerDeathEvent  OnDeath = new PlayerDeathEvent();
	[NonSerialized] public readonly PlayerDeathEvent  OnKill = new PlayerDeathEvent();

	[NonSerialized] public int PlayerId;
	[NonSerialized] public PlayerInfo PlayerInfo;
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
	
	public float AttackRecoveryTimeMax;
	public float AttackRecoveryTimeMin;
	
	public Leg LegRight;
	public Leg LegLeft;
	public Rigidbody2D Head	// exposing this to improve future flexibility; ie changing player structure
	{
		get { return this.GetComponent<Rigidbody2D>(); }
	}

	public void SetPlayerInfo(PlayerInfo playerInfo)
	{
		PlayerInfo = playerInfo;
		Controller = PlayerInfo.Controller;	
		Start();

		foreach (Transform child in Head.transform)     Destroy(child.gameObject);
		foreach (Transform child in LegLeft.transform)  Destroy(child.gameObject);
		foreach (Transform child in LegRight.transform) Destroy(child.gameObject);

		Head.GetComponent<SpriteRenderer>().color = playerInfo.Team.Color;
		LegLeft.GetComponent<SpriteRenderer>().color = playerInfo.Team.Color;
		LegRight.GetComponent<SpriteRenderer>().color = playerInfo.Team.Color;
		
		EquipCostumePart(Head.transform, 	 playerInfo.Costume.Head);
		EquipCostumePart(LegLeft.transform,  playerInfo.Costume.LegLeft);
		EquipCostumePart(LegRight.transform, playerInfo.Costume.LegRight);
		
		LegLeft.EquipShoe(GameplayManager.Instance.InstantiateShoe(playerInfo.ShoeLeft));
		LegRight.EquipShoe(GameplayManager.Instance.InstantiateShoe(playerInfo.ShoeRight));
	}

	private void EquipCostumePart(Transform parent, GameObject costume)
	{
		if (costume == null) return;

		var obj = Instantiate(costume, parent, true);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
	}

	public static bool DealDamage(Player attacker, Player receiver, float damage, bool ff = false)
	{
		Debug.Assert(receiver != null);
		if (!ff && attacker != null && attacker.PlayerInfo != null && receiver.PlayerInfo != null)
		{
			if (attacker.PlayerInfo.Team.Equals(receiver.PlayerInfo.Team)) return false;
		}
		
		if (receiver.PlayerInfo != null)
		{
			receiver.PlayerInfo.DamageReceived += damage;
			if (receiver.PlayerInfo.Team != null) receiver.PlayerInfo.Team.DamageReceived += damage;
		}	
		if (attacker != null && attacker.PlayerInfo != null)
		{
			attacker.PlayerInfo.DamageDealt += damage;
			if (attacker.PlayerInfo.Team != null) attacker.PlayerInfo.Team.DamageDealt += damage;
		}
		
		receiver.Hp -= damage;
		receiver.OnDamageTaken.Invoke(attacker, receiver, damage);
		if (attacker != null) attacker.OnDamageDealt.Invoke(attacker, receiver, damage);

		if (receiver.Hp <= 0) Kill(attacker, receiver, ff);
		return true;
	}

	public static bool Kill(Player attacker, Player receiver, bool ff = false)
	{
		Debug.Assert(receiver != null);
		
		if (!ff && attacker != null && attacker.PlayerInfo != null && receiver.PlayerInfo != null)
		{
			if (attacker.PlayerInfo.Team.Equals(receiver.PlayerInfo.Team)) return false;
		}

		if (receiver.PlayerInfo != null)
		{
			receiver.PlayerInfo.Deaths += 1;
			if (receiver.PlayerInfo.Team != null) receiver.PlayerInfo.Team.Deaths += 1;
		}	
		if (attacker != null && attacker.PlayerInfo != null)
		{
			attacker.PlayerInfo.Kills += 1;
			if (attacker.PlayerInfo.Team != null) attacker.PlayerInfo.Team.Kills += 1;
		}
				
		receiver.OnDeath.Invoke(attacker, receiver);
		if (attacker != null) attacker.OnKill.Invoke(attacker, receiver);
		
		receiver.LegLeft.EquipShoe(null);
		receiver.LegRight.EquipShoe(null);
		
		receiver.OnDamageTaken.RemoveAllListeners();
		receiver.OnDeath.RemoveAllListeners();
		Destroy(receiver.transform.root.gameObject);

		return true;
	}
	
	private void Start ()
	{
		//Head.centerOfMass += HeadMassOffset;
		
		LegLeft.Awake();
		LegLeft.Orientation = -1;
		LegLeft.Player = this;
    
		LegRight.Awake();
		LegRight.Orientation = 1;
		LegRight.Player = this;

		
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
#if true
				var theta = leg.Orientation * Vector2.SignedAngle(Vector2.up, legDirWorldSpace);
				// compensate for leg wind-up animation
				theta -= leg.Orientation * leg.AttackRotation * leg.AttackCharge * AttackChargeWindUpArc;
				
				if (theta > -120 && theta <= 45) leg.AttackRotation = leg.Orientation;
				else 							 leg.AttackRotation = -leg.Orientation;
#endif
#if false
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
#endif
			}
			
			if (leg.IsAttackCharging && (!bumper.IsPressed || leg.AttackCharge >= 1))
			{
				if (leg.AttackCharge < AttackChargeThreshold)
				{
					leg.AttackCharge = 0;
					leg.AttackDamage = 0;
				}
				else
				{
					leg.AttackCharge = Mathf.Clamp01(
						(leg.AttackCharge - AttackChargeThreshold) / (1 - AttackChargeThreshold));
					
					var arc = -leg.AttackRotation * Mathf.Lerp(AttackArcMin, AttackArcMax, leg.AttackCharge);
					leg.AttackTargetDirection = Quaternion.AngleAxis(arc, Vector3.forward) * legDirWorldSpace;
				
					leg.AttackDamage = Mathf.Lerp(AttackDamageMin, AttackDamageMax, leg.AttackCharge);									
					leg.AttackRecovery = Mathf.Lerp(AttackRecoveryTimeMin, AttackRecoveryTimeMax, leg.AttackCharge);
				
					leg.AttackButtonHeld = bumper.IsPressed;
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
				}
			}
			else  							// regular movement
			{
				Vector2 wishDir;
				if (leg.IsAttackRecovering)
				{
					wishDir = leg.AttackTargetDirection;
					leg.AttackRecovery -= Time.deltaTime;

					if (leg.AttackRecovery <= 0)
					{
						leg.AttackDamage = 0;
					}
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
		
		var triggerVal  = trigger.Value;
		
		if (leg.HasShoe) {	// shoe abilities		

			if (leg.IsAttacking || leg.IsAttackCharging)
			{
				triggerHeld = false;
				triggerDown = false;
				triggerUp   = bumper.WasPressed;

				triggerVal = 0;
			}

			switch (leg.CurrentShoe.Type)
			{
				case ShoeType.Debug:
					leg.CurrentShoe.transform.localEulerAngles = new Vector3(0, 0, triggerVal * 180);
					break;
				
				
				case ShoeType.Gun:
					Debug.Assert(leg.CurrentShoe is GunShoe);
					var gun = leg.CurrentShoe as GunShoe;

					if (triggerHeld)
					{
						gun.Charge += Time.deltaTime / Mathf.Lerp(gun.ChargeTimeMax, gun.ChargeTimeMin, triggerVal);
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
						    Mathf.Lerp(rocket.BurnTimeMax, rocket.BurnTimeMin, triggerVal);

						var efficiency = Mathf.Clamp01(rocket.Fuel / rocket.FuelEfficiencyFalloff);
						leg.Rigidbody.AddForce(-legDirWorldSpace * efficiency *
							Mathf.Lerp(rocket.ForceMin, rocket.ForceMax, triggerVal));
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
 
// boilerplate classes
public class PlayerDamageEvent : UnityEvent<Player, Player, float> {}

public class PlayerDeathEvent : UnityEvent<Player, Player> {}