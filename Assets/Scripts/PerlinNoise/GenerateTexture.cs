using System.Collections;
using UnityEngine;

public static class GenerateTexture {

	public static Texture2D TextureFromNoiseMap(float[,] heightMap){
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		Color[] color = new Color[width*height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {

				color[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
			}
		}
		return textureFromColorMap (width, height, color);
	}

	public static Texture2D textureFromColorMap(int width, int height, Color[] colorMap){
		Texture2D texture = new Texture2D (width, height);

		//texture mode (make block texture)
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colorMap);
		texture.Apply ();
		return texture;
	}
}
