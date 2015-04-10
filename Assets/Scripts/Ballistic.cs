using UnityEngine;
using System.Collections;

public class Ballistic : MonoBehaviour, ILeadable {

	public float grav = 9.8f;
	public float dragConst = 0.001f; // breaks physics if greater than about 0.33f
	public float mass = 1f; // wao
	//public float dragExpon = 2f;
	// set to the same thing forever and ever, so evaluating the sqrt and pow just waste computation
	public float lTime = 5f;
	public float timer;

	public Vector3 vel;
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
		Debug.DrawLine (prevPos, transform.position, DebugColors.TrailColor(vel.sqrMagnitude), .3f);

		// point projectile collision
		RaycastHit hit;
		if (Physics.Linecast(prevPos, transform.position, out hit)){
			//Debug.Log("Ballistic hit at: " + hit.point);
			Destroy(this.gameObject);
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

	public static void BallisticLaunch(GameObject projectile, Vector3 velocity){
		Ballistic bal = projectile.GetComponent<Ballistic> ();
		if (!bal) {
			//Debug.Log("Ballistic Launch: added ballistic component");
			bal = projectile.AddComponent<Ballistic>();
		}
		else{
			//Debug.Log("Ballistic Launch: projectile had ballistic component");
		}
		bal.vel = velocity;
	}

	public static float AirDensity(Vector3 position){
		return 1f;
	}

	public float getSqrTerminalVelocity(){
		return mass * grav / dragConst;
	}

	public float getCharacteristicTime(){
		return Mathf.Sqrt(getSqrTerminalVelocity()) / grav;
	}

	public Vector3 getPosition(){
		return transform.position;
	}

	public Vector3 getVelocity(){
		return vel;
	}

	Vector3 getAcceleration(){
		// drag
		Vector3 accel = vel.normalized * -dragConst * vel.sqrMagnitude * AirDensity(transform.position) / mass;
		// gravity
		accel += Vector3.down * grav;
		return accel;
	}

	public void ApplyForce(Vector3 force){
		vel += force / mass;
	}
}
