using UnityEngine;
using System.Collections;

public interface IGunRecorder {
	void RecordShotData(BallisticProfile bP, BallisticShotInfo sI, BallisticResult bR);
}
