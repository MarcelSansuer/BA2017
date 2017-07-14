using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode{Noise, Color, Mesh};
	public DrawMode mode; 

	public const int mapChunkSize = 241;

	[Range(0,6)]
	public int previewLOD;
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

	Queue<MapThreadInfo<MapData>> mapDataThreadQueue = new Queue<MapThreadInfo<MapData>>();
	Queue<MapThreadInfo<MeshData>> meshDataThreadQueue = new Queue<MapThreadInfo<MeshData>> ();

	//Threding for mapData
	public void RequestMapData(Action<MapData> callback) {
		ThreadStart threadStart = delegate {
			MapDataThread (callback);
		};

		new Thread (threadStart).Start ();
	}

	void MapDataThread(Action<MapData> callback) {
		MapData mapData = GenerateMap ();
		lock (mapDataThreadQueue) {
			mapDataThreadQueue.Enqueue (new MapThreadInfo<MapData> (callback, mapData));
		}
	}

	//Threding for MeshData
	public void RequestMeshData(MapData mapData, int LOD, Action<MeshData> callback) {
		ThreadStart threadStart = delegate {
			MeshDataThread (mapData, LOD, callback);
		};

		new Thread (threadStart).Start ();
	}

	void MeshDataThread(MapData mapData, int LOD, Action<MeshData> callback) {
		MeshData meshData = MashGenerator.GenerateMash (mapData.noiseMap, heightMultiplier, meshHeightCurve, LOD);
		lock (meshDataThreadQueue) {
			meshDataThreadQueue.Enqueue (new MapThreadInfo<MeshData> (callback, meshData));
		}
	}

	void Update(){
		int countMapDataThreadqueue = mapDataThreadQueue.Count;
		int countMeshDataThreadqueue = meshDataThreadQueue.Count;

		if(countMapDataThreadqueue > 0){
			for(int i = 0; i < countMapDataThreadqueue; i++){
				//info next item out of the que
				MapThreadInfo<MapData> threadInfo = mapDataThreadQueue.Dequeue();
				threadInfo.callback (threadInfo.parameter);
			}
		}

		if(countMeshDataThreadqueue > 0){
			for(int i = 0; i < countMeshDataThreadqueue; i++){
				//info next item out of the que
				MapThreadInfo<MeshData> threadInfo = meshDataThreadQueue.Dequeue();
				threadInfo.callback (threadInfo.parameter);
			}
		}
	}

	public MapData GenerateMap()
    {
        OnValuedate();

        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseScale, octaves, persistence, lacunarity, seed, myOffset);

        //Color the 2d map 
		Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
		colorTheMap(noiseMap, colorMap);

		return new MapData (noiseMap, colorMap);
    }

// show the result to the screen
    private void getDisplay()
    {
		MapData mapData = GenerateMap();

        if (FindObjectOfType<MapDisplay>())
        {
            MapDisplay display = FindObjectOfType<MapDisplay>();

            if (mode == DrawMode.Noise)
            {
				display.drawTexture2D(GenerateTexture.TextureFromNoiseMap(mapData.noiseMap));
            }
            else if (mode == DrawMode.Color)
            {
                display.drawTexture2D(GenerateTexture.textureFromColorMap(mapChunkSize
					, mapChunkSize, mapData.colorMap));
            }
            else if (mode == DrawMode.Mesh)
            {
				display.drawMesh(MashGenerator.GenerateMash(mapData.noiseMap, heightMultiplier, meshHeightCurve, previewLOD), GenerateTexture.textureFromColorMap(mapChunkSize
					, mapChunkSize, mapData.colorMap));
            }
        }
        else
        {
            print("Not found");
        }
    }

    //get color of heightvalues
    private void colorTheMap(float[,] noiseMap, Color[] color)
    {
        for (int y = 0; y < mapChunkSize; ++y)
        {
            for (int x = 0; x < mapChunkSize; ++x)
            {
                float currentheight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentheight <= regions[i].height)
                    {
                        color[y * mapChunkSize
                 + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
    }

    [System.Serializable]
	public struct Regoins{
		public string name;
		public float height;
		public Color color;
	}

	struct MapThreadInfo<T>{
		public readonly Action<T> callback;
		public readonly T parameter;

		public MapThreadInfo (Action<T> callback, T parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}
	}

	void OnValuedate(){
		if (octaves < 0)
			octaves = 0;
		if (lacunarity < 1)
			lacunarity = 1;
	
	}


}

public struct MapData{
	public readonly float[,] noiseMap;
	public readonly Color[] colorMap;

	public MapData (float[,] noiseMap, Color[] colorMap)
	{
		this.noiseMap = noiseMap;
		this.colorMap = colorMap;
	}
}