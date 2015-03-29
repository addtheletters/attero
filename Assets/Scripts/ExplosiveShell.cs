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
			// TODO make this murder the gameobject horribly
			throw new System.NotImplementedException ();
		}
		else{
			return false;
		}
	}

}
