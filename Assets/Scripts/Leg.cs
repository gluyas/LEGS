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
	public float Damage;
	
	public Vector2 ShoePosOffset;
	public Shoe CurrentShoe;

	public bool HasShoe
	{
		get { return CurrentShoe != null; }
	}
	
	[NonSerialized] public HingeJoint2D Hinge;
	[NonSerialized] public Collider2D Collider;
	[NonSerialized] public Rigidbody2D Rigidbody;

	[NonSerialized] public Vector2 LastInputDirection = Vector2.down;
	[NonSerialized] public Vector2 AttackTargetDirection = Vector2.down;

	[NonSerialized] public bool AttackButtonHeld;
	[NonSerialized] public float AttackCharge;
	[NonSerialized] public float AttackDamage;
	[NonSerialized] public float AttackRotation;
	public bool IsAttacking
	{
		get { return AttackDamage > 0; }
	}
	public bool IsAttackCharging
	{
		get { return AttackCharge > 0 && !IsAttacking; }
	}
	
	[NonSerialized] public float AttackRecovery;
	public bool IsAttackRecovering
	{
		get { return AttackRecovery > 0; }
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
			CurrentShoe.transform.parent = this.transform;
			CurrentShoe.transform.localPosition = ShoePosOffset;
			CurrentShoe.transform.localRotation = Quaternion.identity;
			CurrentShoe.IsEquipped = true;
		}
	}
	
	private void OnCollisionEnter2D(Collision2D other)
	{
		var player = other.gameObject.GetComponent<Player>();
		if (player != null) {
			player.Hp -= Damage;
		}
	}
	
	// Use this for initialization
	private void Awake()
	{
		if (CurrentShoe != null) EquipShoe(CurrentShoe);	// ensure attached shoe is correctly set up
		
		Collider = GetComponent<Collider2D>();
		Hinge = GetComponent<HingeJoint2D>();
		Rigidbody = GetComponent<Rigidbody2D>();
	}
}
