using UnityEngine;
using System.Collections;

public abstract class GunControl : MonoBehaviour {

	protected Gun gun;

	protected void Start(){
		gun = GetComponent<Gun> ();
		if (!gun) {
			Debug.Log("GunControl: no gun on object " + this.gameObject);
		}
	}

	protected void Update(){
		if (ShouldFire()) {
			Shoot();
		}
	}

	public abstract void Shoot ();
		//gun.FireAt(target);

	public abstract bool ShouldFire ();
		//return (bool)target;

	public float TimeToImpact( Vector3 target ){
		return (target - transform.position).sqrMagnitude / gun.muzzleVel; // badly underapproximation, need to count drag and gravity, return negatives if target unreachable?
	}

	// another function stating aim needed to hit the target, counting drag and gravity (math)
	
	public Vector3 LeadPosition( ILeadable target ){
		return target.getPosition() + (target.getVelocity() * TimeToImpact(target.getPosition()));
	}

	public void DebugShowLead( ILeadable target ){
		Debug.DrawRay (target.getPosition(), target.getVelocity() * 100f, DebugColors.LEADLINE, 2f);
	}
}
