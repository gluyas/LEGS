using System.Collections;
using System.Collections.Generic;
using System.Collections; 
using UnityEngine;

public class platformSpawner : MonoBehaviour {

	public GameObject platformPrefab;
	public float spawnTimeMax = 15;
	public float spawnTimeMin = 12;
	public float platformMoveDirection;

	void Start()
	{
		StartCoroutine(MakePlatform());
	}

	IEnumerator MakePlatform() {
		var platform = Instantiate(platformPrefab, this.transform);
		platform.GetComponent<PlatformMovement>().direction = platformMoveDirection;

		yield return new WaitForSeconds(Random.Range (spawnTimeMin, spawnTimeMax));

		StartCoroutine(MakePlatform ());
	}
}