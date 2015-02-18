using UnityEngine;
using System.Collections;

public class LinearDrone : ParametricDrone {

	Vector3 startPos;
	public Vector3 endPos; // relative to world origin, not startpos
	public float travelTime;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		if ( endPos == Vector3.zero ) {
			Debug.Log("LinearDrone: endpos is zero?");
			endPos = DefaultEndPos();
		}
	}

	new void FixedUpdate(){
		t += Time.fixedDeltaTime;
		lastPos = transform.position;

		if(t < travelTime){
			ShiftPosition (t);
		}
	}

	private Vector3 DefaultEndPos(){
		return new Vector3(0, 10, 30);
	}

	public override void ShiftPosition(float param){
		transform.position = startPos + (endPos - startPos) * (param / travelTime);
	}
}
