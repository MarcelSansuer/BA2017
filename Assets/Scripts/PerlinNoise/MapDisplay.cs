using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour {

	public Renderer textureRender;
	public MeshFilter filter;
	public MeshRenderer rendereOfMesh;

	public void drawTexture2D(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height);
	}

	public void drawMesh(MeshData meshData, Texture2D texture){
		filter.sharedMesh = meshData.CreateMesh();
		rendereOfMesh.sharedMaterial.mainTexture = texture;
	}
}