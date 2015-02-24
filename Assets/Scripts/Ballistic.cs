using UnityEngine;
using System.Collections;

public class Ballistic : MonoBehaviour, ILeadable {

	public float grav = 9.8f;
	public float dragConst = 0.001f; // breaks physics if greater than about 0.33f
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

		EulerPhysicsStep (timescale);

		// debug line
		Debug.DrawLine (prevPos, transform.position, DebugColors.TrailColor(vel.sqrMagnitude), 1f);

		// point projectile collision
		RaycastHit hit;
		if (Physics.Linecast(prevPos, transform.position, out hit)){
			//Debug.Log("Ballistic hit at: " + hit.point);
			Destroy(this.gameObject);
		}

	}

	void EulerPhysicsStep(float timescale){
		// update position and velocity
		prevPos = transform.position;
		
		//vel = ((1 - drag) * vel) + (Vector3.down * (float)(grav * 0.5 *  timescale));
		//transform.position += vel * timescale;

		// this is an odd mix of euler drag and partially integrated gravity
		/*
		transform.position	+= vel * timescale; // newton + euler standard motion
		transform.position	+= vel.normalized * -dragConst * vel.sqrMagnitude * timescale; //Mathf.Pow(vel.magnitude, dragExpon) * timescale; // air drag = velocity^2 * dconst
		transform.position  += Vector3.down * (float)(grav * 0.5 * (timescale * timescale)); // gravity = (1/2)gt^2 // gravity is actually ignorable when drag exists
		vel = (transform.position - prevPos) / timescale; // new velocity
		// this computation should not need to happen...
		*/

		// proper Euler, I think
		vel = vel + getAcceleration () * timescale; // v1 = v0 + a*t
		transform.position += (vel + (getAcceleration() * (float) (0.5 * timescale))) * timescale; // p = p1 + v*dt + .5*a*dt^2

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

	public Vector3 getPosition(){
		return transform.position;
	}

	public Vector3 getVelocity(){
		return vel;
	}

	Vector3 getAcceleration(){
		// drag
		Vector3 accel = vel.normalized * -dragConst * vel.sqrMagnitude;
		// need to remove squares, how to get rid of .normalized? maybe not possible
		// gravity
		accel += Vector3.down * grav;
		return accel;
	}
}
