using UnityEngine;
using System.Collections;

public static class Noise {

	public enum NormMode{
		LOCA,GLOBAL
	};

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, int seed, Vector2 myOffset, NormMode normMode) {
		float[,] noiseMap = new float[mapWidth,mapHeight];

		if (scale <= 0) {
			scale = 0.0001f;
		}

		//get maxvalue for global height
		float maxGlobalHeight = 0;
		float amplitude = 1;
		float frequency = 1;


		//randomaise the result (landscape)
		System.Random rand = new System.Random(seed);
		Vector2[] octaveOffset = new Vector2[octaves];
		for (int i = 0; i < octaves; ++i) {
			float offsetY = rand.Next (-1000, 1000) + myOffset.x;
			float offsetX = rand.Next (-1000, 1000) - myOffset.y;
			octaveOffset [i] = new Vector2 (offsetX, offsetY);

			maxGlobalHeight += amplitude;
			amplitude += persistance;
		}

		//norm values
		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		//if we use it to zoom in in the of the heightmap
		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

				 amplitude = 1;
				 frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					
					float sampleX = (x - halfWidth + octaveOffset[i].x) / scale * frequency ;
					float sampleY = (y - halfHeight + octaveOffset[i].y) / scale * frequency ;
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
				//min and max Noiseheigth for local
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

				if (normMode == NormMode.LOCA) {

					noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
						
				} else {
						//TODO get globel max height value
					float temp = noiseMap[x,y];
					float normGlobalHeight = ((temp + 1) / (2f * maxGlobalHeight / 2.5f));
					noiseMap [x, y] = normGlobalHeight;
				}
			}
		}

		return noiseMap;
	}

}