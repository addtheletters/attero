﻿using UnityEngine;
using System.Collections;

public class Ballistic : MonoBehaviour, ILeadable {

	public BallisticProfile profile = BallisticProfile.standard;

	public float lTime = 5f;
	public float timer;

	Vector3 vel;
	Vector3 prevPos;
	
	void FixedUpdate () {
		// using fixed so physics predictions are more likely accurate
		float timescale = Time.fixedDeltaTime;

		// projectile life, kill if life has expired
		timer += timescale;
		if (timer > lTime) {
			Destroy(this.gameObject);
		}

		VerletPhysicsStep (timescale);

		// debug line
		Debug.DrawLine (prevPos, transform.position, DebugUtil.Colors.TrailColor(vel.sqrMagnitude), .3f);

		// point projectile collision
		RaycastHit hit;
		if (Physics.Linecast(prevPos, transform.position, out hit)){
			//Debug.Log("Ballistic hit at: " + hit.point);
			BallisticHit(hit);
		}

	}

	void EulerPhysicsStep(float timescale){
		prevPos = transform.position;
		transform.position += vel * timescale;
		vel += getAcceleration() * timescale;
	}

	void VerletPhysicsStep(float timescale){
		prevPos = transform.position;
		//http://en.wikipedia.org/wiki/Verlet_integration#Velocity_Verlet
		Vector3 accel		= getAcceleration ();
		transform.position	+= (vel + 0.5f * accel * timescale) * timescale; // (vel * timescale) + (accel * 0.5f * timescale * timescale); // p = p1 + v*dt + .5*a*dt^2 // (not quite euler)
		Vector3 aTPlus		= 2f * (transform.position - prevPos - (vel * timescale)); // future frame acceleration
		vel 				+= 0.5f * (accel + aTPlus) * timescale ;
	}

	void BallisticHit(RaycastHit hit){
		// yay this can be overloaded and stuff
		BallisticReporter br = GetComponent<BallisticReporter>();
		if(br != default(BallisticReporter)){
			br.Report(true);
		}
		Destroy(this.gameObject);
	}

	// Warning: Ballistic launches override the previous velocity and set it to a new one.
	public static void BallisticForceLaunch(GameObject projectile, Vector3 direction, float force){
		Ballistic bal = projectile.GetComponent<Ballistic> ();
		if (!bal) {
			Debug.Log("Ballistic Launch: added ballistic component to object lacking it");
			bal = projectile.AddComponent<Ballistic>();
		}
		bal.ApplyForce( direction.normalized * force );
	}

	public static void BallisticLaunch(GameObject projectile, Vector3 velocity){
		Ballistic bal = projectile.GetComponent<Ballistic> ();
		if (!bal) {
			Debug.Log("Ballistic Launch: added ballistic component to object lacking it");
			bal = projectile.AddComponent<Ballistic>();
		}
		BallisticForceLaunch(projectile, velocity, velocity.magnitude * bal.profile.Mass);
	}

	public static float AirDensity(Vector3 position){
		return 1f;
	}

	public float getSqrTerminalVelocity(){
		return profile.Mass * BallisticProfile.Gravity / profile.Drag;
	}

	public float getCharacteristicTime(){
		return Mathf.Sqrt(getSqrTerminalVelocity()) / BallisticProfile.Gravity;
	}

	public Vector3 getPosition(){
		return transform.position;
	}

	public Vector3 getVelocity(){
		return vel;
	}

	Vector3 getAcceleration(){
		// drag
		Vector3 accel = vel.normalized * -profile.Drag * vel.sqrMagnitude * AirDensity(transform.position) / profile.Mass;
		// gravity
		accel += Vector3.down * BallisticProfile.Gravity;
		return accel;
	}

	public void ApplyForce(Vector3 force){
		vel += force / profile.Mass;
	}
}
