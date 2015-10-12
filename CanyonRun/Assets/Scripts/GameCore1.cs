using UnityEngine;

public class GameCore 
{
	//Set up properties for the Game Core's system memory
	/*private int level;
	private int xp;
	private string boostActive;
	private float boostTimeRemaining;
	private string fireOnCooldown;
	private float fireCooldownRemaining;


	public int Level
	{
		get 
		{
			return level;
		}
		set
		{
			level = value;
		}
	}

	public int XP
		{
		get 
		{
			return xp;
		}
		set
		{
			xp = value;
		}
	}

	public string BoostActive
	{
		get
		{
			return boostActive;
		}
		set
		{
			boostActive = value;
		}
	}

	public float BoostTimeRemaining
	{
		get
		{
			return boostTimeRemaining;
		}
		set
		{
			boostTimeRemaining = value;
		}
	}
	public string FireOnCooldown 
	{
		get 
		{
			return fireOnCooldown;
		}
		set 
		{
			fireOnCooldown = value;
		}
	}
	public float FireCooldownRemaining 
	{
		get
		{
			return fireCooldownRemaining
		}
		*/

	public int 		Level 					= 1;
	public int 		XP 						= 5;
	public bool 	BoostActive;
	public float 	BoostTimeRemaining;
	public bool 	FireOnCooldown;
	public float 	FireCooldownRemaining;
	public bool 	CanFire;

	public float Lerp(float from, float to, float percent)
	{
		float retVal;

		//	This is error checking to prevent percent being invalid number
		if (percent < 0.0f)
			percent = 0.0f;
		else if (percent > 100.0f)
			percent = 100.0f;

		float bigger;
		float smaller;

		if (from > to)
		{
			bigger = from;
			smaller = to;
		}
		else if (from < to)
		{
			bigger = to;
			smaller = from;
		}
		else if (from == to)
			return to;

		retVal = (bigger - smaller) * (percent / 100.0f) + smaller;

		return retVal;
	}

	public int XPRequiredToNextLevel()
	{
		int retVal;

		//	If level equals odd
		if ((Level % 2) == 1) // Modulus operation
		{
			retVal = ((Level + 1) * 5) - XP;
		}
		//	If level equals even
		else
		{
			retVal = ((Level + 1) * 10) - XP;
		}

		return retVal;
	}

	public int XPLossFromMine()
	{

	}
	public int XPGainFromMine()
	{

	}
	public int XPGainFromPickup()
	{

	}
}
