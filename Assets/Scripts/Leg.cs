using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	
	[NonSerialized] public HingeJoint2D Hinge;
	[NonSerialized] public Collider2D Collider;

	[NonSerialized] public Vector2 CurrentInputDir = Vector2.down;

	public void EquipShoe(Shoe newShoe)
	{
		if (newShoe.IsEquipped)
		{
			Debug.LogAssertion("attempted to equip already equipped shoe");
			return;
		}
			
		if (CurrentShoe != null)	// swap old shoe with new one
		{
			CurrentShoe.transform.parent = null;
			CurrentShoe.transform.position = newShoe.transform.position;
			CurrentShoe.transform.rotation = newShoe.transform.rotation;
			CurrentShoe.IsEquipped = false;
		}

		CurrentShoe = newShoe;
		CurrentShoe.transform.parent = this.transform;
		CurrentShoe.transform.localPosition = ShoePosOffset;
		CurrentShoe.transform.localRotation = Quaternion.identity;
		CurrentShoe.IsEquipped = true;
	}
	
	// Use this for initialization
	private void Start ()
	{
		if (CurrentShoe != null) EquipShoe(CurrentShoe);	// ensure attached shoe is correctly set up
		
		Collider = GetComponent<Collider2D>();
		
		Hinge = GetComponent<HingeJoint2D>();
		//Hinge.useMotor = true;
	}

	private void OnValidate()
	{
		Start();
	}
}
