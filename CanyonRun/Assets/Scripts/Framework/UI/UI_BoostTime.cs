using UnityEngine;

public class UI_BoostTime : MonoBehaviour {
	private UnityEngine.UI.Text textField;
	
	void Awake() {
		textField = gameObject.GetComponent<UnityEngine.UI.Text>() as UnityEngine.UI.Text;
	}
	
	public void SetBoostTimeRemaining(float boostTimeRemaining) {
		textField.enabled = true;
		textField.text = "Boost for " + boostTimeRemaining.ToString("0.0") + " more seconds";
	}
	
	public void DisableBoostTimeRemaining() {
		textField.enabled = false;
	}
}
