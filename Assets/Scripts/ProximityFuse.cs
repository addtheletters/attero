using UnityEngine;
using System.Collections;

public class ProximityFuse : Fuse {
	private bool EXTEND_WITH_VELOCITY = true;

	public float range;
	private ILeadable myLeadable;

	LayerMask proximityMask;

	// possible extension: IFFProcimityFuse, capable of designating one specific target or a faction to target,
	// will not detonate if too close to friendlies unless set to do so

	new void Start(){
		base.Start();
		proximityMask = -1;

		if(EXTEND_WITH_VELOCITY){
			myLeadable = this.GetComponent<ILeadable>();
		}
		if(myLeadable == null){
			Debug.Log ("ProximityFuse: No leadable component...");
			EXTEND_WITH_VELOCITY = false;
		}
		// if velocity, check if has velocity
	}

	public override bool ShouldDetonate(){
		// TODO layermask filter
		// TODO velocity added
		RaycastHit hit;
		if(EXTEND_WITH_VELOCITY){
			return Physics.SphereCast(transform.position, range, myLeadable.getVelocity(), out hit, proximityMask);
		}
		else{
			return Physics.CheckSphere(transform.position, range, proximityMask);
		}
	}
}
