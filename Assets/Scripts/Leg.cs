using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

	public Vector2 ToePos
	{
		get { return this.transform.TransformPoint(ShoePosOffset); }
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
			switch (CurrentShoe.Type)	// shoe specific unequip logic
			{
				case ShoeType.Gun:
					Debug.Assert(CurrentShoe is GunShoe);
					var gun = CurrentShoe as GunShoe;

					gun.Charge = 0;
					break;
				
				case ShoeType.Sticky:
					Debug.Assert(CurrentShoe is StickyShoe);
					var sticky = CurrentShoe as StickyShoe;

					if (sticky.IgnoreCollider != null)	// ensure that collision is re-enabled
					{
						Physics2D.IgnoreCollision(Player.HeadCollider, sticky.IgnoreCollider, false);
						sticky.IgnoreCollider = null;
					}
					
					Destroy(sticky.HingeInstance);
					break;
			}
			
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
			
			switch (CurrentShoe.Type)
			{				
				case ShoeType.Sticky:
					Debug.Assert(CurrentShoe is StickyShoe);
					var sticky = CurrentShoe as StickyShoe;

					var hinge = gameObject.AddComponent<HingeJoint2D>();
					hinge.enableCollision = false;
					hinge.autoConfigureConnectedAnchor = true;
					hinge.motor = new JointMotor2D
					{
						motorSpeed = 0,
						maxMotorTorque = sticky.HingeResistance,
					};
					hinge.useMotor = true;
					
					sticky.HingeInstance = hinge;
					hinge.enabled = false;
					break;
			}
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

	private void OnCollisionStay2D(Collision2D other)
	{
		if (CurrentShoe != null) switch (CurrentShoe.Type)
		{
			case ShoeType.Sticky:
				Debug.Assert(CurrentShoe is StickyShoe);
				var sticky = CurrentShoe as StickyShoe;

				if (sticky.TryStick && !sticky.IsStuck)
				{
					var toePos = ToePos;
					var nearest = other.contacts
						.OrderBy(p => Vector2.Distance(toePos, p.point))
						.First();
					
					if (Vector2.Distance(toePos, nearest.point) < sticky.StickRadius)
					{
						var hinge = sticky.HingeInstance;
						hinge.connectedBody = nearest.rigidbody;
						hinge.anchor = transform.InverseTransformPoint(nearest.point);
						hinge.enabled = true;

						sticky.IgnoreCollider = nearest.collider;
						Physics2D.IgnoreCollision(Player.HeadCollider, nearest.collider);
					}
				}
				break;
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
