using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicGunRecorder : MonoBehaviour, IGunRecorder {

	public Dictionary<BallisticProfile, Dictionary<BallisticShotInfo, List<BallisticResult>>> database;

	void Start(){
		database = new Dictionary<BallisticProfile, Dictionary<BallisticShotInfo, List<BallisticResult>>>();
	}

	public void RecordShotData (BallisticProfile bP, BallisticResult bR, BallisticShotInfo sI){
		if(!database.ContainsKey(bP)){
			database[bP] = new Dictionary<BallisticShotInfo, List<BallisticResult>>();
		}
		if(!database[bP].ContainsKey(sI)){
			database[bP][sI] = new List<BallisticResult>();
		}
		database[bP][sI].Add(bR);
		//Debug.Log ("shot data recorded for " + bP);
	}

	public string RetrieveShotData(){
		string buffer = "{ data:[\n";
		foreach(KeyValuePair<BallisticProfile,  Dictionary<BallisticShotInfo, List<BallisticResult>>> entry in database){
			buffer += "{\nprofile:" + entry.Key + ",\n";
			buffer += "shots:" + "[\n";
			foreach(KeyValuePair<BallisticShotInfo, List<BallisticResult>> subentry in entry.Value){
				buffer += "{\n";
				buffer += subentry.Key + ": [\n";
				foreach(BallisticResult result in subentry.Value){
					buffer += result + ",\n";
				}
				buffer += "]\n},\n";
			}
			buffer += "]\n";
			buffer += "}\n";
		}
		return buffer;
	}

	public void LogShotData(){
		Debug.Log (RetrieveShotData());
	}

	public void SaveShotData(){
		DebugUtil.FileIO.Write("stats.txt", RetrieveShotData(), true);
	}
}


