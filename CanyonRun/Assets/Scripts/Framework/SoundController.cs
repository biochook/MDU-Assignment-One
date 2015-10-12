using UnityEngine;

public class SoundController : MonoBehaviour {
	public static SoundController Instance {get; private set;}
	
	public AudioSource sfx;

	public AudioClip[] HitMine;
	public AudioClip[] DestroyedMine;
	public AudioClip[] XPBoost;
	public AudioClip[] XPGain;
	public AudioClip[] Fire;

	void Awake() {
		Instance = this;
	}

	public void PlaySound(AudioSource destinationSource, AudioClip clip) {
		destinationSource.PlayOneShot (clip);
	}
	
	public static void OnFire() {
		AudioClip clip = Instance.Fire [Random.Range (0, Instance.Fire.Length)];
		Instance.PlaySound (Instance.sfx, clip);
	}
	
	public static void OnHitMine() {
		AudioClip clip = Instance.HitMine [Random.Range (0, Instance.HitMine.Length)];
		Instance.PlaySound (Instance.sfx, clip);
	}

	public static void OnDestroyedMine() {
		AudioClip clip = Instance.DestroyedMine [Random.Range (0, Instance.DestroyedMine.Length)];
		Instance.PlaySound (Instance.sfx, clip);
	}
	
	public static void OnHitXPBoost() {
		AudioClip clip = Instance.XPBoost [Random.Range (0, Instance.XPBoost.Length)];
		Instance.PlaySound (Instance.sfx, clip);
	}
	
	public static void OnHitXPGain() {
		AudioClip clip = Instance.XPGain [Random.Range (0, Instance.XPGain.Length)];
		Instance.PlaySound (Instance.sfx, clip);
	}
}
