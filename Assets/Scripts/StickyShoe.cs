using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyShoe : Shoe
{
	public float StickRadius;
	public float HingeResistance; 
	
	[NonSerialized] public bool TryStick;
	public bool IsStuck
	{
		get { return HingeInstance.enabled; }
	}
	[NonSerialized] public HingeJoint2D HingeInstance;
	
	private void OnValidate()
	{
		Type = ShoeType.Sticky;
	}
}
