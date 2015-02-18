using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public float inaccuracy	= 1; // in degrees, forming a cone of possible shots around the aim line
	public float muzzleVel	= 10f;
	public GameObject projectile;


	public GameObject FireAt(Vector3 target){
		return Fire (target - transform.position);
	}

	public GameObject Fire(Vector3 direction){
		// debug aimline

		GameObject fired = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
		Vector3 launchvec = direction.normalized * muzzleVel;
		Vector3 variance = inaccuracy * Random.insideUnitSphere; // unit sphere used to get random vector that in-total rotates at most 'inaccuracy'
		//Debug.Log ("magnitude:"+launchvec.magnitude);
		launchvec = Quaternion.Euler(variance.x, variance.y, variance.z) * launchvec;
		//Debug.Log ("magnitude:"+launchvec.magnitude); // verify that only accuracy has been altered, muzzle velocity is consistent

		Ballistic.BallisticLaunch (fired, launchvec);

		// debug lines
		Debug.DrawRay (transform.position, direction * muzzleVel, Color.blue, 3f);
		Debug.DrawRay (transform.position, launchvec, Color.green, 3f);

		return fired;
	}
}
