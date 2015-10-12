using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameCoreInterface {
	private static GameCore core = new GameCore();
	private static FallbackGameCore fallbackCore = new FallbackGameCore();
	private static BindingFlags SearchFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
	
	private enum InterfaceStatus {
		Missing,
		WrongSignature,
		IntegrityCheckFailed,
		Valid
	}
	
	private enum SubsystemStatus {
		Offline,
		CommunicationsError,
		AnomalousDataDetected,
		Online
	}
	
	private enum Subsystem {
		SystemMemory,
		CoreLogic,
		SensorySystem,
		WeaponSystem,
		EnhancementSystem,
		CareerProgression
	}
	
	private static PropertyInfo property_Level;
	private static PropertyInfo property_XP;
	private static PropertyInfo property_BoostActive;
	private static PropertyInfo property_BoostTimeRemaining;
	private static PropertyInfo property_CanFire;

	private static MethodInfo method_Lerp;
	private static MethodInfo method_OnFire;
	private static MethodInfo method_DecreaseFireCooldownRemaining;
	private static MethodInfo method_EnableBoost;
	private static MethodInfo method_DecreaseBoostTimeRemaining;
	private static MethodInfo method_AddXP;
	private static MethodInfo method_XPRequiredToNextLevel;
	private static MethodInfo method_XPLossFromMine;
	private static MethodInfo method_XPGainFromMine;
	private static MethodInfo method_XPGainFromPickup;
	
	private static object activeCore_Lerp;
	private static object activeCore_Sensors;
	private static object activeCore_AllOtherSystems;
	
	private static Dictionary<Subsystem, SubsystemStatus> CurrentSubsystemStatus = new Dictionary<Subsystem, SubsystemStatus>();
	
	public static void PatchInterfaces() {
		// Test the interfaces but disallow any display
		GameCoreInterface.TestInterfaces(false);
		
		if (CurrentSubsystemStatus[Subsystem.CoreLogic] == SubsystemStatus.Online) {
			method_Lerp 							= typeof(GameCore).GetMethod("Lerp", SearchFlags);
			activeCore_Lerp 						= core;
		}
		else {
			method_Lerp 							= typeof(FallbackGameCore).GetMethod("Lerp", SearchFlags);
			activeCore_Lerp							= fallbackCore;
		}
		
		if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.Online) {
			method_XPRequiredToNextLevel 			= typeof(GameCore).GetMethod("XPRequiredToNextLevel", SearchFlags);
			method_XPLossFromMine 					= typeof(GameCore).GetMethod("XPLossFromMine", SearchFlags);
			method_XPGainFromMine 					= typeof(GameCore).GetMethod("XPGainFromMine", SearchFlags);
			method_XPGainFromPickup 				= typeof(GameCore).GetMethod("XPGainFromPickup", SearchFlags);
			activeCore_Sensors 						= core;
		}
		else {
			method_XPRequiredToNextLevel 			= typeof(FallbackGameCore).GetMethod("XPRequiredToNextLevel", SearchFlags);
			method_XPLossFromMine 					= typeof(FallbackGameCore).GetMethod("XPLossFromMine", SearchFlags);
			method_XPGainFromMine 					= typeof(FallbackGameCore).GetMethod("XPGainFromMine", SearchFlags);
			method_XPGainFromPickup 				= typeof(FallbackGameCore).GetMethod("XPGainFromPickup", SearchFlags);
			activeCore_Sensors						= fallbackCore;
		}
		
		// Link up the properties and remaining methods to the appropriate core
		if ((CurrentSubsystemStatus[Subsystem.SystemMemory] == SubsystemStatus.Online) &&
			(CurrentSubsystemStatus[Subsystem.WeaponSystem] == SubsystemStatus.Online) &&
			(CurrentSubsystemStatus[Subsystem.EnhancementSystem] == SubsystemStatus.Online) &&
			(CurrentSubsystemStatus[Subsystem.CareerProgression] == SubsystemStatus.Online)) {
			property_Level 							= typeof(GameCore).GetProperty("Level", SearchFlags);
			property_XP 							= typeof(GameCore).GetProperty("XP", SearchFlags);
			property_BoostActive 					= typeof(GameCore).GetProperty("BoostActive", SearchFlags);
			property_BoostTimeRemaining 			= typeof(GameCore).GetProperty("BoostTimeRemaining", SearchFlags);
			property_CanFire 						= typeof(GameCore).GetProperty("CanFire", SearchFlags);

			method_OnFire 							= typeof(GameCore).GetMethod("OnFire", SearchFlags);
			method_DecreaseFireCooldownRemaining 	= typeof(GameCore).GetMethod("DecreaseFireCooldownRemaining", SearchFlags);

			method_EnableBoost 						= typeof(GameCore).GetMethod("EnableBoost", SearchFlags);
			method_DecreaseBoostTimeRemaining 		= typeof(GameCore).GetMethod("DecreaseBoostTimeRemaining", SearchFlags);

			method_AddXP 							= typeof(GameCore).GetMethod("AddXP", SearchFlags);
			
			activeCore_AllOtherSystems				= core;
		}
		else {
			property_Level 							= typeof(FallbackGameCore).GetProperty("Level", SearchFlags);
			property_XP 							= typeof(FallbackGameCore).GetProperty("XP", SearchFlags);
			property_BoostActive 					= typeof(FallbackGameCore).GetProperty("BoostActive", SearchFlags);
			property_BoostTimeRemaining 			= typeof(FallbackGameCore).GetProperty("BoostTimeRemaining", SearchFlags);
			property_CanFire 						= typeof(FallbackGameCore).GetProperty("CanFire", SearchFlags);

			method_OnFire 							= typeof(FallbackGameCore).GetMethod("OnFire", SearchFlags);
			method_DecreaseFireCooldownRemaining 	= typeof(FallbackGameCore).GetMethod("DecreaseFireCooldownRemaining", SearchFlags);

			method_EnableBoost 						= typeof(FallbackGameCore).GetMethod("EnableBoost", SearchFlags);
			method_DecreaseBoostTimeRemaining 		= typeof(FallbackGameCore).GetMethod("DecreaseBoostTimeRemaining", SearchFlags);

			method_AddXP 							= typeof(FallbackGameCore).GetMethod("AddXP", SearchFlags);
			
			activeCore_AllOtherSystems				= fallbackCore;
		}
	}
	
	public static void TestInterfaces(bool displayOutput = true) {
		// By default all subystems are offline
		CurrentSubsystemStatus[Subsystem.SystemMemory] = SubsystemStatus.Offline;
		CurrentSubsystemStatus[Subsystem.CoreLogic] = SubsystemStatus.Offline;
		CurrentSubsystemStatus[Subsystem.SensorySystem] = SubsystemStatus.Offline;
		CurrentSubsystemStatus[Subsystem.WeaponSystem] = SubsystemStatus.Offline;
		CurrentSubsystemStatus[Subsystem.EnhancementSystem] = SubsystemStatus.Offline;
		CurrentSubsystemStatus[Subsystem.CareerProgression] = SubsystemStatus.Offline;

		// Write out the status information
		if (displayOutput) {
			Terminal.WriteLine("Initiating Pre-Flight Systems Check");
		}
		
		// Test the system memory, core logic and sensory systems
		CurrentSubsystemStatus[Subsystem.SystemMemory] = EvaluateGameCore_SystemMemory();
		CurrentSubsystemStatus[Subsystem.CoreLogic] = EvaluateGameCore_CoreLogic();
		CurrentSubsystemStatus[Subsystem.SensorySystem] = EvaluateGameCore_SensorSystem();
		
		if (displayOutput) {
			// Write out the memory subsystem message
			Terminal.WriteLine("Searching for primary system memory ..........................");
			if (CurrentSubsystemStatus[Subsystem.SystemMemory] == SubsystemStatus.Offline) {
				Terminal.WriteLine("    Primary system memory offline");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.SystemMemory] == SubsystemStatus.CommunicationsError) {
				Terminal.WriteLine("    Primary system memory failed comms check");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.SystemMemory] == SubsystemStatus.Online) {
				Terminal.WriteLine("    Primary system memory online");
			}
			
			// Write out the core logic subsystem message
			Terminal.WriteLine("Searching for core logic subsystem ...........................");
			if (CurrentSubsystemStatus[Subsystem.CoreLogic] == SubsystemStatus.Offline) {
				Terminal.WriteLine("    Core logic subsystem offline");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.CoreLogic] == SubsystemStatus.CommunicationsError) {
				Terminal.WriteLine("    Core logic subsystem failed comms check");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.CoreLogic] == SubsystemStatus.Online) {
				Terminal.WriteLine("    Core logic subsystem online");
			}
			
			// Write out the sensory subsystem message
			Terminal.WriteLine("Searching for sensory subsystem ..............................");
			if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.Offline) {
				Terminal.WriteLine("    Sensory subsystem offline");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.CommunicationsError) {
				Terminal.WriteLine("    Sensory subsystem failed comms check");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.Online) {
				Terminal.WriteLine("    Sensory subsystem online");
			}
		}
		
		// Test the remaining subsystems only if the system memory, core logic and sensory systems are online
		if ((CurrentSubsystemStatus[Subsystem.SystemMemory] == SubsystemStatus.Online) &&
			(CurrentSubsystemStatus[Subsystem.CoreLogic] == SubsystemStatus.Online) &&
			(CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.Online)) {
			CurrentSubsystemStatus[Subsystem.WeaponSystem] = EvaluateGameCore_WeaponSystem();
			CurrentSubsystemStatus[Subsystem.EnhancementSystem] = EvaluateGameCore_EnhancementSystem();
			CurrentSubsystemStatus[Subsystem.CareerProgression] = EvaluateGameCore_CareerProgression();
		}
		
		if (displayOutput) {
			// Write out the weapons subsystem message
			Terminal.WriteLine("Searching for weapons subsystem ..............................");
			if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.Offline) {
				Terminal.WriteLine("    Weapons subsystem offline");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.CommunicationsError) {
				Terminal.WriteLine("    Weapons subsystem failed comms check");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.Online) {
				Terminal.WriteLine("    Weapons subsystem online");
			}
			
			// Write out the enhancements subsystem message
			Terminal.WriteLine("Searching for enhancements subsystem .........................");
			if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.Offline) {
				Terminal.WriteLine("    Enhancements subsystem offline");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.CommunicationsError) {
				Terminal.WriteLine("    Enhancements subsystem failed comms check");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.Online) {
				Terminal.WriteLine("    Enhancements subsystem online");
			}
			
			// Write out the career progression subsystem message
			Terminal.WriteLine("Searching for career progression subsystem ...................");
			if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.Offline) {
				Terminal.WriteLine("    Career progression subsystem offline");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.CommunicationsError) {
				Terminal.WriteLine("    Career progression subsystem failed comms check");
				Terminal.WriteLine("    Switching to secondary subsystem");
			}
			else if (CurrentSubsystemStatus[Subsystem.SensorySystem] == SubsystemStatus.Online) {
				Terminal.WriteLine("    Career progression subsystem online");
			}
	
			Terminal.WriteLine("");
	
			// Display the final summary		
			int numPrimarySystemsActive = CurrentSubsystemStatus.Count(s => s.Value == SubsystemStatus.Online);
			Terminal.WriteLine(numPrimarySystemsActive.ToString() + " of " + CurrentSubsystemStatus.Count.ToString() + " primary subsystems are online.");
		}
	}
	
	private static SubsystemStatus EvaluateGameCore_SystemMemory() {
		List<InterfaceStatus> statusResults = new List<InterfaceStatus>();
		
		// Test all of the properties
		statusResults.Add(EvaluatePropertyStatus("Level", typeof(int)));
		statusResults.Add(EvaluatePropertyStatus("XP", typeof(int)));
		statusResults.Add(EvaluatePropertyStatus("BoostActive", typeof(bool)));
		statusResults.Add(EvaluatePropertyStatus("BoostTimeRemaining", typeof(float)));
		statusResults.Add(EvaluatePropertyStatus("FireOnCooldown", typeof(bool)));
		statusResults.Add(EvaluatePropertyStatus("FireCooldownRemaining", typeof(float)));
		statusResults.Add(EvaluatePropertyStatus("CanFire", typeof(bool)));
		
		// Are all of the interfaces valid?
		if (statusResults.Count(s => s == InterfaceStatus.Valid) == statusResults.Count) {
			return SubsystemStatus.Online;
		} // All of the interfaces are missing?
		else if (statusResults.Count(s => s == InterfaceStatus.Missing) == statusResults.Count) {
			return SubsystemStatus.Offline;
		}
		
		// Otherwise it is a mixed bag
		return SubsystemStatus.CommunicationsError;
	}
	
	private static SubsystemStatus EvaluateGameCore_CoreLogic() {
		List<InterfaceStatus> statusResults = new List<InterfaceStatus>();
		
		statusResults.Add(EvaluateMethodStatus("Lerp", typeof(float), new []{typeof(float), typeof(float), typeof(float)}));
		
		// Are all of the interfaces valid?
		if (statusResults.Count(s => s == InterfaceStatus.Valid) == statusResults.Count) {
			return SubsystemStatus.Online;
		} // All of the interfaces are missing?
		else if (statusResults.Count(s => s == InterfaceStatus.Missing) == statusResults.Count) {
			return SubsystemStatus.Offline;
		}
		
		// Otherwise it is a mixed bag
		return SubsystemStatus.CommunicationsError;
	}
	
	private static SubsystemStatus EvaluateGameCore_WeaponSystem() {
		List<InterfaceStatus> statusResults = new List<InterfaceStatus>();
		
		statusResults.Add(EvaluateMethodStatus("OnFire", typeof(void), null));
		statusResults.Add(EvaluateMethodStatus("DecreaseFireCooldownRemaining", typeof(void), new []{typeof(float)}));
		
		// Are all of the interfaces valid?
		if (statusResults.Count(s => s == InterfaceStatus.Valid) == statusResults.Count) {
			return SubsystemStatus.Online;
		} // All of the interfaces are missing?
		else if (statusResults.Count(s => s == InterfaceStatus.Missing) == statusResults.Count) {
			return SubsystemStatus.Offline;
		}
		
		// Otherwise it is a mixed bag
		return SubsystemStatus.CommunicationsError;
	}
	
	private static SubsystemStatus EvaluateGameCore_EnhancementSystem() {
		List<InterfaceStatus> statusResults = new List<InterfaceStatus>();
		
		statusResults.Add(EvaluateMethodStatus("EnableBoost", typeof(void), new []{typeof(float)}));
		statusResults.Add(EvaluateMethodStatus("DecreaseBoostTimeRemaining", typeof(void), new []{typeof(float)}));
		
		// Are all of the interfaces valid?
		if (statusResults.Count(s => s == InterfaceStatus.Valid) == statusResults.Count) {
			return SubsystemStatus.Online;
		} // All of the interfaces are missing?
		else if (statusResults.Count(s => s == InterfaceStatus.Missing) == statusResults.Count) {
			return SubsystemStatus.Offline;
		}
		
		// Otherwise it is a mixed bag
		return SubsystemStatus.CommunicationsError;
	}

	private static SubsystemStatus EvaluateGameCore_CareerProgression() {
		List<InterfaceStatus> statusResults = new List<InterfaceStatus>();
		
		statusResults.Add(EvaluateMethodStatus("AddXP", typeof(bool), new []{typeof(int)}));
		
		// Are all of the interfaces valid?
		if (statusResults.Count(s => s == InterfaceStatus.Valid) == statusResults.Count) {
			return SubsystemStatus.Online;
		} // All of the interfaces are missing?
		else if (statusResults.Count(s => s == InterfaceStatus.Missing) == statusResults.Count) {
			return SubsystemStatus.Offline;
		}
		
		// Otherwise it is a mixed bag
		return SubsystemStatus.CommunicationsError;
	}
	
	private static SubsystemStatus EvaluateGameCore_SensorSystem() {
		List<InterfaceStatus> statusResults = new List<InterfaceStatus>();
		
		statusResults.Add(EvaluateMethodStatus("XPRequiredToNextLevel", typeof(int), new []{typeof(int)}));
		statusResults.Add(EvaluateMethodStatus("XPLossFromMine", typeof(int), new []{typeof(int)}));
		statusResults.Add(EvaluateMethodStatus("XPGainFromMine", typeof(int), new []{typeof(int)}));
		statusResults.Add(EvaluateMethodStatus("XPGainFromPickup", typeof(int), new []{typeof(int)}));
		
		// Are all of the interfaces valid?
		if (statusResults.Count(s => s == InterfaceStatus.Valid) == statusResults.Count) {
			return SubsystemStatus.Online;
		} // All of the interfaces are missing?
		else if (statusResults.Count(s => s == InterfaceStatus.Missing) == statusResults.Count) {
			return SubsystemStatus.Offline;
		}
		
		// Otherwise it is a mixed bag
		return SubsystemStatus.CommunicationsError;
	}

	private static InterfaceStatus EvaluatePropertyStatus(string name, Type requiredType) {
		// Check for the presence of the property using reflection
		try {
			// Search for public properties on the GameCore
			PropertyInfo propInfo = typeof(GameCore).GetProperty(name, SearchFlags);
			
			// If the property was found check the type
			if (propInfo.PropertyType == requiredType) {
				return InterfaceStatus.Valid;
			}
			else {
				Debug.LogError("The property " + name + " on the GameCore has the wrong type.");
				return InterfaceStatus.WrongSignature;
			}
		}
		catch (NullReferenceException) {
			Debug.LogError("The property " + name + " could not be found on the GameCore.");
			return InterfaceStatus.Missing;
		}
	}
	
	private static InterfaceStatus EvaluateMethodStatus(string name, Type requiredReturnType, Type[] requiredParameters) {
		// Check for the presence of the method using reflection
		try {
			// Search for the method on the GameCore
			MethodInfo methodInfo = typeof(GameCore).GetMethod(name, SearchFlags);
			
			// If the method was found check the return type
			if (methodInfo.ReturnType == requiredReturnType) {
				ParameterInfo[] parameters = methodInfo.GetParameters();

				// Method has parameters?
				if (parameters.Length > 0) {
					// Method has parameters but none were expected
					if (requiredParameters == null) {
						Debug.LogError("The method " + name + " on the GameCore does not match the required syntax.");
						return InterfaceStatus.WrongSignature;
					}
					
					// Wrong number of parameters?
					if (parameters.Length  != requiredParameters.Length) {
						Debug.LogError("The method " + name + " on the GameCore does not match the required syntax.");
						return InterfaceStatus.WrongSignature;
					}
					
					// Test each parameter to make sure the type is correct
					for(int paramIndex = 0; paramIndex < parameters.Length; ++paramIndex) {
						if (parameters[paramIndex].ParameterType != requiredParameters[paramIndex]) {
							Debug.LogError("The method " + name + " on the GameCore does not match the required syntax.");
							return InterfaceStatus.WrongSignature;
						}
					}
				} // Method has no parameters?
				else {
					if ((requiredParameters != null) && (requiredParameters.Length > 0)) {
						Debug.LogError("The method " + name + " on the GameCore does not match the required syntax.");
						return InterfaceStatus.WrongSignature;
					}
				}
				
				return InterfaceStatus.Valid;
			}
			else {
				Debug.LogError("The method " + name + " on the GameCore does not match the required syntax.");
				return InterfaceStatus.WrongSignature;
			}
		}
		catch (NullReferenceException) {
			Debug.LogError("The method " + name + " could not be found on the GameCore.");
			return InterfaceStatus.Missing;
		}
	}
	
	public static void Update(float deltaTime) {
		// Update the boost time if it is active
		if (BoostActive) {
			DecreaseBoostTimeRemaining(deltaTime);
		}
		
		// Update the can fire cooldown if it is active
		if (!CanFire) {
			method_DecreaseFireCooldownRemaining.Invoke(activeCore_AllOtherSystems, new[] {(object)deltaTime});
		}
	}
	
	#region Lerp Interfaces
	public static float Lerp(float fromValue, float toValue, float percentage) {
		return (float) method_Lerp.Invoke(activeCore_Lerp, new [] {(object)fromValue, (object)toValue, (object)percentage});
	}
	
	public static Vector3 Lerp(Vector3 fromValue, Vector3 toValue, float percentage) {
		return new Vector3(Lerp(fromValue.x, toValue.x, percentage),
						   Lerp(fromValue.y, toValue.y, percentage),
						   Lerp(fromValue.z, toValue.z, percentage));
	}
	#endregion
	
	#region Player State Interfaces
	public static int Level {
		get {
			return (int)property_Level.GetValue(activeCore_AllOtherSystems, null);
		}
		set {
			property_Level.SetValue(activeCore_AllOtherSystems, (object)value, null);
		}
	}
	
	public static int XP {
		get {
			return (int)property_XP.GetValue(activeCore_AllOtherSystems, null);
		}
		set {
			property_XP.SetValue(activeCore_AllOtherSystems, (object)value, null);
		}
	}
	
	public static bool BoostActive {
		get {
			return (bool)property_BoostActive.GetValue(activeCore_AllOtherSystems, null);
		}
	}
	
	public static bool CanFire {
		get {
			return (bool)property_CanFire.GetValue(activeCore_AllOtherSystems, null);
		}
	}
	
	public static float BoostTimeRemaining {
		get {
			return (float)property_BoostTimeRemaining.GetValue(activeCore_AllOtherSystems, null);
		}
	}
	
	public static void SetPlayerState(int newLevel, int newXP) {
		Level = newLevel;
		XP = newXP;
	}
	
	public static bool AddXP(int deltaXP) {
		return (bool) method_AddXP.Invoke(activeCore_AllOtherSystems, new [] {(object)deltaXP});
	}
	
	public static void EnableBoost(float boostTime) {
		method_EnableBoost.Invoke(activeCore_AllOtherSystems, new [] {(object)boostTime});
	}
	
	public static void DecreaseBoostTimeRemaining(float deltaTime) {
		method_DecreaseBoostTimeRemaining.Invoke(activeCore_AllOtherSystems, new [] {(object)deltaTime});
	}
	
	public static void OnFire() {
		method_OnFire.Invoke(activeCore_AllOtherSystems, null);
	}
	#endregion
	
	#region XP Query Interfaces
	public static int XPRequiredToNextLevel() {
		return XPRequiredToNextLevel(Level) - XP;
	}
	
	public static int XPRequiredToNextLevel(int currentLevel) {
		return (int)method_XPRequiredToNextLevel.Invoke(activeCore_Sensors, new [] {(object)currentLevel});
	}
	
	public static int XPLossFromMine(int currentLevel) {
		return (int)method_XPLossFromMine.Invoke(activeCore_Sensors, new [] {(object)currentLevel});
	}
	
	public static int XPGainFromMine(int currentLevel) {
		return (int)method_XPGainFromMine.Invoke(activeCore_Sensors, new [] {(object)currentLevel});
	}
	
	public static int XPGainFromPickup(int currentLevel) {
		return (int)method_XPGainFromPickup.Invoke(activeCore_Sensors, new [] {(object)currentLevel});
	}
	
	public static float XPMultiplierFromBoost() {
		return 2.0f;
	}
	#endregion
}
