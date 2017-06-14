using System.Collections;
using UnityEngine;

/*
That Geneartor use Perlin Noise to create prozedurale Terrain in life time. 
author: Marcel Sansür
date: 12.Juni.2017

sources:
https://youtu.be/vFvwyu_ZKfU
https://youtu.be/WP-Bm65Q-1Y?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html


The class Noise return a 2D array of float values and create a grid of value, between One and Zero. 
*/

public static class Noise {

	public static float[,] NoiseMapGenerator(int width, int height){
		float[,] noiseMap = new float[width, height];

		for(int y = 0; y < height; ++y){
			for(int x = 0; x < width; ++x){
				//float value
			}
		}
		return noiseMap;
	}
}
