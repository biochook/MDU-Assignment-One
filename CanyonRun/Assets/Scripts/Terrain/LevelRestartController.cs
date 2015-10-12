using UnityEngine;

public class LevelRestartController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collider) {
		// Player hit the level restart controller
		if (collider.CompareTag(Constants.Tag_Player)) {
			GameController.RestartLevel();
		}
	}
}
