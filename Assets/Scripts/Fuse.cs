using UnityEngine;
using System.Collections;

public abstract class Fuse : MonoBehaviour {

	// fuses are designed such that multiple fuses can be attached
	// to the same shell; any one of their conditions being satisfied
	// will blow up the shell

	// TODO more fuse types
	// DONE Time: detonates after a time has passed
	// -Contact / Percussion: detonates upon impact
	// -Proximity / Radar: detonates when near something or about to hit something (IRL, momentum carries fragments forward to deal damage)
	// -maybe Altitide: detonates when shell reaches a certain height
	// because some shells were set to airburst x meters above the ground
	// -maybe Distance: detonates when shell has travelled x meters

	// maybe for later, either as a fuse or as an extension to ExplosiveShell: delayed detonation?
	// fuse condition satisfied, then wait some time before exploding

	protected IDetonatable explosive;

	protected void Start(){
		explosive = GetComponent<IDetonatable> ();
		if (explosive == null) {
			Debug.Log("Fuse: no explosive on object " + this.gameObject);
		}
	}
	
	// Update is called once per frame
	protected void Update () {
		if(ShouldDetonate()){
			explosive.Detonate();
		}
	}

	public abstract bool ShouldDetonate(); 
	
}
