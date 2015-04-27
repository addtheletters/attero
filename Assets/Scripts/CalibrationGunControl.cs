using UnityEngine;
using System.Collections;

public class CalibrationGunControl : GunControl {
	// Defaults to firing along the +x axis
	// Fires incrementally at angles

	private float startAltitude;
	private float maxAltitude;
	private float altitudeIncrement;

	private float currentAltitude;

	private bool roundsComplete = false;

	public void FireCalibShot(){
		Shoot();
		currentAltitude += altitudeIncrement;

		if(currentAltitude > maxAltitude){
			ResetCalib();
		}
	}

	public void ResetCalib(){
		roundsComplete = true;
		currentAltitude = startAltitude;
	}

	public static Vector3 AimVector( float angle ){
		return new Vector3(); // TODO this
	}
	
	#region implemented abstract members of GunControl
	public override void Shoot (){
		gun.Fire( AimVector(currentAltitude) );
	}

	public override bool ShouldFire (){
		return false; // should never fire automatically
	}
	#endregion
}
