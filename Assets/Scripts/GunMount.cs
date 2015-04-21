using UnityEngine;
using System.Collections;
using System;

public class GunMount : MonoBehaviour {

	// gun aim speed
	public HorizontalCoords traverse = new HorizontalCoords(60f, 60f);

	// altitude restrictions
	public float maxElevation  = 90;	// how far above the horizon
	public float maxDepression = 0;		// how far below the horizon

	// azimuth (horizontal) restrictions.
	public float maxAzimuthVariance = 90; // how far to the left and right of center
	// 90 will allow gun to traverse a 180 degree arc

	// is the gun changing its aim point?
	public bool aiming;

	// current aim angle
	public HorizontalCoords currentAim;
	public HorizontalCoords targetAim;

	// Use this for initialization
	void Start () {
		if(maxElevation < 0 || maxElevation > 180)
			Debug.Log ("GunMount: Elevation limit of " + maxElevation + " should be positive and <= 180.");
		if(maxDepression < 0 || maxDepression > 180)
			Debug.Log ("GunMount: Depression limit of " + maxDepression + " should be positive and <= 180.");
		if(maxAzimuthVariance < 0 || maxAzimuthVariance > 180)
			Debug.Log ("GunMount: Azimuth limits of " + maxAzimuthVariance + " should be positive and <= 180.");
	}
	
	// Update is called once per frame
	void Update () {
		ShowDebugAimline();
		if(aiming){
			MoveAimTowards(targetAim);
			//Debug.Log ("reaiming");
			if( currentAim.IsApproximately( targetAim ) ){
				//Debug.Log ("approximately in right dir");
				currentAim = targetAim;
				if( IsPointedIn(targetAim) ){
					//Debug.Log ("is pointed in target dir");
					aiming = false;
				}
			}
		}
		EnsureValidAngle();
	}

	void ShowDebugAimline(){
		//Debug.Log (currentAim.euler);
		Debug.DrawRay(transform.position, Quaternion.Euler(currentAim.euler) * transform.forward, Color.cyan);
	}
	
	void EnsureValidAngle(){
		currentAim = CloseAsPossibleTo(currentAim);
	}
	
	void MoveAimTowards(HorizontalCoords aimPoint){
		//Debug.Log ("Moving aim towards " + aimPoint);
		aimPoint = CloseAsPossibleTo(aimPoint).simplified;
		//Debug.Log ("Closest Aimpoint is " + aimPoint);
		// because of the structure of HorizontalCoords, not using MoveTowardsAngle should
		// mean this works better here (doesn't turn through impossible angles to reach target angle)
		currentAim.Azimuth = Mathf.MoveTowards(currentAim.Azimuth, aimPoint.Azimuth, traverse.Azimuth * Time.deltaTime);
		currentAim.Altitude = Mathf.MoveTowards(currentAim.Altitude, aimPoint.Altitude, traverse.Altitude * Time.deltaTime);
	}

	// is the gun currently aimed in aimPoint?
	bool IsPointedIn(HorizontalCoords aimPoint){
		return currentAim == aimPoint;
	}

	// is it possible for the gun to be aimed in aimPoint?
	bool CanPointIn(HorizontalCoords aimPoint){
		//return aimPoint.IsWithin(new HorizontalCoords(-maxAzimuthVariance, -maxDepression), new HorizontalCoords(maxAzimuthVariance, maxElevation));
		float tAlt = aimPoint.Altitude;
		float tAzi = aimPoint.Azimuth;
		return tAlt >= -maxDepression && tAlt <= maxElevation && tAzi >= -maxAzimuthVariance && tAzi <= maxAzimuthVariance;
	}

	// get the closest horizontal coords this mount can achieve to aimPoint
	HorizontalCoords CloseAsPossibleTo(HorizontalCoords aimPoint){
		return new HorizontalCoords( Mathf.Clamp(aimPoint.Azimuth, -maxAzimuthVariance, maxAzimuthVariance), Mathf.Clamp(aimPoint.Altitude, -maxDepression, maxElevation) );
	}

	// mount will try to orient in aimPoint
	bool SetAimTarget(HorizontalCoords aimPoint){
		// simplification here? would be redundant
		bool viable = CanPointIn(aimPoint);
		if(viable){
			targetAim = aimPoint;
			Debug.Log ("GunMount: Set aim target successfully to " + targetAim);
		}
		else{
			targetAim = CloseAsPossibleTo(aimPoint);
			Debug.Log ("GunMount: Set aim target to closest possible, " + targetAim);
		}
		aiming = true;
		return viable;
	}
}
