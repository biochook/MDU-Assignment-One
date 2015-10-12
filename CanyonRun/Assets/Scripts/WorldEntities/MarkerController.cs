using UnityEngine;

public class MarkerController : MonoBehaviour {
	public GameObject MarkerMesh;
	
	void Awake() {
		MarkerMesh.GetComponent<MeshRenderer>().enabled = false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
