using UnityEngine;

public class UIController : MonoBehaviour {
	public static UIController Instance {get; private set;}
	
	public UI_XPToLevel XPRequiredToLevel;
	public UI_CurrentLevel CurrentLevel;
	public UI_BoostTime BoostTime;
	public UI_LaunchButton LaunchButton;
	
	void Awake() {
		Instance = this;
	}
	
	public static void SetCurrentLevel(int currentLevel) {
		Instance.CurrentLevel.SetCurrentLevel(currentLevel);
	}
	
	public static void SetXPToLevel(int XPToLevel) {
		Instance.XPRequiredToLevel.SetXPToLevel(XPToLevel);
	}
	
	public static void SetBoostTimeRemaining(float boostTimeRemaining) {
		Instance.BoostTime.SetBoostTimeRemaining(boostTimeRemaining);
	}
	
	public static void DisableBoostTimeRemaining() {
		Instance.BoostTime.DisableBoostTimeRemaining();
	}
	
	public static void EnableLaunchButton() {
		Instance.LaunchButton.EnableLaunchButton();
	}
}
