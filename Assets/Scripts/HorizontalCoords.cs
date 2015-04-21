using UnityEngine;
using System.Collections;

[System.Serializable]
public struct HorizontalCoords{
	
	private static readonly float FLOAT_TOLERANCE = 0.001f;
	
	[SerializeField]
	private float azi; // horizontal angular shift, from north towards the east
	[SerializeField]
	private float alt; // vertical angular shift, from the horizon up towards the sky
	
	// optimizations will surely result in less SimplifyAngle usage
	// but OPTIMIZATIONS CAN WAIT HAH
	
	public HorizontalCoords(float azi, float alt){
		this.azi = azi; 
		this.alt = alt;
	}
	
	// return value will be between -180 and 180
	public static float SimplifyAngle(float angle){
		float simpliflied = angle % 360;
		if(simpliflied > 180){
			simpliflied = simpliflied - 360;
		}
		Debug.Log (angle + " simplified is " + simpliflied);
		return simpliflied;
	}

	public float SimplifiedAzimuth{
		get{
			return SimplifyAngle(this.azi);
		}
		set{
			this.azi = SimplifyAngle(value);
		}
	}
	
	public float SimplifiedAltitude{
		get{
			return SimplifyAngle(this.alt);
		}
		set{
			this.alt = SimplifyAngle(value);
		}
	}

	public float Azimuth{
		get{
			return this.azi;
		}
		set{
			this.azi = value;
		}
	}
	public float Altitude{
		get{
			return this.alt;
		}
		set{
			this.alt = value;
		}
	}
	
	public Vector3 euler{
		get{
			return new Vector3(-alt, azi, 0);
		}
	}
	
	public HorizontalCoords simplified{
		get{
			return new HorizontalCoords(SimplifiedAzimuth, SimplifiedAltitude);
		}
	}
	
	public bool IsWithin(HorizontalCoords min, HorizontalCoords max){
		return this.Altitude >= min.Altitude && this.Altitude <= max.Altitude && this.Azimuth >= min.Azimuth && this.Azimuth <= max.Azimuth;
	}
	
	public static bool FloatsApproximately(float a, float b, float tolerance){
		return Mathf.Abs(a - b) < tolerance;
	}
	
	public bool IsApproximately(HorizontalCoords approx){
		return FloatsApproximately(this.Altitude, approx.Altitude, FLOAT_TOLERANCE) && FloatsApproximately(this.Azimuth, approx.Azimuth, FLOAT_TOLERANCE);
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