using UnityEngine;

public class TestTerrainGeneration : MonoBehaviour {

	public float offsetX = 100f;
	public float offsetY = 100f;

	public int depth = 10;
	public float scale = 20f;

	public int width = 256;
	public int height = 256;


	void Start(){
		offsetX = Random.Range (0f, 999999f);
		offsetY = Random.Range (0f, 999999f);
	}

	void Update(){
		Terrain terrain = GetComponent<Terrain> ();
		terrain.terrainData = GenerateTerrain(terrain.terrainData);

		offsetX += Time.deltaTime * 5f;
	}

	TerrainData GenerateTerrain (TerrainData terrainData){
		terrainData.heightmapResolution = width + 1;
		terrainData.size = new Vector3 (width, depth, height);
		terrainData.SetHeights (0, 0, GenerateHeights ());
		return terrainData;
	}

	//genaret the heights of the terrain
	float[,] GenerateHeights(){
		float[,] heights = new float[width, height];

		for(int x = 0; x < width; ++x){
			for(int y = 0; y < height; ++y){

				//get some Perlin Noise Value
				heights[x,y] = CalculateHeight(x,y);
			}
		}

		return heights;
	}

	float CalculateHeight(int x, int y){
		float valueX = (float) x / width * scale + offsetX;
		float valueY = (float) y / height * scale+ offsetY;

		return Mathf.PerlinNoise (valueX, valueY);
	}
}
