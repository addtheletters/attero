using UnityEngine;
using System.Collections;

public class GunEmplacement : AutoGunControl {

	ILeadable currentTarget;
	public bool targetFound;
	public float range = 30f;

	public float lookRequestedIn;
	public float lookTime = 1f; // delay between scans for targets, could be thought of as time needed to check if target is valid

	// Update is called once per frame
	new void Update () {
		base.Update ();

		if(targetFound){ // if we have a target
			// make sure it hasn't gone out of range
			if(!TargetInRange(currentTarget)){
				currentTarget = null;
				targetFound = false;
				//Debug.Log ("GunEmplacement: target left range");
			}
		}

		lookRequestedIn -= Time.deltaTime; // tick down look timer
		if (lookRequestedIn <= 0) { // if need to look for target
			FindTarget(); // retarget
			lookRequestedIn = lookTime; // reset look timer
		}
		// retargets occur regardless of whether we already have a target or not, this may change
	}


	void FindTarget(){
		Collider[] close = Physics.OverlapSphere (transform.position, range);
		GameObject closest = default(GameObject);
		float minsqrdist = float.MaxValue;
		for(int i = 0; i < close.Length; i++){
			if(close[i].GetComponent(typeof(ILeadable)) == null){ // make this more efficient sometime?
				continue;
			}
			float sqrdist = (close[i].gameObject.transform.position - transform.position).sqrMagnitude;
			if(sqrdist < minsqrdist){
				closest = close[i].gameObject;
				minsqrdist = sqrdist;
			}
		}
		if (closest == default(GameObject)) {
			//Debug.Log("GunEmplacement: found no target within range");
		}
		else{
			targetFound = true;
			currentTarget = (ILeadable)closest.GetComponent(typeof(ILeadable));
			Debug.Log ("GunEmplacement: target found! + " + currentTarget.getPosition() + " at range " + minsqrdist);
		}
	}

	bool TargetInRange( ILeadable target ){
		return (target.getPosition () - transform.position).sqrMagnitude < (range * range); 
	}

	public override void Shoot(){
		//theGun.FireAt(currentTarget.getPosition());
		//DebugShowLead (currentTarget);
		theGun.FireAt (LeadPosition(currentTarget));
	}

	public override bool shouldFire(){
		return targetFound;
	}

}
