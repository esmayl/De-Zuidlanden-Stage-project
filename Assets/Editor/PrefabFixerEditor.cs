using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PrefabFixer))]
public class PrefabFixerEditor : Editor {

	public static GameObject[] objectsFound;

	static PrefabFixerEditor()
	{
		objectsFound = Resources.LoadAll<GameObject>("");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		base.DrawHeader();
		PrefabFixer instance = (PrefabFixer)target;

		if (GUILayout.Button("Fix") && objectsFound.Length>1) { instance.Fix(objectsFound); }
	}
}
