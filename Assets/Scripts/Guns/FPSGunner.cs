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
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		//Screen.lockCursor = true;
		/*gun = GetComponent<Gun>();
		if (!gun) {
			Debug.Log ("FPSGunner: no gun found");
		}*/
	}

	GameObject FindCamera(){
		return Camera.main.gameObject;
	}
	
	new void Update () {
		base.Update ();
		if(!(Cursor.lockState == CursorLockMode.Locked)){
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	public override bool ShouldFire(){
		return Input.GetButton ("Fire1");
	}

	public override void Shoot(){
		gun.Fire (cam.transform.forward);
	}
}
