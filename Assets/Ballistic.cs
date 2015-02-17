using UnityEngine;
using System.Collections;

public class Ballistic : MonoBehaviour {

	public float grav = 9.8f;
	public float drag = 0.001f;

	public float lTime = 10f;
	public float timer;

	public Vector3 vel;
	Vector3 prevPos;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// projectile life, kill if life has expired
		timer += Time.deltaTime;
		if (timer > lTime) {
			Destroy(this.gameObject);
		}

		// update position and velocity
		prevPos = transform.position;
		transform.position	+= vel * Time.deltaTime; // newton + euler standard motion 
		transform.position	+= -drag * vel * Time.deltaTime; // air drag = velocity * dconst
		transform.position  += Vector3.down * (float)(grav * 0.5 * (Time.deltaTime * Time.deltaTime)); // gravity = (1/2)gt^2
		vel = (transform.position - prevPos) / Time.deltaTime; // new velocity

		// debug line
		Debug.DrawLine (prevPos, transform.position, Color.red, 1f);

		// point projectile collision
		RaycastHit hit;
		if (Physics.Linecast(prevPos, transform.position, out hit)){
			Debug.Log("Ballistic hit at: " + hit.point);
			Destroy(this.gameObject);
		}

	}


	public static void BallisticLaunch(GameObject projectile, Vector3 velocity){
		Ballistic bal = projectile.GetComponent<Ballistic> ();
		if (!bal) {
			Debug.Log("Ballistic Launch: added ballistic component");
			bal = projectile.AddComponent<Ballistic>();
		}
		else{
			Debug.Log("Ballistic Launch: projectile had ballistic component");
		}
		bal.vel = velocity;
	}
}
