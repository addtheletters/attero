using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public float inaccuracy	= 1; // in degrees, forming a cone of possible shots around the aim line
	public float muzzleVel	= 300f;
	public GameObject projectile;

	private bool provideGunInfo;

	float shotsFired = 0;

	void Start(){
		if(projectile.GetComponent<BallisticReporter>() == null){
			provideGunInfo = false;
		}
		else{
			provideGunInfo = true;
		}
	}

	public GameObject FireAt(Vector3 target){
		return Fire (target - transform.position);
	}

	public GameObject Fire(Vector3 direction){
		GameObject fired = (GameObject)Instantiate(projectile, transform.position, transform.rotation);
		Vector3 launchvec = direction.normalized * muzzleVel;
		Vector3 variance = inaccuracy * Random.insideUnitSphere; // unit sphere used to get random vector that in-total rotates at most 'inaccuracy'
		//Debug.Log ("magnitude:"+launchvec.magnitude);
		launchvec = Quaternion.Euler(variance.x, variance.y, variance.z) * launchvec;
		//Debug.Log ("magnitude:"+launchvec.magnitude); // verify that only accuracy has been altered, muzzle velocity is consistent

		Ballistic.BallisticLaunch (fired, launchvec);

		// debug lines
		Debug.DrawRay (transform.position, (direction * muzzleVel).normalized * 3f, DebugColors.AIMLINE, 1f);
		Debug.DrawRay (transform.position, launchvec.normalized * 5f, DebugColors.FIRELINE, 1f);

		shotsFired ++;

		if(provideGunInfo){
			((BallisticReporter)fired.GetComponent<BallisticReporter>()).Bsi = new BallisticShotInfo(BallisticShotInfo.GetAngleFor(direction),muzzleVel);
		}
		return fired;
	}
}
