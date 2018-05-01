using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour {

	public float hit = 10;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnCollisionEnter2D(Collision2D col){
		Debug.Log("hi");
		if(col.collider.gameObject.tag == "Player"){
			col.gameObject.BroadcastMessage("ApplyDamage", hit);
		}
	}
}
