using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode{Noise, Color};
	public DrawMode mode; 

	public int mapWidth;
	public int mapHeight;
	public float noiseScale;

	public int seed;
	public Vector2 myOffset;

	public int octaves;
	[Range (0,1)]
	public float persistence;
	public float lacunarity;

	public Regoins[] regions;
	public bool autoUpdate;

	public void GenerateMap() {
		OnValuedate ();

		float[,] noiseMap = Noise.GenerateNoiseMap (mapWidth, mapHeight, noiseScale, octaves, persistence,lacunarity, seed, myOffset);

		//Color the 2d map 
		Color[] color = new Color[mapHeight * mapWidth];
		for (int y = 0; y < mapHeight; ++y) {
			for (int x = 0; x < mapWidth; ++x) {
				float currentheight = noiseMap [x, y];

				for(int i = 0; i < regions.Length; i++){
					if (currentheight <= regions [i].height) { 
						color [y * mapWidth + x] = regions [i].color;
						break;
					}
				}
			}
		}

		if (FindObjectOfType<MapDisplay> ()) {
			MapDisplay display = FindObjectOfType<MapDisplay> ();

			if (mode == DrawMode.Noise) {
				display.drawTexture2D (GenerateTexture.TextureFromNoiseMap(noiseMap));
			} else {
				display.drawTexture2D (GenerateTexture.textureFromColorMap(mapWidth, mapHeight, color));
			}


		} else {
			print ("Not found");
		}
	}

	[System.Serializable]
	public struct Regoins{
		public string name;
		public float height;
		public Color color;
	}

	void OnValuedate(){
		if (mapWidth < 1)
			mapWidth = 1;
		if (mapHeight < 1)
			mapHeight = 1;
		if (octaves < 0)
			octaves = 0;
		if (lacunarity < 1)
			lacunarity = 1;
	
	}
}