using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint2D))]
public class Leg : MonoBehaviour
{
	[NonSerialized] public HingeJoint2D Hinge;
	[NonSerialized] public Collider2D Collider;

	[NonSerialized] public Vector2 CurrentInputDir = Vector2.down;
	
	// Use this for initialization
	private void Start ()
	{
		Collider = GetComponent<Collider2D>();
		
		Hinge = GetComponent<HingeJoint2D>();
		//Hinge.useMotor = true;
	}

	private void OnValidate()
	{
		Start();
	}
}
