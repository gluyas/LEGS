using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Leg class should be used more as a container class for leg-specfic details
/// Related gameplay code should be kept in the Player class where possible.
/// </summary>
[RequireComponent(typeof(HingeJoint2D))]
public class Leg : MonoBehaviour
{
	public Vector2 ShoePosOffset;
	public Shoe CurrentShoe;

	public bool HasShoe
	{
		get { return CurrentShoe != null; }
	}
	
	[NonSerialized] public HingeJoint2D Hinge;
	[NonSerialized] public Collider2D Collider;
	[NonSerialized] public Rigidbody2D Rigidbody;
	[NonSerialized] public Player Player;

	[NonSerialized] public float Orientation;
	
	[NonSerialized] public Vector2 LastInputDirection = Vector2.down;
	[NonSerialized] public Vector2 AttackTargetDirection = Vector2.down;

	[NonSerialized] public bool AttackButtonHeld;
	[NonSerialized] public float AttackCharge;
	[NonSerialized] public float AttackDamage;
	[NonSerialized] public float AttackRotation = 1;
	public bool IsAttacking
	{
		get { return AttackDamage > 0 && AttackCharge > 0; }
	}
	public bool IsAttackCharging
	{
		get { return AttackCharge > 0 && !IsAttacking; }
	}
	
	[NonSerialized] public float AttackRecovery;
	public bool IsAttackRecovering
	{
		get { return AttackRecovery > 0 && AttackCharge <= 0; }
	}
	
	[NonSerialized] public bool TryEquip;

	public void EquipShoe(Shoe newShoe)
	{
		if (CurrentShoe == newShoe) return;
		if (newShoe != null && newShoe.IsEquipped)
		{
			Debug.LogAssertion("attempted to equip already equipped shoe");
			return;
		}
			
		if (CurrentShoe != null)	// swap old shoe with new one
		{
			CurrentShoe.transform.parent = null;
			if (newShoe != null)
			{
				CurrentShoe.transform.position = newShoe.transform.position;
				CurrentShoe.transform.rotation = newShoe.transform.rotation;
			}
			CurrentShoe.IsEquipped = false;
		}

		CurrentShoe = newShoe;
		if (CurrentShoe != null)
		{
			// check shoe orientation
			if (CurrentShoe.transform.localScale.x * Orientation < 0)
			{
				var scale = CurrentShoe.transform.localScale;
				scale.x *= -1;
				CurrentShoe.transform.localScale = scale;
			}			
			CurrentShoe.transform.parent = this.transform;
			
			var offset = ShoePosOffset;
			offset.x *= Orientation;
			CurrentShoe.transform.localPosition = offset;
			
			CurrentShoe.transform.localRotation = Quaternion.identity;		
	
			CurrentShoe.IsEquipped = true;
		}
	}
	
	private void OnCollisionEnter2D(Collision2D other)
	{
		var player = other.gameObject.GetComponent<Player>();
		if (player != null)
		{
			if (Player.DealDamage(this.Player, player, AttackDamage)) AttackDamage = 0;
		}
	}
	
	// Use this for initialization
	public void Awake()
	{
		if (CurrentShoe != null && !CurrentShoe.IsEquipped) EquipShoe(CurrentShoe);
		
		Collider = GetComponent<Collider2D>();
		Hinge = GetComponent<HingeJoint2D>();
		Rigidbody = GetComponent<Rigidbody2D>();
	}
}
