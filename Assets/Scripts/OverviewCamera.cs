using UnityEngine;
using System.Collections;

public class OverviewCamera : MonoBehaviour {

	public float 		homeFOV;
	private Quaternion 	homeRot;
	private Vector3 	homePos;

	public bool rotReset	= false;
	public bool zoomReset	= false;
	public bool posReset	= false;

	private float 		startResetRotTime;
	private Quaternion	startResetRot;

	private Vector3 posResetVel;
	private float fovResetVel;

	public float resetDur = 0.5f;

	public float rotSpeed = 5f;
	public float minXAngle = 0f;
	public float maxXAngle = 360f;

	public float baseMoveSpeed = 50f;
	public float baseHeight;

	public float zoomSpeed = 360f;

	public float minZoomFOV = 10;
	public float maxZoomFOV = 120;


	private Camera cam;

	public float camSnapMargin = .001f;

	// Use this for initialization
	void Start () {
		homeRot = transform.rotation;
		homePos = transform.position;
		if (baseHeight == 0)
			baseHeight = transform.position.y;

		cam = Camera.main;
		if (!cam)
			Debug.Log("OverviewCamera: no camera found");
		else
			homeFOV = cam.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
		// Vector2 mousepos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

		// if tries to move camera during reset, abort pos reset
		if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0){
			posReset = false;
		}

		float horiz = Input.GetAxis ("Horizontal");
		float verti = Input.GetAxis ("Vertical");
		transform.Translate ( baseMoveSpeed * Time.deltaTime * (horiz * transform.right + verti * Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, Vector3.up))), Space.World);

		// if tries to scroll during reset, abort zoom reset
		if(Input.GetAxisRaw("Mouse ScrollWheel") != 0){
			zoomReset = false;
		}
		float scroll = Input.GetAxis ("Mouse ScrollWheel");
		cam.fieldOfView -= scroll * zoomSpeed * Time.deltaTime;

		if (cam.fieldOfView < minZoomFOV)
			cam.fieldOfView = minZoomFOV;
		if (cam.fieldOfView > maxZoomFOV)
			cam.fieldOfView = maxZoomFOV;

		if (Input.GetKey (KeyCode.Space)) {
			Debug.Log ("Overview Cam: Resetting camera.");
			ResetRotation();
			ResetZoom();
			ResetPosition();
		}

		if (Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(1)) {
			//Debug.Log ("Mouse looking.");
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			// if user is doing rot inputs, cancel rotation movements
			if(Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0){
				rotReset = false;
			}

			Vector2 mouseDel = rotSpeed * new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			transform.RotateAround(transform.position, Vector3.up, mouseDel.x);
			transform.RotateAround(transform.position, -transform.right, mouseDel.y);
			
		}
		else{
			if(Input.GetKey (KeyCode.LeftShift)){
				// TODO: make holding shift allow you to pan by moving mouse to edges, Starcraft style

				//Debug.Log ("Mouse confined? What does this do exactly?");
				Cursor.lockState = CursorLockMode.Confined;
			}
			else{
				Cursor.lockState = CursorLockMode.None;
			}
			Cursor.visible = true;
		}


		if (rotReset) {
			// TODO: this better maybe?
			if( Quaternion.Angle(transform.rotation, homeRot) < camSnapMargin * 100){
				//Debug.Log ("angle is " + Quaternion.Angle(transform.rotation, homeRot) );
				InstResetRotation();
				rotReset = false;
			}
			transform.rotation = Quaternion.Slerp ( startResetRot, homeRot, Mathf.SmoothStep(0, 1, (Time.time - startResetRotTime) / resetDur) );
		}
		if (zoomReset) {
			// TODO: this
			if( Mathf.Abs(cam.fieldOfView - homeFOV) < camSnapMargin ){
				InstResetZoom();
				zoomReset = false;
			}
			cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, homeFOV, ref fovResetVel, resetDur);

		}
		if (posReset) {
			// TODO: this
			if( (transform.position - homePos).sqrMagnitude < camSnapMargin ){
				InstResetPosition();
				posReset = false;
			}
			transform.position = Vector3.SmoothDamp(transform.position, homePos, ref posResetVel, resetDur);
		}
		
		
	}


	void InstResetZoom(){
		cam.fieldOfView = homeFOV;
		fovResetVel = 0;
	}
					
	void ResetZoom(){
		if (!zoomReset) {
			zoomReset = true;
		}
	}

	void InstResetRotation(){
		transform.rotation = homeRot;
	}

	void ResetRotation(){
		if(!rotReset){
			startResetRotTime	= Time.time;
			startResetRot = transform.rotation;
			rotReset	= true;
		}
	}

	void InstResetPosition(){
		transform.position = homePos;
	}

	void ResetPosition(){
		if (!posReset) {
			posReset	= true;
			posResetVel	= Vector3.zero;
		}
	}



}
