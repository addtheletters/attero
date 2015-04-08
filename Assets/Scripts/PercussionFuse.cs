using UnityEngine;
using System.Collections;

public class PercussionFuse : Fuse {

	public bool hasImpacted = false;

	public float impact_detonation_threshold = 10f;
	// int collisionMask
	// layermask should be handled by the collider 

	void OnCollisionEnter(Collision coll){
		float speed = coll.relativeVelocity.magnitude;
		// if it hits hard enough, set to blow up next frame
		if (speed > impact_detonation_threshold) {
			hasImpacted = true;
		}
	}

	public override bool ShouldDetonate(){
		return hasImpacted;
	}
}
