using UnityEngine;
using System.Collections;

public class CalibrationGunControl : GunControl {
	// Defaults to firing along the +z axis
	// Fires incrementally at angles

	private float startAltitude = 0;
	private float maxAltitude = 90;
	private float altitudeIncrement = 5;

	private float currentAltitude;

	private bool roundsComplete = false;

	public bool Complete{
		get{
			return roundsComplete;
		}
	}

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

	#region implemented abstract members of GunControl
	public override void Shoot (){
		GameObject projectile = gun.Fire( BallisticShotInfo.GetAimVectorFor(currentAltitude) );
		projectile.GetComponent<BallisticReporter>().Bsi = new BallisticShotInfo(currentAltitude, gun.muzzleVel);
		projectile.GetComponent<BallisticReporter>().recorder = GetComponent<IGunRecorder>();
	}

	public override bool ShouldFire (){
		return false; // should never fire automatically
	}
	#endregion
}
