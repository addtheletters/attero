using UnityEngine;
using System.Collections;

public class GunEmplacement : AutoGunControl {

	ILeadable currentTarget;
	public bool targetFound;
	public float range = 30f;

	// change behaviour such that:
	// -takes longer to acquire target
	// -periodic scans for targets
	// -when target acquired, delay before firing starts
	// -when target leaves, not immediate switch target
	// -if target comes back before lock lost, continues shooting immediately
	// -while firing, much increased time between scans for targets


	public float lookRequestedIn;
	public float lookTime = 1f; // delay between scans for targets, could be thought of as time needed to check if target is valid

	public bool firing;
	public float aimTime = 0.3f; // delay between target chosen and beginning of fire on target

	public float targetStickiness = 0.5f; // scale on time to retarget while firing at a target in-range

	// Update is called once per frame
	new void Update () {
		base.Update ();

		if(!targetFound || !TargetInRange(currentTarget)){ // if no current target or target has left
			lookRequestedIn -= Time.deltaTime; // we will look for a new target in lookTime
			if (lookRequestedIn <= 0) { // if need to look for target
				FindTarget(); // retarget
				lookRequestedIn = lookTime; // reset look timer
			}
		}
		else{

		}


		/*

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

		*/
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
			Debug.Log ("GunEmplacement: target found! + " + currentTarget.getPosition() + " at squared range " + minsqrdist);
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
