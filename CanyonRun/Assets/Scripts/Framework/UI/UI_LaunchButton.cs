using UnityEngine;
using UnityEngine.UI;

public class UI_LaunchButton : MonoBehaviour {
	private UnityEngine.UI.Button button;
	private CanvasRenderer buttonRenderer;
	private UnityEngine.UI.Text text;
	
#pragma warning disable 0618
	void Awake() {
		button = gameObject.GetComponent<UnityEngine.UI.Button>() as UnityEngine.UI.Button;
		buttonRenderer = button.GetComponentInChildren<CanvasRenderer>() as CanvasRenderer;
		text = button.GetComponentInChildren<Text>() as Text;

		// Hide the button		
		button.enabled = false;
		buttonRenderer.SetAlpha(0);
		text.color = Color.clear;
	}
	
	
	public void LaunchButtonClicked() {
		Application.LoadLevel("Level1");
	}
	
	public void EnableLaunchButton() {
		button.enabled = true;
		buttonRenderer.SetAlpha(1);
		text.color = Color.white;
	}
#pragma warning restore 0618
}
