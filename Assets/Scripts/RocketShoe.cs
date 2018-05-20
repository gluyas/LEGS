using System;
using UnityEngine;

public class RocketShoe : Shoe
{
	public float ForceMax;
	public float ForceMin;
	
	public float BurnTimeMax;
	public float BurnTimeMin;
	
	public float RegenerationTime;

	public float FuelEfficiencyFalloff;
	
	public Gradient FuelLevelColor;
	
	[NonSerialized] public float Fuel = 1;

	private void OnValidate()
	{
		Type = ShoeType.Rocket;
		//ForceMin = ForceMax * BurnTimeMin / BurnTimeMax * ForceMinEfficiencyFactor;
	}

}