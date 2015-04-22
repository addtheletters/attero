using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Ballistic))]
public class BallisticReporter : MonoBehaviour {

	public GameObject recorderObject;
	private IGunRecorder recorder;
	private Ballistic bal;
	private BallisticShotInfo bsi = default(BallisticShotInfo); // assigned when appropriate launch platform fires the shot

	public float reportInterval;
	protected float lastReportTime;

	void Start () {
		if(!recorderObject){
			Debug.Log ("Ballistic Reporter: no recorderObject assigned, cannot put data anywhere");
		}
		else{
			recorder = recorderObject.GetComponent<IGunRecorder>();
			if(recorder == null){
				Debug.Log ("Ballistic Reporter: no recorder found on assigned recorderObject, cannot put data anywhere");
			}
		}
		if(bsi == default(BallisticShotInfo)){
			Debug.Log ("Ballistic Reporter: BSI is null, needs assignment");
		}
		bal = GetComponent<Ballistic>();
	}
	
	void Update () {
		lastReportTime = Time.time;
		if((Time.time - lastReportTime) > reportInterval){
			Report();
		}
	}

	void Report(){
		recorder.RecordShotData(bal.profile, bsi, new BallisticResult( GetRelativeResult(), bal.timer ));
	}

	Vector2 GetRelativeResult(){
		Vector3 deltaVec = bal.getPosition() - recorderObject.transform.position;
		return new Vector2(Mathf.Sqrt(deltaVec.x * deltaVec.x + deltaVec.z * deltaVec.z), deltaVec.y);
	}
		                  
}
