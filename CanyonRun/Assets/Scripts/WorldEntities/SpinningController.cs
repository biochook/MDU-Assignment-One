using UnityEngine;

public class SpinningController : MonoBehaviour {
	private float currentTime = 0;

	public float RPM_X = 30.0f;
	private float RPS_X;
	
	public float RPM_Y = 20.0f;
	private float RPS_Y;
	
	public float RPM_Z = 10.0f;
	private float RPS_Z;

	// Use this for initialization
	void Start () {
		RPS_X = RPM_X / 60.0f;
		RPS_Y = RPM_Y / 60.0f;
		RPS_Z = RPM_Z / 60.0f;
	}
	
	// Update is called once per frame
	void Update () {
		// Update the current time
		currentTime += Time.deltaTime;

		// Calculate and apply the rotation
		float xAngle = currentTime * RPS_X * 360.0f;
		float yAngle = currentTime * RPS_Y * 360.0f;
		float zAngle = currentTime * RPS_Z * 360.0f;
		transform.localEulerAngles = new Vector3(xAngle, yAngle, zAngle);
	}
}
