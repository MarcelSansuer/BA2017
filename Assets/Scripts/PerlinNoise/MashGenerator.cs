﻿using System.Collections;
using UnityEngine;

public static class MashGenerator {

	public static MeshData GenerateMash(float[,] heightMap, float heightMultiplier, AnimationCurve _meshHeightCurve, int levelOfDetail){

		AnimationCurve meshHeightCurve = new AnimationCurve (_meshHeightCurve.keys);

		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		int vertexIndex = 0;

		int simplifiMesh;

		if(levelOfDetail == 0){
			simplifiMesh = 1;
		}else{
			simplifiMesh = levelOfDetail * 2;
		}

		int vertecPerLine = (width - 1) / simplifiMesh + 1;
		MeshData meshData = new MeshData (vertecPerLine, vertecPerLine);
		
		float topLeftX = (width - 1) / 2f;
		float topLeftZ = (height - 1) / 2f;

		for (int y = 0; y < height; y += simplifiMesh) {
			for (int x = 0; x < width; x += simplifiMesh) {
				//lock(meshHeightCurve){
					meshData.vertices [vertexIndex] = new Vector3 (topLeftX + x, meshHeightCurve.Evaluate(heightMap [x, y]) * heightMultiplier,topLeftZ - y);	
				//}

				meshData.uvs [vertexIndex] = new Vector2(x / (float)width, y / (float)height);
				if (x < width - 1 && y < height - 1) {
					meshData.addTraingle (vertexIndex, vertexIndex + vertecPerLine + 1, vertexIndex + vertecPerLine);
					meshData.addTraingle (vertexIndex + vertecPerLine + 1, vertexIndex, vertexIndex + 1);
				}
				vertexIndex++;
			}
		}
		return meshData;
	}
}

//get Meshdata
public class MeshData{
	public Vector3[] vertices;
	public int[]triangles;
	public Vector2[] uvs;
	int trangleIndex;

	public MeshData(int meshHeigth, int meshWidth){
		vertices = new Vector3[meshWidth * meshHeigth];
		triangles = new int[(meshWidth - 1) * (meshHeigth - 1) * 6];
		uvs = new Vector2[meshWidth * meshHeigth];
	}

	public void addTraingle(int a, int b, int c){
		triangles [trangleIndex] = a;
		triangles [trangleIndex+1] = b;
		triangles [trangleIndex+2] = c;
		trangleIndex += 3;
	}

	public Mesh CreateMesh(){
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals ();
		return mesh;
	}
}