using UnityEngine;
using System.Collections;

public abstract class GunControl : MonoBehaviour {

	protected Gun theGun;

	protected void Start(){
		theGun = GetComponent<Gun> ();
		if (!theGun) {
			Debug.Log("GunControl: no gun");
		}
	}

	protected void Update(){
		if (shouldFire()) {
			Shoot();
		}
	}

	public abstract void Shoot ();
		//theGun.FireAt(target);

	public abstract bool shouldFire ();
		//return (bool)target;

	public static Vector3 LeadPosition( Vector3 targetPos, Vector3 targetVel, float timeGap ){
		return targetPos + targetVel * timeGap;
	}

	public static float TimeToImpact( float distance, float speed ){
		return distance / speed;
	}

}
