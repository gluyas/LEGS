using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeelyShoe : Shoe
{
	public float WheelForceMax;
	public float WheelForceExponent;
	public float WheelSpeedMax;
	
	public float WheelRadius;
	public float WheelHopRadius;

	[NonSerialized] public ContactPoint2D? LastContact = null;
	[NonSerialized] public bool IsTouching;

	public PhysicsMaterial2D Material;
	[NonSerialized] public PhysicsMaterial2D OriginalMaterial;
	
	private void OnValidate()
	{
		Type = ShoeType.Heely;
	}
}
