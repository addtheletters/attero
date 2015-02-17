using UnityEngine;
using System.Collections;

public class FPSGunner : MonoBehaviour {

	Gun fpsgun;
	GameObject cam;

	// Use this for initialization
	void Start () {
		cam = FindCamera ();
		if (!cam) {
			Debug.Log("FPSGunner: no camera found");
		}
		fpsgun = GetComponent<Gun>();
		if (!fpsgun) {
			Debug.Log ("FPSGunner: no gun found");
		}
		Screen.lockCursor = true;
	}

	GameObject FindCamera(){
		return Camera.main.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1")){
			Shoot();
		}
	}

	void Shoot(){
		fpsgun.Fire (cam.transform.forward);
	}
}
