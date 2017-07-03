using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode{Noise, Color, Mesh};
	public DrawMode mode; 

	const int mapChunkSize = 241;

	[Range(0,6)]
	public int levelOfDetail;
	public float noiseScale;

	public float heightMultiplier;
	public AnimationCurve meshHeightCurve;

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

		float[,] noiseMap = Noise.GenerateNoiseMap (mapChunkSize
, mapChunkSize, noiseScale, octaves, persistence,lacunarity, seed, myOffset);

		//Color the 2d map 
		Color[] color = new Color[mapChunkSize * mapChunkSize
];
		for (int y = 0; y < mapChunkSize; ++y) {
			for (int x = 0; x < mapChunkSize
	; ++x) {
				float currentheight = noiseMap [x, y];

				for(int i = 0; i < regions.Length; i++){
					if (currentheight <= regions [i].height) { 
						color [y * mapChunkSize
				 + x] = regions [i].color;
						break;
					}
				}
			}
		}

		if (FindObjectOfType<MapDisplay> ()) {
			MapDisplay display = FindObjectOfType<MapDisplay> ();

			if (mode == DrawMode.Noise) {
				display.drawTexture2D (GenerateTexture.TextureFromNoiseMap (noiseMap));
			} else if (mode == DrawMode.Color) {
				display.drawTexture2D (GenerateTexture.textureFromColorMap (mapChunkSize
		, mapChunkSize, color));
			} else if (mode == DrawMode.Mesh) {
				display.drawMesh(MashGenerator.GenerateMash (noiseMap, heightMultiplier, meshHeightCurve, levelOfDetail), GenerateTexture.textureFromColorMap (mapChunkSize
		, mapChunkSize, color));
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
		if (octaves < 0)
			octaves = 0;
		if (lacunarity < 1)
			lacunarity = 1;
	
	}
}