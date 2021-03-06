﻿using UnityEngine;
using System.Collections;

public struct BallisticShotInfo : IJSONable {

	// upward from the horizon. 90 is straight up
	[SerializeField]
	private float angle;
	// muzzle velocity
	[SerializeField]
	private float speed;

	public BallisticShotInfo(float angle, float speed){
		this.angle = angle;
		this.speed = speed;
	}

	public float Angle {
		get {
			return angle;
		}
		set {
			angle = value;
		}
	}

	public float Speed {
		get {
			return speed;
		}
		set {
			speed = value;
		}
	}

	public static float GetAngleFor(Vector3 aim){
		// TODO test this
		return 90 - Vector3.Angle(aim, Vector3.up);
	}

	public static Vector3 GetAimVectorFor( float angle ){
		return Quaternion.Euler(-angle, 0, 0) * (new Vector3(0, 0, 1)); // along z+ axis, so rotation on x axis means rotating upwards?
	}

	public static bool operator ==(BallisticShotInfo a, BallisticShotInfo b){
		return a.angle == b.angle && a.speed == b.speed;
	}
	
	public static bool operator !=(BallisticShotInfo a, BallisticShotInfo b){
		return a.angle != b.angle || a.speed != b.speed;
	}
	
	public override bool Equals(object obj){
		return this == (BallisticShotInfo)obj;
	}
	
	public override int GetHashCode() {
		return (angle.GetHashCode() + speed).GetHashCode(); // again again totally not ripped from a certain complex number struct
	}
	
	public override string ToString(){
		return string.Format ("[BallisticShotInfo: Angle={0}, Speed={1}]", Angle, Speed);
	}

	public string ToJSON(){
		return string.Format("{type:\"BallisticShotInfo\", angle:{0}, speed:{1}}", Angle, Speed);
	}
}
