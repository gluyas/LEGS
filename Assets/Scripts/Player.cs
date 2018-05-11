using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;

public class Player : MonoBehaviour
{
	private const float LegDeadzoneMagnitude = 0.05f;
	
	private static int _playerCount = 0;
	
	[NonSerialized] public int PlayerId;
	[NonSerialized] public InputDevice Controller;

#if false
	public float HeadTorqueMax;
	public float HeadStabilizerCurve;
	public float HeadTiltThetaMax;
	public Vector2 HeadMassOffset;
#endif
	
	public float LegSpeedMax;
	public float LegTorqueMax;
	public float LegTorqueMin;
	public float LegSmoothAngle = 1;
	
	public Leg LegRight;
	public Leg LegLeft;
	public Rigidbody2D Head	// exposing this to improve future flexibility; ie changing player structure
	{
		get { return this.GetComponent<Rigidbody2D>(); }
	}
	
	private void Start ()
	{
		//Head.centerOfMass += HeadMassOffset;
		
		{	// ignore self collisions
			var collider = Head.GetComponent<Collider2D>();
			Physics2D.IgnoreCollision(collider, LegLeft.Collider);
			Physics2D.IgnoreCollision(collider, LegRight.Collider);
			Physics2D.IgnoreCollision(LegLeft.Collider, LegRight.Collider);
		}
		
		PlayerId = _playerCount++;
		{	// set the controller. currently only works the first time when loaded in a scene
			var controllers = InputManager.Devices.ToList();
			if (PlayerId < controllers.Count)
			{
				Controller = controllers[PlayerId];
			}
			else
			{
				Debug.LogFormat("Player{0} missing controller", PlayerId);
			}
		}
	}
	
	private void FixedUpdate () 
	{
		if (Controller == null) return;
		
		UpdateLeg(LegLeft, Controller.LeftStick);
		UpdateLeg(LegRight, Controller.RightStick);

#if false
		{	// head stabilization
			// angular velocity (omega)
			var omega	    = Head.angularVelocity;
			var omegaTarget = 0;

			var omegaStabilizer = Mathf.Sign(omegaTarget - omega);
			
			// orientation (theta)
			var dirFacing  = Head.transform.up;
			var dirUpright = Vector2.up;
			
			if (Controller.LeftTrigger || Controller.RightTrigger)	// user head tilt
			{
				var rollDir = Controller.LeftTrigger.Value - Controller.RightTrigger.Value;
				dirUpright = Quaternion.Euler(0, 0, rollDir * HeadTiltThetaMax) * dirUpright;
			}
		
			var thetaStabilizer = Vector2.SignedAngle(dirFacing, dirUpright) / 180;
			
			// interpolate between stabilizers to create dynamic equilibrium
			omega = Mathf.Abs(omega);
			omega *= HeadStabilizerCurve;
			Head.AddTorque(HeadTorqueMax * Mathf.Lerp(thetaStabilizer, omegaStabilizer, omega / (omega + 1)));
		}
#endif
	}

	private Vector2 wishDirLeft;
	private Vector2 wishDirRight;
	
	private void UpdateLeg(Leg leg, TwoAxisInputControl input)
	{
		// leg movement
		var inputDir = input.Vector;
		if (inputDir.magnitude > LegDeadzoneMagnitude)
		{
			leg.CurrentInputDir = inputDir.normalized;
		}
		inputDir = leg.CurrentInputDir;

		var legDirWorldSpace = Quaternion.AngleAxis(-leg.Hinge.jointAngle, new Vector3(0, 0, 1)) * -Head.transform.up;
		
		var motor = leg.Hinge.motor;

		var theta = Vector2.SignedAngle(inputDir, legDirWorldSpace);
		var smoothingFactor = Mathf.Clamp01(Mathf.Abs(theta) / LegSmoothAngle);
		
		motor.motorSpeed = LegSpeedMax * Mathf.Sign(theta) * smoothingFactor;
		motor.maxMotorTorque = Mathf.Lerp(LegTorqueMin, LegTorqueMax, smoothingFactor);
		
		leg.Hinge.motor = motor;
#if true
		{	// debug stuff
			Color color;
			if (leg == LegLeft) color = Color.red;
			else 				color = Color.green;
			
			Debug.DrawRay(transform.position, inputDir, Color.Lerp(color, Color.grey, 0.5f));
			Debug.DrawRay(transform.position, legDirWorldSpace, color);
		}
#endif
	}
}
