/*
Quellen:

https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html
*/


using UnityEngine;

public class PerlinNoise : MonoBehaviour {

	public float offsetX = 100f;
	public float offsetY = 100f;

	public float scale = 10f;

	public int width = 256;
	public int height = 256;


	void Start(){
		offsetX = Random.Range (0f, 999999f);
		offsetY = Random.Range (0f, 999999f);
	}


	void Update () {
		Renderer rand = GetComponent<Renderer>();	
		rand.material.mainTexture = GenerateTexture ();
	}

	//Generate Heightmap texture with PerlinNoise 
	Texture2D GenerateTexture (){
		Texture2D texture = new Texture2D(width,height);

		//PerlinNoise
		for (int x = 0; x < width; ++x){ //x loop
			for (int y = 0; y < height; ++y) { // y loop
			
				//set the color
				Color color = calcColor(x,y);
				texture.SetPixel(x,y,color);
			}
		}

		texture.Apply ();
		return texture;
	}

	//Calculate the color for the Heightmap
	Color calcColor(int x, int y){

		//cast the x and y value (we need 1/0)
		float castX = (float)x / width * scale + offsetX;
		float castY = (float)y / height * scale + offsetY;

		float value = Mathf.PerlinNoise(castX,castY);
		return new Color (value, value, value);
	}
}
