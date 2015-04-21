using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualizerTest : MonoBehaviour {

	public class Logifier{
		private static int[] lookup;
		public static void Setup(){
			lookup = new int[256];
			for (int i = 1; i < 256; i++){
				lookup[i] = (int)(Mathf.Log(i) / Mathf.Log(2));
			}
		}
		public static int LogLookup(int i)
		{
			if (i >= 0x1000000) { return lookup[i >> 24] + 24; }
			else if (i >= 0x10000) { return lookup[i >> 16] + 16; }
			else if (i >= 0x100) { return lookup[i >> 8] + 8; }
			else { return lookup[i]; }
		}
		public static int Pow2(int p){
			if(p < 0){
				Debug.Log ("Not implemented");
				return -1;
			}
			int ret = 1;
			for(int i = 0; i < p; i++){
				ret *= 2;
			}
			return ret;
		}
	}

	public GameObject visualPrefab;
	public Vector3 spacing;
	public Vector3 scaling;

	public GameObject[] visuals;
	private float[] audiodata;
	private float[] audiospectrum;

	public AudioSource source;

	public int samples = 1024;
	public int divisions = 1;
	public int visualizedDivisions = 30;

	public int sectsize;

	public bool useSpectrum = true;

	public float spectModifier = 2f;
	public float rotModifier = 2f;

	public float rotChangeTimeBuff = .001f;
	public float rotSpeed = 20f;
	public float rotChangeAbility = 20f;

	private float rotChangeTime;
	private Vector3 currRotBaseVel;
	private Vector3 currRotAccel;
	private Vector3 currRotTarget;

	private List<float> recentScaledVol;
	public int volHistoryLen = 10;

	private const float fMax = 22000;
	private const float fMin = 10;

	private Logifier logifier;
	
	// Use this for initialization
	void Start () {
		Logifier.Setup();

		recentScaledVol = new List<float>();

		audiodata = new float[samples];
		audiospectrum = new float[samples];
		visuals = new GameObject[divisions];

		if(samples < 64){
			Debug.Log ("VisualizerTest: spectrum will fail, fewer samples than minimum of 64");
		}

		if(visualizedDivisions > divisions){
			Debug.Log ("VisualizerTest: number of visual bars specified exceeds number of tracked audio divisions");
		}
		
		for(int i = 0; i < visualizedDivisions; i++){
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

	private float BandVol(float fLow, float fHigh, float[] spectrum){
		fLow	= Mathf.Clamp (fLow, fMin, fMax);
		fHigh	= Mathf.Clamp (fHigh, fLow, fMax);

		int n1 = Mathf.FloorToInt(fLow * samples / fMax);
		int n2 = Mathf.FloorToInt(fHigh * samples / fMax);

		float total = 0;
		for( int i = n1; i <= n2; i++){
			total += spectrum[i];
		}

		return total / (n2-n1 + 1);
	}

	private float ScaleTo01(float val, float min, float max){
		return Mathf.Clamp01((val - min) / (max - min));
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
		while(dataIndex < samples && visualIndex <= visualizedDivisions){
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

		for(int i = 0; i < visualizedDivisions; i++){
			visuals[i].GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.blue, Color.yellow, sectvol[i]);
			visuals[i].transform.localScale = Vector3.one + scaling * spectModifier * Mathf.Max(0.001f, 50 * sectvol[i]);
		}



		// Volume display (rotation speed)?
		float vol = 0;
		for (int i = 0; i < samples; i++) {
			vol += audiodata[i] * audiodata[i];
		}

		float rms = Mathf.Sqrt(vol / samples);
		float scaledVol = ScaleTo01( Mathf.Sqrt(rms), 0, 1);

		
//		Debug.Log("rms is " + rms);
		Debug.Log("scaled vol is " + scaledVol);

		recentScaledVol.Add(scaledVol);
		while(recentScaledVol.Count > volHistoryLen){
			recentScaledVol.RemoveAt(0);
		}
		float workingVol = RecentAvgVol() * rotModifier;
		float workingVolSquared = workingVol * workingVol;
		float workingVolCubed = workingVolSquared * workingVol;

		//Debug.Log ("working: " + workingVol + ".  curr:" + scaledVol);

		transform.Rotate( currRotBaseVel *  workingVol );
		currRotBaseVel = Vector3.SmoothDamp(currRotBaseVel, currRotTarget, ref currRotAccel, Time.smoothDeltaTime) * workingVolSquared;
		rotChangeTime -= Time.smoothDeltaTime * workingVolCubed; 
		if(rotChangeTime <= 0){
			rotChangeTime = rotChangeTimeBuff;
			//Debug.Log ("VisualizerTest: Changed target rotation speed.");
			currRotTarget = (Quaternion.Euler(Random.insideUnitSphere * workingVolCubed * rotChangeAbility) * currRotTarget).normalized * rotSpeed;
		}

		// Targeting specific frequency bands?
		//Debug.Log("first:" + BandVol(10, 100, audiospectrum));
		//Debug.Log("second:" + BandVol(200, 300, audiospectrum));
	}
}
