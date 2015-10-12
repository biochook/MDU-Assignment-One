using UnityEngine;

public class GameController : MonoBehaviour {
	public static GameController Instance {get; private set;}
	
	public float BoostTime = 5.0f;
	
	void Awake() {
		Instance = this;
		
		GameCoreInterface.PatchInterfaces();
	}

	// Use this for initialization
	void Start () {
		// Load the player state
		int playerLevel = PlayerPrefs.GetInt(Constants.Settings_PlayerLevel, 1);
		int playerXP = PlayerPrefs.GetInt(Constants.Settings_PlayerXP, 0);
		
		// Set the player state
		GameCoreInterface.SetPlayerState(playerLevel, playerXP);
		
		// Update the UI
		UIController.SetCurrentLevel(playerLevel);
		UIController.SetXPToLevel(GameCoreInterface.XPRequiredToNextLevel());
		UIController.DisableBoostTimeRemaining();
	}
	
	// Update is called once per frame
	void Update () {
		// Update the game core
		GameCoreInterface.Update(Time.deltaTime);
		
		// Update the UI
		UIController.SetCurrentLevel(GameCoreInterface.Level);
		UIController.SetXPToLevel(GameCoreInterface.XPRequiredToNextLevel());
		if (GameCoreInterface.BoostActive) {
			UIController.SetBoostTimeRemaining(GameCoreInterface.BoostTimeRemaining);
		}
		else {
			UIController.DisableBoostTimeRemaining();
		}
	}
	
	public static bool CanFire {
		get {
			return GameCoreInterface.CanFire;
		}
	}
	
	public static void OnFire() {
		SoundController.OnFire();
		GameCoreInterface.OnFire();
	}
	
	public static void OnHitMine(GameObject hitObject) {
		// Trigger any sounds if required
		SoundController.OnHitMine();
		
		// Determine the XP loss and apply it
		int XPLoss = GameCoreInterface.XPLossFromMine(GameCoreInterface.Level);
		GameCoreInterface.AddXP(-XPLoss);
		
		// Destroy the object
		GameObject.Destroy(hitObject);
	}
	
	public static void OnDestroyedMine(GameObject destroyedObject) {
		// Trigger any sounds if required
		SoundController.OnDestroyedMine();
		
		// Determine the XP gain and apply it
		int XPGain = GameCoreInterface.XPGainFromMine(GameCoreInterface.Level);
		GameCoreInterface.AddXP(XPGain);
		
		// Destroy the object
		GameObject.Destroy(destroyedObject);
	}
	
	public static void OnHitXPBoost(GameObject hitObject) {
		// Trigger any sounds if required
		SoundController.OnHitXPBoost();
		
		// Enable the boost
		GameCoreInterface.EnableBoost(Instance.BoostTime);
		
		// Destroy the object
		GameObject.Destroy(hitObject);
	}
	
	public static void OnHitXPGain(GameObject hitObject) {
		// Trigger any sounds if required
		SoundController.OnHitXPGain();
		
		// Determine the XP loss and apply it
		int XPGain = GameCoreInterface.XPGainFromPickup(GameCoreInterface.Level);
		GameCoreInterface.AddXP(XPGain);
		
		// Destroy the object
		GameObject.Destroy(hitObject);
	}
	
	public static void RestartLevel() {
		// Save the player state
		PlayerPrefs.SetInt(Constants.Settings_PlayerLevel, GameCoreInterface.Level);
		PlayerPrefs.SetInt(Constants.Settings_PlayerXP, GameCoreInterface.XP);
		PlayerPrefs.Save();
		
		// Restart the level
		Application.LoadLevel(Application.loadedLevelName);
	}
}
