using UnityEngine;

public class PlayerController : MonoBehaviour {
	public GameObject Plane;
	public GameObject ForwardLookingCamera;
	public GameObject ProjectilePrefab;
	public float PlaneTiltAmount = 45.0f;
	public float ForwardCameraTiltAmount = 15.0f;
	public float InputForce_HScale = 20.0f;
	public float InputForce_VScale = 10.0f;
	public float ProjectileForce = 500.0f;
	
	private Rigidbody rigidBody;
	
	void Awake() {
		rigidBody = gameObject.GetComponent<Rigidbody>();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1") && GameController.CanFire) {
			// Trigger the fire event
			GameController.OnFire();
			
			// Spawn the projectile and launch it
			GameObject projectile = GameObject.Instantiate(ProjectilePrefab) as GameObject;
			projectile.transform.position = transform.position;
			projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * ProjectileForce);
		}
	}
	
	void FixedUpdate() {
		Vector3 userInput = new Vector3(Input.GetAxis("Horizontal") * InputForce_HScale, 0, Input.GetAxis("Vertical") * InputForce_VScale);
		
		// Apply the force to move the player
		rigidBody.AddForce(userInput);
		
		// Tilt the plane (and camera) based on the horizontal movement
		Plane.transform.rotation = Quaternion.Euler(0, 0, -PlaneTiltAmount * Input.GetAxis("Horizontal"));
		ForwardLookingCamera.transform.rotation = Quaternion.Euler(0, 0, -ForwardCameraTiltAmount * Input.GetAxis("Horizontal"));
	}
	
	void OnTriggerEnter(Collider collider) {
		// Notify the game controller that we hit something
		if (collider.CompareTag(Constants.Tag_Mine)) {
			GameController.OnHitMine(collider.gameObject);
		}
		else if (collider.CompareTag(Constants.Tag_XPBoost)) {
			GameController.OnHitXPBoost(collider.gameObject);
		}
		else if (collider.CompareTag(Constants.Tag_XPGain)) {
			GameController.OnHitXPGain(collider.gameObject);
		}
	}
}
