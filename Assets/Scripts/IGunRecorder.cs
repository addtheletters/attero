using UnityEngine;
using System.Collections;

public interface IGunRecorder {
	void RecordShotData(BallisticProfile bP, BallisticResult bR, BallisticShotInfo sI);
}
