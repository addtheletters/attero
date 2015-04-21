using UnityEngine;
using System.Collections;

public interface IGunRecorder {
	void RecordShotData(float elevation, float shotSpeed, Vector3 hitPos, float hitTime);
	
}
