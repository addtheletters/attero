using UnityEngine;
using System.Collections;

public class Ballistic : MonoBehaviour, ILeadable {

	public float grav = 9.8f;
	public float drag = 0.001f;

	public float lTime = 5f;
	public float timer;

	public Vector3 vel;
	Vector3 prevPos;

	
	void FixedUpdate () {
		// using fixed so physics predictions are more likely accurate

		// projectile life, kill if life has expired
		timer += Time.fixedDeltaTime;
		if (timer > lTime) {
			Destroy(this.gameObject);
		}

		// update position and velocity
		prevPos = transform.position;

		//vel = ((1 - drag) * vel) + (Vector3.down * (float)(grav * 0.5 *  Time.fixedDeltaTime));
		//transform.position += vel * Time.fixedDeltaTime;

		transform.position	+= (1) * vel * Time.fixedDeltaTime; // newton + euler standard motion
		transform.position	+= vel.normalized * -drag * vel.sqrMagnitude * Time.fixedDeltaTime; // air drag = velocity^2 * dconst
		transform.position  += Vector3.down * (float)(grav * 0.5 * (Time.fixedDeltaTime * Time.fixedDeltaTime)); // gravity = (1/2)gt^2
		vel = (transform.position - prevPos) / Time.fixedDeltaTime; // new velocity


		// debug line
		Debug.DrawLine (prevPos, transform.position, DebugColors.TrailColor(vel.sqrMagnitude), 1f);

		// point projectile collision
		RaycastHit hit;
		if (Physics.Linecast(prevPos, transform.position, out hit)){
			//Debug.Log("Ballistic hit at: " + hit.point);
			Destroy(this.gameObject);
		}

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
}
