using UnityEngine;

public class TerrainGenerator : MonoBehaviour {
	float RavineWidth = 0.2f;
	float RavineVariation = 0.15f;
	float RavineVariationFrequency = 32.0f;
		
	public float EntityYOffset = 0.0f;
	public int NumberOfFullWidthEntities = 75;
	public float EntityGenerationStartPercentage = 0.05f;
	public int NumberOfStationaryEntities = 50;
	public GameObject[] FullWidthEntities;
	public GameObject[] StationaryEntities;
	
	private Terrain terrain;
	
	void Awake() {
		// Retrieve the terrain
		terrain = gameObject.GetComponent<Terrain>();
		
		GenerateTerrain();
		PopulateCanyon();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void PopulateCanyon() {
		// Retrieve the bounds of the terrain
		Vector3 terrainBounds = terrain.GetComponent<Collider>().bounds.size;
		
		// Calculate ravine information
		float ravineWidth = RavineWidth * terrainBounds.x;
		float ravineVariation = RavineVariation * terrainBounds.x;
		float ravineCentreX = terrainBounds.x / 2.0f;
		
		// Populate the full width entities
		float fullWidthEntityInterval = 1.0f / NumberOfFullWidthEntities;
		for (float percentage = EntityGenerationStartPercentage; percentage < 1.0f; percentage += fullWidthEntityInterval) {
			// Determine the working variation at this point
			float workingRavineVariation = ravineVariation * Mathf.Sin(percentage * RavineVariationFrequency);
	
			// Calculate the spawn point		
			Vector3 spawnPoint = new Vector3(ravineCentreX + workingRavineVariation, EntityYOffset, percentage * terrainBounds.z);
			
			// Select a random entity to spawn
			GameObject selectedPrefab = FullWidthEntities[Random.Range(0, FullWidthEntities.Length)];
			
			// Spawn the new entity
			GameObject newEntity = GameObject.Instantiate(selectedPrefab) as GameObject;
			
			// Parent the entity to the terrain and position it
			newEntity.transform.parent = gameObject.transform;
			newEntity.transform.localPosition = spawnPoint;
		}
		
		// Populate the stationary entities
		float stationaryEntityInterval = 1.0f / NumberOfStationaryEntities;
		for (float percentage = EntityGenerationStartPercentage + stationaryEntityInterval * 0.5f; percentage < 1.0f; percentage += fullWidthEntityInterval) {
			// Determine the working variation at this point
			float workingRavineVariation = ravineVariation * Mathf.Sin(percentage * RavineVariationFrequency);
			float workingCentreX = ravineCentreX + workingRavineVariation;
	
			// Calculate the spawn point		
			Vector3 spawnPoint = new Vector3(workingCentreX + Random.Range(-ravineWidth, ravineWidth), EntityYOffset, percentage * terrainBounds.z);
			
			// Select a random entity to spawn
			GameObject selectedPrefab = StationaryEntities[Random.Range(0, StationaryEntities.Length)];
			
			// Spawn the new entity
			GameObject newEntity = GameObject.Instantiate(selectedPrefab) as GameObject;
			
			// Parent the entity to the terrain and position it
			newEntity.transform.parent = gameObject.transform;
			newEntity.transform.localPosition = spawnPoint;
		}
	}
	
	void GenerateTerrain() {
		// Calculate ravine information in points
		int ravineWidth = Mathf.FloorToInt(RavineWidth * terrain.terrainData.heightmapResolution);
		int ravineVariation = Mathf.FloorToInt(RavineVariation * terrain.terrainData.heightmapResolution);
		int ravineCentreY = terrain.terrainData.heightmapResolution / 2;
		
		// Setup our heightmap array
		float [,] heightMap = new float[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];
		
		// Iterate along the X axis of the terrain generating the cross sections
		for (int x = 0; x < terrain.terrainData.heightmapResolution; ++x) {
			// Calculate our percentage progress along the terrain
			float xPercentage = (float)x / (float)terrain.terrainData.heightmapResolution;
			
			// Determine the working variation at this point
			int workingRavineVariation = Mathf.FloorToInt(ravineVariation * Mathf.Sin(xPercentage * RavineVariationFrequency));
			
			// Modify the start based on this variation
			int ravineStartY = ravineCentreY - ravineWidth + workingRavineVariation;
			int ravineEndY = ravineStartY + ravineWidth * 2;
			
			// Iterate along the cross section
			for (int y = 0; y < terrain.terrainData.heightmapResolution; ++y) {
				float heightPercentage = 0.0f;
				
				// Is the point outside of the ravine (ie. should it be at max height)
				if ((y < ravineStartY) || (y > ravineEndY)) {
					heightPercentage = 1.0f;
				} // Otherwise we need to calculate the height of the ravine
				else {
					float yPercentage = 1.0f - Mathf.Abs(((float)(y - ravineStartY) / (float)ravineWidth) - 1.0f);
					heightPercentage = Mathf.Pow(Mathf.Cos(yPercentage * Mathf.PI / 2.0f), 64);
				}
					
				heightPercentage += Mathf.Clamp(-0.1f + (Mathf.PerlinNoise(x * 1.9f, y * 0.1f) * 0.3f), 0.0f, 1.0f);
				heightMap[x, y] = heightPercentage;
			}
		}
		
		// Apply the generated heightmap
		terrain.terrainData.SetHeights(0, 0, heightMap);
	}
}
