using UnityEngine;
using System.Collections;

public struct BallisticProfile {

	[SerializeField]
	private static float gravity = 9.8f;// gravity constant applied; this should be constant

	[SerializeField]
	private float drag;
	// drag constant applied; scales resistance based on velocity
	// breaks physics if greater than about 0.33f

	[SerializeField]
	// mass of the object; more mass = more inertia = less effective air drag force
	private float mass;			

	public BallisticProfile(float drag, float mass){
		this.drag = drag;
		this.mass = mass;
	}

	public float Drag{
		get{
			return this.drag;
		}
		set{
			this.drag = value;
		}
	}
	
	public float Mass{
		get{
			return this.mass;
		}
		set{
			this.mass = value;
		}
	}

	public static float Gravity{
		get{
			return gravity;
		}
		set{
			gravity = value;
		}
	}

	private static BallisticProfile standard{
		get{
			return new BallisticProfile(0.001f, 1f);
		}
	}

	public static bool operator ==(BallisticProfile a, BallisticProfile b){
		return a.drag == b.drag && a.mass == b.mass;
	}
	
	public static bool operator !=(BallisticProfile a, BallisticProfile b){
		return a.drag != b.drag || a.mass != b.mass;
	}

	public override bool Equals(object obj){
		return this == (BallisticProfile)obj;
	}
	
	public override int GetHashCode() {
		return (drag.GetHashCode() + mass).GetHashCode(); // again totally not ripped from a certain complex number struct
	}

	public override string ToString ()
	{
		return string.Format ("[BallisticProfile: Drag={0}, Mass={1}]", Drag, Mass);
	}
}
