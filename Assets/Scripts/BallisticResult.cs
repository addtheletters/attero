using UnityEngine;
using System.Collections;

public struct BallisticResult {

	// where the shot is
	[SerializeField]
	private Vector2 position;

	// how long the shot has been in the air
	// (based off fixedDeltaTime?
	[SerializeField]
	private float flightTime;

	// did this result come from the projectile hitting something?
	[SerializeField]
	private bool impact;

	// private bool hasRicocheted? maybe impact serves the purpose
	
	public BallisticResult(Vector2 position, float flightTime, bool impact){
		this.position = position;
		this.flightTime = flightTime;
		this.impact = impact;
	}
	
	public BallisticResult(Vector2 position, float flightTime){
		this.position = position;
		this.flightTime = flightTime;
		this.impact = false;
	}

	public Vector2 Position {
		get {
			return position;
		}
		set {
			position = value;
		}
	}

	public float FlightTime {
		get {
			return flightTime;
		}
		set {
			flightTime = value;
		}
	}
	
	public static bool operator ==(BallisticResult a, BallisticResult b){
		return a.position == b.position && a.flightTime == b.flightTime;
	}
	
	public static bool operator !=(BallisticResult a, BallisticResult b){
		return a.position != b.position || a.flightTime != b.flightTime;
	}
	
	public override bool Equals(object obj){
		return this == (BallisticResult)obj;
	}
	
	public override int GetHashCode() {
		return (position.GetHashCode() + flightTime).GetHashCode(); // again again totally not ripped from a certain complex number struct
	}
	
	public override string ToString(){
		return string.Format ("[BallisticResult: position={0}, flightTime={1}, impact={2}]", position, flightTime, impact);
	}


}
