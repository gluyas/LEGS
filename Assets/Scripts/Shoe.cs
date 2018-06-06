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
				Rigidbody.simulated = false;
			}
			else  		// set unequipped
			{
				Rigidbody.simulated = true;
				IdleTime = 0;
			}
			_isEquipped = value;
		}
	}

	[NonSerialized] public float IdleTime;

	protected void FixedUpdate()
	{
		if (IsEquipped || GameplayManager.Instance == null) return;

		IdleTime += Time.deltaTime;
		if (IdleTime >= GameplayManager.Instance.ItemDespawnTime)
		{
			Destroy(gameObject);
		}
	}

	public Rigidbody2D Rigidbody
	{
		get { return GetComponent<Rigidbody2D>(); }
	}

	public static ShoeType NextType(ShoeType prev)
	{
		var count = Enum.GetValues(typeof(ShoeType)).Length;
		return (ShoeType) (((int) prev + 1) % count);
	}
	
	public static ShoeType PrevType(ShoeType succ)
	{
		var count = Enum.GetValues(typeof(ShoeType)).Length;
		return (ShoeType) (((int) succ + count - 1) % count);
	}
}

[Serializable]
public enum ShoeType
{
	Gun,
	Rocket,
	Sticky,
}