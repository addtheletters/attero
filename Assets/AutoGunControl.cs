﻿using UnityEngine;
using System.Collections;

public abstract class AutoGunControl : GunControl {

	// note: GunControl is UNREGULATED (unlimited rate of fire). AutoGunControl is regulated.

	protected float shotDelay = 0.1f;
	protected float lastShotTime;

	protected new void Update () {
		if (shouldAutoFire ()) {
			Shoot();
			lastShotTime = Time.time;
		}
	}

	public virtual bool shouldAutoFire(){
		return (shouldFire () && ((Time.time - lastShotTime) > shotDelay));
	}

	public static float CalcShotDelay(float roundsPerMinute){
		return 60 / roundsPerMinute;
	}
}
