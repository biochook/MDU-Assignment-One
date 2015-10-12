using UnityEngine;

public class PatrollingMineController : MonoBehaviour {
	public GameObject StartPoint;
	public GameObject EndPoint;
	
	public float CyclesPerSecond = 0.5f;
	
	private float progress = 0.0f;
	private bool movingToEnd = true;
	private Vector3 movementVector;

	// Use this for initialization
	void Start () {
		// Store the start location and movement vector
		transform.position = StartPoint.transform.position;
		movementVector = EndPoint.transform.position - StartPoint.transform.position;
		
		// Randomise the starting percentage
		progress = Random.Range(0.0f, 1.0f);
		
		// Update the position
		transform.position = GameCoreInterface.Lerp(StartPoint.transform.position, StartPoint.transform.position + movementVector, progress);
	}
	
	// Update is called once per frame
	void Update () {
		// Update the progress
		progress += (movingToEnd ? 1.0f : -1.0f) * (Time.deltaTime * CyclesPerSecond);
		
		// Once we overrun the progress flip the direction
		if ((progress < 0.0f) || (progress > 1.0f)) {
			movingToEnd = !movingToEnd;
		}
		
		// Clamp the progress
		progress = Mathf.Clamp(progress, 0.0f, 1.0f);
		
		// Update the position
		transform.position = GameCoreInterface.Lerp(StartPoint.transform.position, StartPoint.transform.position + movementVector, progress);
	}
}
