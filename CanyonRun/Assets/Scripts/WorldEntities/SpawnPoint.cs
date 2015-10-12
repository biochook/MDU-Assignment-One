using UnityEngine;

public class SpawnPoint : MonoBehaviour {
	public GameObject[] SpawnPointMeshes;
	
	void Awake() {
		foreach(GameObject mesh in SpawnPointMeshes) {
			mesh.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
