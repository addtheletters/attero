using UnityEngine;
using System.Collections;

public class GunControl : MonoBehaviour {

	public static Vector3 LeadPosition( Vector3 targetPos, Vector3 targetVel, float timeGap ){
		return targetPos + targetVel * timeGap;
	}

	public static float TimeToImpact( float distance, float speed ){
		return distance / speed;
	}


}
