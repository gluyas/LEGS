using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using InControl;

public class Body : MonoBehaviour {
	
	public float health = 50;
	private float initHealth;
	
	public Rigidbody2D rb;
	public Text win;
	public GameObject text;
	
	public Text display;
	public string buttonName;
	
	// Leg Movement script to retrieve INCONTROL inputs
	private LegMovement legScript;

	void Start () {
		text.SetActive(false);
		initHealth = health;
		
		//INCONTROL Grabbing leg child for INCONTROL inputs
		legScript = this.gameObject.transform.GetChild(0).GetComponent<LegMovement>();
	}
	
	void Update () {
		float percentage = (health* 100)/initHealth;
		
		if(health <= 0){
			Die();
			percentage = 0;
		}
		

		
		display.text = "" + percentage + "%";
		
		if(legScript.joystick.Action1){
			Restart();
		}
	}
	
	void ApplyDamage (int damage){
		health -= damage;
	}
	
	void Die(){
		//Destroy(gameObject);
		rb.mass = 100;
		rb.freezeRotation = false;
		//gameObject.BroadcastMessage
		
		if(gameObject.name == "PlayerOne"){
			win.text = "Player Two Wins!";
			text.SetActive(true);
			
		} else if(gameObject.name == "PlayerTwo"){
			win.text = "Player One Wins!";
			text.SetActive(true);
		}
	}
	
	void Restart(){
		 SceneManager.LoadScene( SceneManager.GetActiveScene().name );
	}
}
