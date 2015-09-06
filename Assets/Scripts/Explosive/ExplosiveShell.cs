using UnityEngine;
using System.Collections;

public class ExplosiveShell : MonoBehaviour, IDetonatable {

	public float timeSinceArmed; // initialize as negative to make shells have to exist for some time before they can explode
	
	// Update is called once per frame
	void Update () {
		timeSinceArmed += Time.deltaTime;
	}

	public bool Detonate ()
	{
		if(timeSinceArmed > 0){
			this.BlowUp();
			return true;
		}
		else{
			return false; // shell not armed yet
		}
	}

	protected void BlowUp(){ // name edited for family friendliness
		// blows up
		Debug.Log ("Shell was detonated.");
		Destroy (this); // removes shell script
		//Destroy(this.gameObject); // destroys entire object
	}

}
