using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDamageZone : MonoBehaviour
{
	public bool Kill;
	public float Damage;
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		var player = other.GetComponent<Player>();
		if (player != null)
		{
			if (Kill) Player.Kill(null, player);
			else      Player.DealDamage(null, player, Damage);
		}
	}
}
