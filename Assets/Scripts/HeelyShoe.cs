using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeelyShoe : Shoe
{
	public float WheelRadius;
	
	public float MaxForce;
	
	[NonSerialized] public bool TryRoll;
	
	private void OnValidate()
	{
		Type = ShoeType.Heely;
	}
}
