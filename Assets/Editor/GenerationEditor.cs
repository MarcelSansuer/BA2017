using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (MapGenerator))]
public class GenerationEditor : Editor {

	public override void OnInspectorGUI(){
		MapGenerator mapGen = (MapGenerator)target;

		if(DrawDefaultInspector () && mapGen.autoUpdate == true){
			mapGen.GenerateMap ();
		}

		if(GUILayout.Button ("Generate")){
			mapGen.GenerateMap ();
		}
	}
}
