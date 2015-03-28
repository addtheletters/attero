using UnityEngine;
using System.Collections;

/*
 *	 Script for RTS-like camera movement, with ability
 *	 for smooth camera movements between saved camera states.
 *
 *	Look rotation currently ignores roll (z axis rotation).
 */
// TODO implement moving the camera vertically / some unfixed standard for camera height
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

	private Vector3 posVel;
	//private Vector3 rotEulerVel;
	private float rotProgress;
	private float rotVel;
	private float fovVel;

	private float clampXVel;

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

		float horiz = Input.GetAxis ("Horizontal");
		float verti = Input.GetAxis ("Vertical");
		if( horiz != 0 || verti != 0){
			Vector3 transVec = horiz * Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
			if(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.x, 0)) < 1){
				// angle is close to horizontal
				//Debug.Log ("translating while looking horizontal!" + Time.timeSinceLevelLoad);
				transVec += verti * Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
			}
			else if(CamLookingDown()){
				// looking downward
				//Debug.Log("translating while looking down! " + Time.timeSinceLevelLoad + transform.up + Vector3.ProjectOnPlane(transform.up, Vector3.up).normalized);
				transVec += verti * Vector3.ProjectOnPlane(transform.up, Vector3.up).normalized;
			}
			else{
				// looking upward
				//Debug.Log ("translating while looking up! " + Time.timeSinceLevelLoad);
				transVec += verti * Vector3.ProjectOnPlane(-transform.up, Vector3.up).normalized;
			}
			transform.Translate ( baseMoveSpeed * Time.deltaTime * transVec, Space.World);

		}
		// if tries to move camera during reset, abort pos reset
		if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0){
			posChange = false;
		}
		
		float scroll = Input.GetAxis ("Mouse ScrollWheel");
		if( scroll != 0 ){
			cam.fieldOfView -= scroll * zoomSpeed * Time.deltaTime;
		}

		// if tries to scroll during reset, abort zoom reset
		if(Input.GetAxisRaw("Mouse ScrollWheel") != 0){
			zoomChange = false;
		}
		if (cam.fieldOfView < minZoomFOV)
			cam.fieldOfView = minZoomFOV;
		if (cam.fieldOfView > maxZoomFOV)
			cam.fieldOfView = maxZoomFOV;
		
		if (Input.GetKeyDown (KeyCode.Space)) {
			//Debug.Log ("Overview Cam: Resetting camera.");
			ChangeCamTo(home);
		}
		if (Input.GetKeyDown (KeyCode.Backspace)) {
			//Debug.Log("Overview Cam: Moving camera back to previous state.");
			ChangeCamTo(last);
		}
		
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(1)) {
			//Debug.Log ("Mouse looking.");
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			
			Vector2 mouseDel = rotSpeed * Time.deltaTime * new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			transform.RotateAround(transform.position, Vector3.up, mouseDel.x);
			transform.RotateAround(transform.position, -transform.right, mouseDel.y);

			// if user is doing rot inputs, cancel rotation movements
			if(Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0){
				rotChange = false;
			}

		}
		else{
			if(!rotChange){ // if being auto-rotated or manual, do not worry about clamp
				ClampLookAngle();
			}
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
			rotProgress = Mathf.SmoothDamp(rotProgress, 1, ref rotVel, changeDur);
			transform.rotation = Quaternion.Slerp (last.rot, target.rot, rotProgress);
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

	/*
	private void ClampLookAngle (){
		Vector3 startEuler = transform.rotation.eulerAngles;
		Vector3 adjustedEuler = startEuler;
		bool zNormal	= Mathf.Abs(startEuler.z) < 5;
		bool zFlip		= Mathf.Approximately (startEuler.z, 180);
		startEuler.x = startEuler.x % 360; // just making sure
		if (zNormal) {
			Debug.Log ("Z is normal");
			if(startEuler.x >= 270){
				// looking above the horizon
				Debug.Log("Above horizon, Trying to clamp xlook between 275 and inf");
				adjustedEuler.x = Mathf.Clamp(startEuler.x, 265, Mathf.Infinity);
			}
			else if(startEuler.x <= 90){
				// looking below the horizon
				Debug.Log("Below horizon, Trying to clamp xlook between -inf and 85");
				adjustedEuler.x = Mathf.Clamp (startEuler.x, Mathf.NegativeInfinity, 85);
			}
			else{
				// should not happen, implies looking upsidedown/backwards (as if zFlip were true)
			}
		}
		else{
			Debug.Log ("Z is not normal");
		}

		transform.rotation = Quaternion.Euler (adjustedEuler);
	}*/

	private void ClampLookAngle(){
		if (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, 180)) < 1) {
			Vector3 tmpEuler = transform.rotation.eulerAngles;
			if(tmpEuler.x < 180){
				tmpEuler.x = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.x, 90, ref clampXVel, .1f);
			}
			else{
				tmpEuler.x = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.x, 270, ref clampXVel, .1f);
			}
			transform.rotation = Quaternion.Euler(tmpEuler);
		}
	}

	private bool CamLookingDown(){
		return Vector3.Angle( cam.transform.forward, Vector3.down ) < Vector3.Angle(cam.transform.forward, Vector3.up); 
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
		rotProgress = 0;
		if(!rotChange){
			rotChange	= true;
			rotVel 		= 0;
			//rotEulerVel = Vector3.zero;
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
