using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualizerTest : MonoBehaviour {

	public GameObject visualPrefab;
	public Vector3 spacing;
	public Vector3 scaling;

	public GameObject[] visuals;
	private float[] audiodata;
	private float[] audiospectrum;

	public AudioSource source;

	public int samples = 1024;
	public int divisions = 1;

	public int sectsize;

	public bool useSpectrum = true;

	public float rotChangeTimeBuff = .001f;
	public float rotSpeed = 20f;
	public float rotChangeAbility = 20f;

	private float rotChangeTime;
	private Vector3 currRotBaseVel;
	private Vector3 currRotAccel;
	private Vector3 currRotTarget;

	private List<float> recentScaledVol;
	public int volHistoryLen = 10;
	
	// Use this for initialization
	void Start () {
		recentScaledVol = new List<float>();

		audiodata = new float[samples];
		audiospectrum = new float[samples];
		visuals = new GameObject[divisions];

		if(samples < 64){
			Debug.Log ("VisualizerTest: spectrum will fail, fewer samples than minimum of 64");
		}
		
		for(int i = 0; i < divisions; i++){
			visuals[i] = (GameObject)Instantiate(visualPrefab, transform.position + spacing * i, transform.rotation);
			visuals[i].transform.parent = this.transform;
		}
		sectsize = samples / divisions;
		if(sectsize < 1){
			sectsize = 1;
		}

		if(scaling == Vector3.zero){
			Debug.Log ("VisualizerTest: no scaling given, defaulting to y scaling");
			scaling = new Vector3(0, 1, 0);
		}

		currRotTarget = Random.insideUnitSphere * rotSpeed;

	}

	private float RecentAvgVol(){
		if(recentScaledVol.Count == 0){
			return 0;
		}
		float total = 0;
		float[] arr = recentScaledVol.ToArray();
		for(int i = 0 ; i < recentScaledVol.Count; i++){
			total += arr[i];
		}
		return total / recentScaledVol.Count;
	}

	// Update is called once per frame
	void Update () {
		source.GetOutputData(audiodata, 0);
		source.GetSpectrumData(audiospectrum, 0, FFTWindow.BlackmanHarris);

		//Debug.Log(audiodata.Length + " : " + audiodata);
		//Debug.Log(audiodata.Length + " : " + audiospectrum);

		// Spectrum display (bars, etc)
		float[] sectvol = new float[divisions];

		int dataIndex = 0;
		int visualIndex = 0;
		while(dataIndex < samples){
			//Debug.Log ("data index: " + dataIndex + ", visual index: " + visualIndex);
			if(visualIndex >= divisions){
				visualIndex = divisions-1;
			}
			if(useSpectrum){
				sectvol[visualIndex] += audiospectrum[dataIndex];
			}
			else{
				sectvol[visualIndex] += Mathf.Abs(audiodata[dataIndex]);
			}

			dataIndex++;
			if( sectsize == 1 || ((((dataIndex-1) % (sectsize-1)) == 0) && dataIndex > 0)){
				//Debug.Log (dataIndex + ", sectsize is " + (sectsize-1) + "..." + (dataIndex % sectsize-1));
				//Debug.Log (1 % 1023);
				if(!useSpectrum){
					sectvol[visualIndex] = Mathf.Clamp01(Mathf.Sqrt(sectvol[visualIndex] / samples) * sectvol[visualIndex]);
				}
				visualIndex ++;
			}
		}

		for(int i = 0; i < divisions; i++){
			visuals[i].GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.blue, Color.yellow, sectvol[i]);
			visuals[i].transform.localScale = Vector3.one + scaling * Mathf.Max(0.001f, 50 * sectvol[i]);
		}



		// Volume display (rotation speed)?
		float vol = 0;
		for (int i = 0; i < samples; i++) {
			vol += Mathf.Abs(audiodata[i]);
		}
		float rms = Mathf.Sqrt(vol / samples);
		float scaledVol = Mathf.Clamp(Mathf.Sqrt(rms * vol), 0, 2) / 2;
		recentScaledVol.Add(scaledVol);
		while(recentScaledVol.Count > volHistoryLen){
			recentScaledVol.RemoveAt(0);
		}
		float workingVol = RecentAvgVol();
		float workingVolSquared = workingVol * workingVol;
		float workingVolCubed = workingVolSquared * workingVol;

		Debug.Log ("working: " + workingVol + ".  curr:" + scaledVol);

		transform.Rotate( currRotBaseVel *  workingVol );
		currRotBaseVel = Vector3.SmoothDamp(currRotBaseVel, currRotTarget, ref currRotAccel, Time.smoothDeltaTime) * workingVolSquared;
		rotChangeTime -= Time.smoothDeltaTime * workingVolCubed; 
		if(rotChangeTime <= 0){
			rotChangeTime = rotChangeTimeBuff;
			//Debug.Log ("VisualizerTest: Changed target rotation speed.");
			currRotTarget = (Quaternion.Euler(Random.insideUnitSphere * workingVolCubed * rotChangeAbility) * currRotTarget).normalized * rotSpeed;
		}
	}
}
