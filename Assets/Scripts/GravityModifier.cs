using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifier : MonoBehaviour
{
	public float GravityFactor;

	[NonSerialized] private Vector2 _gravity;
	[NonSerialized] private GravityModifier _instance;
	
	private void Start()
	{
		if (_instance != null) return;

		_instance = this;
		_gravity = Physics2D.gravity;	
		Physics2D.gravity *= GravityFactor;
	}

	private void OnDestroy()
	{
		if (_instance != this) return;

		Physics2D.gravity = _gravity;
	}
}
