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
		set {
			if (!value)
			{
				Charge = 0;
				GetComponent<Renderer>().material.color = ChargeColor.Evaluate(0);
			
				Attacker.IgnoreCollisions(GetComponent<Collider2D>(), false);
				this.gameObject.layer = LayerMask.NameToLayer("Pickups");
				Attacker = null;
			} else Debug.Assert(false);
		}
	}
	
	public Gradient ChargeColor;

	private void FixedUpdate()
	{
		if (!IsProjectile) return;
		
		var velocity = GetComponent<Rigidbody2D>().velocity;
		if (velocity.magnitude >= ProjectileSpeedThreshold)
		{
			Charge = (velocity.magnitude - ProjectileSpeedMin) / (ProjectileSpeedMax - ProjectileSpeedMin);
#if true
			{	// path tracing
				Color color;
				if      (Charge >= 1) color = Color.black;
				else if (Charge <= 0) color = Color.white;
				else                  color = Color.Lerp(Color.blue, Color.red, Charge);

				Debug.DrawRay(this.transform.position, velocity * Time.deltaTime, color, 1.5f);
			}
#endif
			Charge = Mathf.Clamp01(Charge);
		}
		else IsProjectile = false;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!IsProjectile) return;
		
		var target = other.gameObject.GetComponent<Player>();
		if (target != null)
		{
			var damage = Mathf.Lerp(DamageMin, DamageMax, Charge);
			if (Player.DealDamage(Attacker, target, damage)) IsProjectile = false;
		}
	}

	private void OnValidate()
	{
		Type = ShoeType.Gun;
	}
}