using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class LegMovement : MonoBehaviour
{

	//INCONTROL
	[SerializeField] public int playerNum;
	[SerializeField] public bool leftLeg;
	public InputDevice joystick;

	//public Transform target;
	//public bool isButton = true;
	public BoxCollider2D otherleg;
	public BoxCollider2D currentleg;
	public float interp;
//	public string HorizontalAxis;
//	public string VerticalAxis;
	public Rigidbody2D rb;
	public HingeJoint2D hj;
	JointMotor2D motor;

	float z;

	void Start ()
	{
		Physics2D.IgnoreCollision (otherleg, currentleg);
		
		motor = new JointMotor2D();
		motor.maxMotorTorque = 1000;
		rb = GetComponent<Rigidbody2D> ();

		//INCONTROL
		if(InputManager.Devices[playerNum] != null){
			joystick = InputManager.Devices[playerNum];
		}
		else
			Debug.Log("Player " + playerNum + "'s controller doesn't exist!");
	}

	void Update ()
	{
		Debug.DrawRay (this.transform.position, -transform.up, Color.red);
		Vector3 inputDirection;
		if (leftLeg)
			inputDirection = new Vector3 (joystick.LeftStickX, joystick.LeftStickY, 0);
		else
			inputDirection = new Vector3 (joystick.RightStickX, joystick.RightStickY, 0);		

		float angle = Vector3.Angle (-transform.up, inputDirection);
		//angle = angle * Mathf.Rad2Deg;
		//Debug.Log (angle)

		float dot = Vector3.Dot (-transform.right, inputDirection);
		//Debug.Log (dot);
		if (dot < 0) {
			angle = 360 - angle;
		}

		float theNameThatExistsWithinYourHeart = 360 - Mathf.Abs(angle);
		//Debug.Log (angle - theNameThatExistsWithinYourHeart);
		//Debug.Log (theNameThatExistsWithinYourHeart + Mathf.Abs(angle));
		if (angle - theNameThatExistsWithinYourHeart < 0) {
			z = Mathf.LerpAngle (transform.rotation.z, theNameThatExistsWithinYourHeart * Mathf.Deg2Rad, interp);
		} else {
			z = Mathf.LerpAngle (transform.rotation.z, angle * Mathf.Deg2Rad, interp);
		}

		//Debug.Log (angle);

		if(inputDirection.x < 0.1f &&  inputDirection.x > -0.1f && inputDirection.y < 0.1f && inputDirection.y > -0.1f && inputDirection.z < 0.1f && inputDirection.z > -0.1f){
			z = 0;
			motor.motorSpeed = 0;
			hj.motor = motor;
		}


		if (angle > 20f) {
			if (dot > 0) {
				angle = angle + 20f;
				motor.motorSpeed = 1000;
				hj.motor = motor;
			} else if (dot < 0) {
				angle = angle - 20f;
				motor.motorSpeed = -1000;
				hj.motor = motor;
			}
		} else {
			motor.motorSpeed = 0;
			hj.motor = motor;
		}
			
		//transform.rotation = Quaternion.Euler (0, 0, z * Mathf.Rad2Deg);

		//transform.Rotate(inputDirection * 30 * Time.deltaTime);

		//transform.Rotate (inputDirection, Space.Self);

		//rb.AddForce(inputDirection  * 30 * Time.deltaTime);
	
	
	}
}