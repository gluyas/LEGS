using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to indicate that an object can be worn as a shoe by a Player
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Shoe : MonoBehaviour
{
	public ShoeType Type;
	
	[NonSerialized] private bool _isEquipped = false;
	public bool IsEquipped
	{
		get { return _isEquipped; }
		set
		{
			if (value)	// set equipped
			{
				GetComponent<Rigidbody2D>().simulated = false;
			}
			else  		// set unequipped
			{
				GetComponent<Rigidbody2D>().simulated = true;
			}
			_isEquipped = value;
		}
	}
}

[Serializable]
public enum ShoeType
{
	Debug,
}