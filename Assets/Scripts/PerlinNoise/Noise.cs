using UnityEngine;
using System.Collections;

public static class Noise {

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, int seed, Vector2 myOffset) {
		float[,] noiseMap = new float[mapWidth,mapHeight];

		if (scale <= 0) {
			scale = 0.0001f;
		}

		//randomaise the result (landscape)
		System.Random rand = new System.Random(seed);
		Vector2[] octaveOffset = new Vector2[octaves];
		for (int i = 0; i < octaves; ++i) {
			float offsetY = rand.Next (-1000, 1000) + myOffset.x;
			float offsetX = rand.Next (-1000, 1000) + myOffset.y;
			octaveOffset [i] = new Vector2 (offsetX, offsetY);
		}

		//norm values
		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		//if we use it to zoom in in the of the heightmap
		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					
					float sampleX = (x - halfWidth) / scale * frequency + octaveOffset[i].x;
					float sampleY = (y - halfHeight) / scale * frequency + octaveOffset[i].y;
					/*
					float sampleX = (x-halfWidth) / scale * frequency;
					float sampleY = (y-halfHeight) / scale * frequency;
					 * 
					float sampleX = x / (mapWidth * scale);
                	float sampleY = y / (mapHeight * scale);
					*/

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				} else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}
				noiseMap [x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
			}
		}

		return noiseMap;
	}

}