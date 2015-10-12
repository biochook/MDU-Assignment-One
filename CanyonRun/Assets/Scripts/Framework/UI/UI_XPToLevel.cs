using UnityEngine;

public class UI_XPToLevel : MonoBehaviour {
	private UnityEngine.UI.Text textField;
	
	void Awake() {
		textField = gameObject.GetComponent<UnityEngine.UI.Text>() as UnityEngine.UI.Text;
	}
	
	public void SetXPToLevel(int XPToLevel) {
		textField.text = "XP To Next Level: " + XPToLevel.ToString();
	}
}
