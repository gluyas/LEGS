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

	public Gradient ChargeColor;
	
	public float ProjectileSpeedMax;
	public float ProjectileSpeedMin;

	[NonSerialized] public bool IsProjectile = false;
	
	private void OnValidate()
	{
		Type = ShoeType.Gun;
	}
}