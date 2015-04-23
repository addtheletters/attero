using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicGunRecorder : MonoBehaviour, IGunRecorder {

	public Dictionary<BallisticProfile, Dictionary<BallisticResult, List<BallisticShotInfo>>> database;

	void Start(){
		database = new Dictionary<BallisticProfile, Dictionary<BallisticResult, List<BallisticShotInfo>>>();
	}

	public void RecordShotData (BallisticProfile bP, BallisticResult bR, BallisticShotInfo sI){
		if(!database.ContainsKey(bP)){
			database[bP] = new Dictionary<BallisticResult, List<BallisticShotInfo>>();
		}
		if(database[bP][bR] == null){
			database[bP][bR] = new List<BallisticShotInfo>();
		}
		database[bP][bR].Add(sI);
	}

}
