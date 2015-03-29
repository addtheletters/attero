using UnityEngine;
using System.Collections;

public class TimeFuse : Fuse {

	public float fuseTime = 1f;

	new void Update () {
		base.Update();

		fuseTime -= Time.deltaTime;
	}

	public override bool ShouldDetonate (){
		return fuseTime <= 0;
	}

}
