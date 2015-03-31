using UnityEngine;
using System.Collections;
using System;

public class GunMount : MonoBehaviour {

	[Serializable]
	public struct HorizontalCoords{
		[SerializeField]
		private float azi; // horizontal angular shift, from north towards the east
		[SerializeField]
		private float alt; // vertical angular shift, from the horizon up towards the sky

		// optimizations will surely result in less SimplifyAngle usage

		public HorizontalCoords(float azi, float alt){
			this.azi = SimplifyAngle(azi); 
			this.alt = SimplifyAngle(alt);
		}
		
		// return value will be between -180 and 180
		public static float SimplifyAngle(float angle){
			float simpliflied = angle % 360;
			if(simpliflied > 180){
				simpliflied = simpliflied - 360;
			}
			return simpliflied;
		}


		public float Azimuth{
			get{
				return SimplifyAngle(this.azi);
			}
			set{
				this.azi = SimplifyAngle(value);
			}
		}

		public float Altitude{
			get{
				return SimplifyAngle(this.azi);
			}
			set{
				this.azi = SimplifyAngle(value);
			}
		}

		public Vector3 euler{
			get{
				return new Vector3(alt, azi, 0);
			}
		}

		public bool IsWithin(HorizontalCoords min, HorizontalCoords max){
			return this.Altitude >= min.Altitude && this.Altitude <= max.Altitude && this.Azimuth >= min.Azimuth && this.Azimuth <= max.Azimuth;
		}

		public static bool operator ==(HorizontalCoords a, HorizontalCoords b){
			return a.Altitude == b.Altitude && a.Azimuth == b.Azimuth;
		}

		public static bool operator !=(HorizontalCoords a, HorizontalCoords b){
			return a.Altitude != b.Altitude || a.Azimuth != b.Azimuth;
		}

		public override bool Equals(object obj){
			return this == (HorizontalCoords)obj;
		}

		public override int GetHashCode() {
			return (azi.GetHashCode() + alt).GetHashCode(); // totally not ripped from a certain complex number struct
		}

		public override string ToString ()
		{
			return string.Format ("[HorizontalCoords: Azimuth={0}, Altitude={1}, euler={2}]", Azimuth, Altitude, euler);
		}

	}

	// gun aim speed
	public HorizontalCoords traverse = new HorizontalCoords(60f, 60f);

	// altitude restrictions
	public float maxElevation  = 90;	// how far above the horizon
	public float maxDepression = 0;		// how far below the horizon

	// azimuth (horizontal) restrictions.
	public float maxAzimuthVariance = 90; // how far to the left and right of center
	// 90 will allow gun to traverse a 180 degree arc

	// is the gun changing its aim point?
	private bool aiming;

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
		if(aiming){
			MoveAimTowards(targetAim);
			if( IsPointedIn(targetAim) ){
				aiming = false;
			}
		}
	}

	void MoveAimTowards(HorizontalCoords aimPoint){
		currentAim.Azimuth = Mathf.MoveTowardsAngle(currentAim.Azimuth, targetAim.Azimuth, traverse.Azimuth * Time.deltaTime);
		currentAim.Altitude = Mathf.MoveTowardsAngle(currentAim.Altitude, targetAim.Altitude, traverse.Altitude * Time.deltaTime);
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
