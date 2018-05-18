﻿using System;
using UnityEngine;

public class RocketShoe : Shoe
{
	public float ForceMax;
	public float ForceMin;
	//[NonSerialized] public float fuel;
	
	public float BurnTimeMax;
	public float BurnTimeMin;
	
	public float RegenerationTime;

	public float FuelEfficiencyFalloff;
	
	public Gradient FuelLevelColor;
	
	[NonSerialized] public float Fuel = 1;

	private void FixedUpdate()
	{
		if (!IsEquipped)
		{
			Fuel += Time.deltaTime / RegenerationTime;
			Fuel = Mathf.Clamp01(Fuel);
			GetComponent<Renderer>().material.color = FuelLevelColor.Evaluate(Fuel);
		}
	}

	private void OnValidate()
	{
		Type = ShoeType.Rocket;
	}

}