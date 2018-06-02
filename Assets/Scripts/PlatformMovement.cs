using UnityEngine;
using System;
using System.Collections;

public class PlatformMovement : MonoBehaviour 
{
	[NonSerialized] public float direction;
	public float moveSpeed = 6f;
	public float lifeTime;

	// Update is called once per frame
	private void FixedUpdate() {
		this.transform.position += new Vector3(direction * moveSpeed * Time.deltaTime, 0);

		this.lifeTime -= Time.deltaTime;
		if (lifeTime <= 0) {
			Destroy(this.gameObject);
		}
		Debug.Log (transform.position);
	}
}
