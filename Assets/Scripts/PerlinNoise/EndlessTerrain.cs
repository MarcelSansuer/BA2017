using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EndlessTerrain : MonoBehaviour {

	const float viewerMoveThrChUpdate = 25f;
	const float SQRviewerMoveThrChUpdate = viewerMoveThrChUpdate * viewerMoveThrChUpdate;

	static MapGenerator mapGenerator;

	public LODInfo[] detailLevels;
	public static float maxViewDst;
	public Transform viewer;
	public static Vector2 viewerPosition;
	Vector2 oldViewerPosition;

	public Material mapMaterial;

	//helper for chunk
	int chunkSize;
	int chunksVisibleInViewDst;




	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	void Start() {
		mapGenerator = FindObjectOfType<MapGenerator>();
		maxViewDst = detailLevels [detailLevels.Length - 1].visibleThresholdDst;
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

		UpdateVisibleChunks ();
	}

	void Update() {
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);
		if ((oldViewerPosition - viewerPosition).sqrMagnitude > SQRviewerMoveThrChUpdate) {
			oldViewerPosition = viewerPosition;
			UpdateVisibleChunks ();
		}
	}

	/*
		Update the postion of the visible chunks
	*/
	void UpdateVisibleChunks() {

		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
			terrainChunksVisibleLastUpdate [i].SetVisible (false);
		}
		terrainChunksVisibleLastUpdate.Clear ();

		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / chunkSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
					terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk ();
					if (terrainChunkDictionary [viewedChunkCoord].IsVisible ()) {
						//when we have generate the chunk than we can dispaly
						terrainChunksVisibleLastUpdate.Add (terrainChunkDictionary [viewedChunkCoord]);
					}
				} else {
					//create a new chunk (Terrain out of plane and noise values)
					terrainChunkDictionary.Add (viewedChunkCoord, new TerrainChunk (viewedChunkCoord, chunkSize, detailLevels,transform, mapMaterial));
				}

			}
		}
	}

	public class TerrainChunk {

		//for the terrain and Game Objects
		GameObject meshObject;
		MeshRenderer meshRenderer;
		MeshFilter meshFilter;

		Vector2 position;
		int size;

		LODInfo[] detailLevels;
		MeshLOD[] lodMeshes;

		MapData mapData;
		bool mapDataReceived;

		int previewLODIndex = -1;

		public TerrainChunk(Vector2 coord, int size,LODInfo[] detailLevels, Transform parent, Material material) {
			this.detailLevels = detailLevels;
			this.size = size;
			position = coord * size;

			//meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
			//add new GameObjekt
			meshObject = new GameObject("Terrain");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshRenderer.material = material;
			meshObject.transform.position = new Vector3(position.x,0,position.y);
			//meshObject.transform.localScale = Vector3.one * size /10f;
			meshObject.transform.parent = parent;
			SetVisible(false);

			lodMeshes = new MeshLOD[detailLevels.Length];
			for(int i = 0 ; i < detailLevels.Length; i++){
				
				lodMeshes[i] = new MeshLOD(this.detailLevels[i].LOD, UpdateTerrainChunk);
			}
			mapGenerator.RequestMapData(position, OnMapDataReceived);
		}

		void OnMapDataReceived(MapData mapData) {
			//mapGenerator.RequestMeshData (mapData, OnMeshDataReceived);
			this.mapData = mapData;
			mapDataReceived = true;

			Texture2D texture = GenerateTexture.textureFromColorMap (MapGenerator.mapChunkSize, MapGenerator.mapChunkSize, mapData.colorMap);
			meshRenderer.material.mainTexture = texture;

			UpdateTerrainChunk ();
		}

		void OnMeshDataReceived(MeshData meshData) {
			meshFilter.mesh = meshData.CreateMesh ();
		}

		public void UpdateTerrainChunk() {
			if(mapDataReceived){
				//give me the smalles way to next egde of the bounding box
				float viewerDstFromNearestEdge = Mathf.Sqrt(new Bounds(position,Vector2.one * size).SqrDistance (viewerPosition));
				bool visible = viewerDstFromNearestEdge <= maxViewDst;

				if (visible) {
					int index = 0;

					for(int i = 0; i <detailLevels.Length; i++){
						if(viewerDstFromNearestEdge > detailLevels[i].visibleThresholdDst){
							index = i + 1;
						}else{
							break;
						}
					}

					if(index != previewLODIndex){
						MeshLOD meshLOD = lodMeshes [index];
						if (meshLOD.hasMesh) {
							previewLODIndex = index;
							meshFilter.mesh = meshLOD.mesh;
						} else if(!meshLOD.hasRequestofmesh){
							meshLOD.RequestofMesh (mapData);
						}
					}
				}

				SetVisible (visible);
			}
		}

		public void SetVisible(bool visible) {
			meshObject.SetActive (visible);
		}

		public bool IsVisible() {
			return meshObject.activeSelf;
		}

	}

	class MeshLOD{
		System.Action callBackOfUpdate;
		public Mesh mesh;
		public bool hasRequestofmesh;
		public bool hasMesh;
		int LOD;

		public MeshLOD(int LOD, System.Action callBackOfUpdate){
			this.LOD = LOD;
			this.callBackOfUpdate = callBackOfUpdate;
		}

		public void RequestofMesh(MapData mapData){
			hasRequestofmesh = true;
			mapGenerator.RequestMeshData (mapData, LOD, OnMeshDataReceived);
		}

		void OnMeshDataReceived(MeshData meshData){
			mesh = meshData.CreateMesh ();
			hasMesh = true;

			callBackOfUpdate ();
		}
	}

	[System.Serializable]
	public struct LODInfo{
		public int LOD;
		public float visibleThresholdDst;
	}
}