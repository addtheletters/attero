using UnityEngine;
using System.Collections;

public class OverviewCamera : MonoBehaviour {

	private Vector3 baseRotation;

	public float baseMoveSpeed = 5f;
	public float baseHeight;

	public float zoomSpeed = 180f;
	public float baseZoomFOV;
	public float minZoomFOV = 10;
	public float maxZoomFOV = 120;

	private Camera cam;

	// Use this for initialization
	void Start () {
		baseRotation = transform.rotation.eulerAngles;
		if (baseHeight == 0)
			baseHeight = transform.position.y;

		cam = Camera.main;
		if (!cam)
			Debug.Log("OverviewCamera: no camera found");
		else
			baseZoomFOV = cam.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
		//Vector2 mousepos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

		// hmmm how to implement? 
		// eventually might want scroll acceleration
		// or just have scroll dependent on distance mouse is from center
		transform.Translate ( baseMoveSpeed * Time.deltaTime * new Vector3( Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), Space.World);
		//Debug.Log (Input.GetAxis ("Mouse ScrollWheel"));
		cam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;

		if (cam.fieldOfView < minZoomFOV)
			cam.fieldOfView = minZoomFOV;
		if (cam.fieldOfView > maxZoomFOV)
			cam.fieldOfView = maxZoomFOV;
	}

	void ResetCameraZoom(){
		cam.fieldOfView = baseZoomFOV;
	}

}
