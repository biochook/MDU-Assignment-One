using UnityEngine;

public class ProjectileController : MonoBehaviour {
	public float ProjectileTimeRemaining = 10.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Time still left on the countdown
		if (ProjectileTimeRemaining > 0) {
			ProjectileTimeRemaining -= Time.deltaTime;
			
			// Projectile is now gone?
			if (ProjectileTimeRemaining <= 0) {
				GameObject.Destroy(gameObject);
			}
		}
	}
	
	void OnTriggerEnter(Collider collider) {
		// Ignore trigger events other than for the player
		if (!collider.CompareTag(Constants.Tag_Player)) {
			// Did we hit a mine?
			if (collider.CompareTag(Constants.Tag_Mine)) {
				GameController.OnDestroyedMine(collider.gameObject);
			}
			
			// Destroy the projectile
			GameObject.Destroy(gameObject);
		}
	}
}
