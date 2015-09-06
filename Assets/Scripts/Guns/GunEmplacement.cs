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
	// need to alter to integrate aimTime and targetStickiness
	public bool firing;
	
	public float aimTime = 0.3f; // delay between target chosen and beginning of fire on target
	public float readyToFireIn;  // is set to aimTime, can fire when <= 0
	public float refindTargetTime = 0.3f;   // how long the target needs to be gone for before a new target is acquired
	public float targetMissingFor;			// how long has the target been unseen or out of range
	
	int obscurementMask;

	new void Start(){
		base.Start ();
		obscurementMask = (1 << LayerMask.NameToLayer("Obscurement")) | (1 << LayerMask.NameToLayer("Ground")); // obscurement layer
	}

	// Update is called once per frame
	new void Update () {
		base.Update ();

		/*
		if (lookRequestedIn <= 0) { // if need to look for target
			targetFound = false;
			currentTarget = null;
			FindTarget(); // retarget
			lookRequestedIn = lookTime; // reset look timer
		}
		if (!TargetInRange (currentTarget)) {
			firing = false;
			if (!targetFound) {
				// if no current target or target has left
				lookRequestedIn -= Time.deltaTime; // we will look for a new target in lookTime
			}
		}
		else{
			firing = true;
		}
		*/

		if(targetFound){ // if we have a target
			// make sure it hasn't gone out of range
			if(!TargetInSight(currentTarget)){
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

	void RedoUpdate(){
		base.Update ();
		if (targetFound) {
			if(readyToFireIn > 0){
				readyToFireIn -= Time.deltaTime;
			}
			if(TargetInSight(currentTarget)){
				targetMissingFor = 0;
			}
			else{
				targetMissingFor += Time.deltaTime;
				if(targetMissingFor > refindTargetTime){

				}
			}
		}
		else{

		}
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
			// need to fix to account for target sightline loss
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
			//Debug.Log ("GunEmplacement: target found! + " + currentTarget.getPosition() + " at squared range " + minsqrdist);
		}
	}

	ILeadable RedoFindTarget(){
		Collider[] close			= Physics.OverlapSphere (transform.position, range);
		ILeadable closestVisible	= default(ILeadable);
		float minRankVal 			= float.MaxValue;
		for(int i = 0; i < close.Length; i++){
			ILeadable tempLeadable = (ILeadable)close[i].GetComponent(typeof(ILeadable));
			if(tempLeadable == null || ViewToObstructed(tempLeadable)){
				continue;
			}
			float targetRank = GetTargetRank(tempLeadable);
			if(targetRank < minRankVal){
				closestVisible	= tempLeadable;
				minRankVal		= targetRank;
			}
		}
		if (closestVisible == default(ILeadable)) {
			//Debug.Log("GunEmplacement: found no visible target within range");
			return null; 
		}
		else{
			Debug.Log ("GunEmplacement: target found! + " + currentTarget.getPosition() + " with target rank " + minRankVal);
			return closestVisible;
		}
	}

	float GetTargetRank(ILeadable target){
		// returns a value which gauges the target priority, based on what kind of
		// target it is and how easy it is to hit
		// right now just returns a distance squared
		return (target.getPosition () - transform.position).sqrMagnitude;
	}

	void SetTarget(ILeadable target){
		currentTarget = target;
		targetFound	  = true;
		readyToFireIn = aimTime;
	}

	bool TargetInSight( ILeadable target ){
		if (!TargetInRange (target)) {
			return false;
		}
		return ViewToObstructed (target);
	}

	bool ViewToObstructed(ILeadable target){
		RaycastHit hit;
		bool castResult = Physics.Raycast (transform.position, target.getPosition() - transform.position, out hit, range, obscurementMask);
		//Debug.Log ("GunEmplacement: cast to target status is " + !castResult);
		if (castResult) {
			//Debug.Log(hit.collider.gameObject);
		}
		return !castResult;
	}

	bool TargetInRange( ILeadable target ){
		if (target == null) {
			return false; // this behavior may change
		}
		return (target.getPosition () - transform.position).sqrMagnitude < (range * range); 
	}

	public override void Shoot(){
		//gun.FireAt(currentTarget.getPosition());
		//DebugShowLead (currentTarget);
		gun.FireAt (LeadPosition(currentTarget));
	}

	public override bool ShouldFire(){
		return targetFound && TargetInSight(currentTarget);
	}

}
