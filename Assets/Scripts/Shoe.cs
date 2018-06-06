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
				GetComponent<Renderer>().enabled = true;
				_idleTime = 0;
				_numFlickers = 1;
			}
			else  		// set unequipped
			{
				Rigidbody.simulated = true;
			}
			_isEquipped = value;
		}
	}

	[NonSerialized] private float _idleTime;
	[NonSerialized] private int _numFlickers = 1;

	protected void FixedUpdate()
	{
		if (IsEquipped || GameplayManager.Instance == null) return;

		var renderer = GetComponent<Renderer>();
		
		_idleTime += Time.deltaTime;
		if (_idleTime >= GameplayManager.Instance.ItemDespawnTime - GameplayManager.Instance.ItemDespawnFlickerTime)
		{
			renderer.enabled = !renderer.enabled;
		} 
		else if (_idleTime >= GameplayManager.Instance.ItemDespawnTime * _numFlickers / (_numFlickers + 1))
		{
			_numFlickers += 1;
			StartCoroutine(FlickerOnce(renderer));
		}
		
		if (_idleTime >= GameplayManager.Instance.ItemDespawnTime)
		{
			Destroy(gameObject);
		}
	}

	private static IEnumerator FlickerOnce(Renderer renderer)
	{
		renderer.enabled = false;
		yield return new WaitForSeconds(GameplayManager.Instance.ItemDespawnFlickerDuration);
		
		renderer.enabled = true;
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