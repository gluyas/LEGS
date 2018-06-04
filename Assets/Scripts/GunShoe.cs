using System;
using UnityEngine;

public class GunShoe : Shoe
{
	public float KnockbackForceMax;
	public float KnockbackForceMin;

	public float ChargeTimeMax;
	public float ChargeTimeMin;
	public float ChargeThreshold;
	public float ChargeExponent;
	[NonSerialized] public float Charge;

	public float DamageMax;
	public float DamageMin;
	
	public float ProjectileSpeedMax;
	public float ProjectileSpeedMin;
	public float ProjectileSpeedThreshold;

	[NonSerialized] public Player Attacker;
	public bool IsProjectile
	{
		get { return Attacker != null; }
	}
	
	public Gradient ChargeColor;

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!IsProjectile) return;
		
		var speed = GetComponent<Rigidbody2D>().velocity.magnitude;
		var isProjectile = speed >= ProjectileSpeedThreshold;

		if (isProjectile)
		{
			var target = other.gameObject.GetComponent<Player>();
			if (target != null)
			{
				var damage = Mathf.Lerp(DamageMin, DamageMax, Charge);
				Debug.LogFormat("{0} @ {1}", damage, speed);
				if (Player.DealDamage(Attacker, target, damage)) isProjectile = false;
			}
		}
		if (!isProjectile)
		{
			Charge = 0;
			GetComponent<Renderer>().material.color = ChargeColor.Evaluate(0);
			
			Attacker.IgnoreCollisions(GetComponent<Collider2D>(), false);
			this.gameObject.layer = LayerMask.NameToLayer("Pickups");
			Attacker = null;
		}
	}

	private void OnValidate()
	{
		Type = ShoeType.Gun;
	}
}