using System;
using UnityEngine;

public class RocketShoe : Shoe
{
	public float ForceMax;
	public float ForceMinEfficiencyFactor;
	public float ForceMin;
	
	public float BurnTimeMax;
	public float BurnTimeMin;
	public float BurnRateExponent;
	
	public float RegenerationTime;
	public float RegenerationDelay;

	public Gradient FuelLevelColor;
	
	[NonSerialized] public float Fuel = 1;
	[NonSerialized] public float Delay = 0;
		
	private void OnValidate()
	{
		Type = ShoeType.Rocket;
		ForceMin = ForceMax * BurnTimeMin / BurnTimeMax * ForceMinEfficiencyFactor;
	}

}