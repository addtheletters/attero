using UnityEngine;
using System.Collections;

public class FPSGunner : AutoGunControl {

	GameObject cam;

	new void Start () {
		base.Start ();

		cam = FindCamera ();
		if (!cam) {
			Debug.Log("FPSGunner: no camera found");
		}
		Screen.lockCursor = true;
		/*theGun = GetComponent<Gun>();
		if (!theGun) {
			Debug.Log ("FPSGunner: no gun found");
		}*/
	}

	GameObject FindCamera(){
		return Camera.main.gameObject;
	}
	
	new void Update () {
		base.Update ();
		if(!Screen.lockCursor){
			Screen.lockCursor = true;
		}
	}

	public override bool shouldFire(){
		return Input.GetButton ("Fire1");
	}

	public override void Shoot(){
		theGun.Fire (cam.transform.forward);
	}
}
