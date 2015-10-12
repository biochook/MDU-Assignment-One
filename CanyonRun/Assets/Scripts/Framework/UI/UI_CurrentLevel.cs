using UnityEngine;

public class UI_CurrentLevel : MonoBehaviour {
	private UnityEngine.UI.Text textField;
	
	void Awake() {
		textField = gameObject.GetComponent<UnityEngine.UI.Text>() as UnityEngine.UI.Text;
	}
	
	public void SetCurrentLevel(int currentLevel) {
		textField.text = "Player Level: " + currentLevel.ToString();
	}
}
