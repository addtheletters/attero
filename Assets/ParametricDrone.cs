using UnityEngine;
using System.Collections;

public abstract class ParametricDrone : MonoBehaviour, ILeadable {

	protected Vector3 lastPos;
	protected float t;

	protected void FixedUpdate () {
		t += Time.fixedDeltaTime;
		lastPos = transform.position;
		ShiftPosition (t);
	}

	public abstract void ShiftPosition (float param);

	public Vector3 getPosition(){
		return transform.position;
	}

	public Vector3 getVelocity(){
		return getPosition() - lastPos;
	}

}
