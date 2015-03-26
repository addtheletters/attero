using UnityEngine;
using System.Collections;

public class OverviewCamera : MonoBehaviour {

	public struct CamState{
		public float fov;
		public Quaternion rot;
		public Vector3 pos;

		public CamState(float fov, Quaternion rot, Vector3 pos){
			this.fov = fov;
			this.rot = rot;
			this.pos = pos;
		}
	}

	private CamState home;
	private CamState target;
	private CamState last;

	public bool rotChange	= false; // auto-changing state
	public bool zoomChange	= false;
	public bool posChange	= false;

	//private float 		startChangeRotTime;
	//private Quaternion	startChangeRot;

	private Vector3 posVel;
	private Vector3 rotEulerVel;
	private float fovVel;

	public float changeDur = 0.5f;

	public float rotSpeed = 200f;

	public float baseMoveSpeed = 50f;
	public float baseHeight;

	public float zoomSpeed = 360f;

	public float minZoomFOV = 10;
	public float maxZoomFOV = 120;


	private Camera cam;

	public float camSnapMargin = .001f;

	// Use this for initialization
	void Start () {
		if (baseHeight == 0)
			baseHeight = transform.position.y;
		cam = Camera.main;
		if (!cam)
			Debug.Log("OverviewCamera: no camera found");
		else {
			home = CurrentCamState();
			target = home;
			last = home;
		}
	}
	
	// Update is called once per frame
	void Update () {
		UseCameraInput  ();
		UpdateCameraPos ();// for organization purposes
	}

	private void UseCameraInput(){
		// Vector2 mousepos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
		
		// if tries to move camera during reset, abort pos reset
		if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0){
			posChange = false;
		}
		float horiz = Input.GetAxis ("Horizontal");
		float verti = Input.GetAxis ("Vertical");
		transform.Translate ( baseMoveSpeed * Time.deltaTime * (horiz * Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized + verti * Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized), Space.World);
		
		// if tries to scroll during reset, abort zoom reset
		if(Input.GetAxisRaw("Mouse ScrollWheel") != 0){
			zoomChange = false;
		}
		float scroll = Input.GetAxis ("Mouse ScrollWheel");
		cam.fieldOfView -= scroll * zoomSpeed * Time.deltaTime;
		
		if (cam.fieldOfView < minZoomFOV)
			cam.fieldOfView = minZoomFOV;
		if (cam.fieldOfView > maxZoomFOV)
			cam.fieldOfView = maxZoomFOV;
		
		if (Input.GetKeyDown (KeyCode.Space)) {
			Debug.Log ("Overview Cam: Resetting camera.");
			ChangeCamTo(home);
		}
		if (Input.GetKeyDown (KeyCode.Backspace)) {
			Debug.Log("Overview Cam: Moving camera back to previous state.");
			ChangeCamTo(last);
		}
		
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(1)) {
			//Debug.Log ("Mouse looking.");
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			
			// if user is doing rot inputs, cancel rotation movements
			if(Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0){
				rotChange = false;
			}
			
			Vector2 mouseDel = rotSpeed * Time.deltaTime * new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
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
	}

	private void UpdateCameraPos(){
		if (rotChange) {
			if( Quaternion.Angle(transform.rotation, target.rot) < camSnapMargin * 100){
				InstChangeRotation(target.rot);
				rotChange = false;
			}
			transform.rotation = Quaternion.Euler (Mathf.SmoothDampAngle(transform.rotation.eulerAngles.x, target.rot.eulerAngles.x, ref rotEulerVel.x, changeDur),
			                                       Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, target.rot.eulerAngles.y, ref rotEulerVel.y, changeDur),
			                                       Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, target.rot.eulerAngles.z, ref rotEulerVel.z, changeDur));//Quaternion.Lerp ( startChangeRot, target.rot, Mathf.SmoothStep(0, 1, (Time.time - startChangeRotTime) / changeDur) );
		}
		if (zoomChange) {
			if( Mathf.Abs(cam.fieldOfView - target.fov) < camSnapMargin ){
				InstChangeZoom(target.fov);
				zoomChange = false;
			}
			cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, target.fov, ref fovVel, changeDur);
			
		}
		if (posChange) {
			if( (transform.position - target.pos).sqrMagnitude < camSnapMargin ){
				InstChangePosition(target.pos);
				posChange = false;
			}
			transform.position = Vector3.SmoothDamp(transform.position, target.pos, ref posVel, changeDur);
		}
	}

	public CamState CurrentCamState(){
		return new CamState (cam.fieldOfView, transform.rotation, transform.position);
	}

	public CamState DesiredCamState(){
		CamState state = CurrentCamState ();
		if (zoomChange) {
			state.fov = target.fov;
		}
		if (rotChange) {
			state.rot = target.rot;
		}
		if (posChange) {
			state.pos = target.pos;		
		}
		return state;
	}

	public static float ClampLookAngle( float angle ){
		// TODO this, to restrict up/down look

		return angle;
	}

	public void ChangeCamTo(CamState state){
		last = DesiredCamState();
		ChangeZoomTo (state.fov);
		ChangeRotationTo (state.rot);
		ChangePositionTo (state.pos);
	}

	public void AbortCamChange(){
		zoomChange = false;
		posChange = false;
		rotChange = false;
	}

	private void InstChangeZoom( float fov ){
		cam.fieldOfView = fov;
		fovVel = 0;
	}
					
	private void ChangeZoomTo(float fov){
		target.fov = fov;
		if (!zoomChange) {
			zoomChange = true;
			fovVel = 0;
		}
	}

	private void InstChangeRotation( Quaternion rot ){
		transform.rotation = rot;
	}

	private void ChangeRotationTo( Quaternion rot ){
		target.rot = rot;
		if(!rotChange){
			//startChangeRotTime	= Time.time;
			//startChangeRot = transform.rotation;
			rotChange	= true;
			rotEulerVel = Vector3.zero;
		}
	}

	private void InstChangePosition( Vector3 pos ){
		transform.position = pos;
	}

	private void ChangePositionTo( Vector3 pos ){
		target.pos = pos;
		if (!posChange) {
			posChange	= true;
			posVel= Vector3.zero;
		}
	}
}
