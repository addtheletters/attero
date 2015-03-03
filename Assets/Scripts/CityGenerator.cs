using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour {

	public GameObject buildingPrefab;
	// coords should be Vector2 but the horizontal axes are x and z
	// therefore to avoid confusion y is ignored
	public Vector3 cityMaxCoords = new Vector3 ( 10, 0, 10);
	public Vector3 cityMinCoords = new Vector3 (-10, 0,-10);
	public Vector3 buildingBaseDim	= new Vector3(1, 0, 1);
	public Vector3 buildingFootprint= new Vector3(1, 0, 1);

	public List<GameObject> generated;

	public float minBuildingHeight = 1f;

	void Start () {
		generated = GenerateCity ();
	}

	List<GameObject> GenerateCity(){
		List<GameObject> buildings = new List<GameObject> ();
		for (float x = cityMinCoords.x; x < cityMaxCoords.x; x += buildingFootprint.x) {
			for(float z = cityMinCoords.z; z < cityMaxCoords.z; z += buildingFootprint.z){
				Vector3 buildingPos = new Vector3(x, 0, z);
				Vector3 scale = newBuildingScale(buildingPos);
				if(validHeight(scale)){
					GameObject b = GenerateBuilding(buildingPos, scale);
					b.transform.parent = this.gameObject.transform;
					buildings.Add (b);
				}
				else{
					Debug.Log("CityGenerator: building too short at (" + x + "," + z + ") with height " + scale.y);
				}
			}
		}
		return buildings;
	}

	GameObject GenerateBuilding(Vector3 pos, Vector3 scale){
		//Vector3 scale = newBuildingScale (pos);
		GameObject building = (GameObject)Instantiate (buildingPrefab, pos, new Quaternion ());
		building.transform.localScale = scale;
		return building;
	}

	bool isInCity(Vector3 pos){
		return pos.x > cityMinCoords.x &&
				pos.x < cityMaxCoords.x &&
				pos.z > cityMinCoords.z &&
				pos.z < cityMaxCoords.z;
	}

	bool validHeight(Vector3 scale){
		return scale.y > minBuildingHeight; 
	}

	Vector3 newBuildingScale(Vector3 pos){
		return new Vector3 (buildingBaseDim.x, newBuildingHeight(pos), buildingBaseDim.z);
	}

	float newBuildingHeight(Vector3 pos){
		return buildingBaseDim.y + getBuildingHeightBase(pos) + newBuildingHeightRand (pos);
	}

	float getBuildingHeightBase(Vector3 pos){
		return Mathf.Max(Random.Range(0.5f, 2f), 10 - Mathf.Sqrt (pos.x * pos.x + pos.z * pos.z));//Mathf.Abs(pos.x) + Mathf.Abs(pos.z);
	}

	float newBuildingHeightRand(Vector3 pos){
		return Random.Range(-8, 2);
	}

}
