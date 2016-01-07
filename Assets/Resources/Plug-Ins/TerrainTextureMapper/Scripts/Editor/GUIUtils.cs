using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class GUIUtils {

	
	public static void Separator()
	{
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.LabelField("*************************", "*************************************************************");
		EditorGUILayout.EndHorizontal();
	}
	
	public static void AddBasicInformation(IItem item)
	{
		//bug inside GUI value cannot be NULL :-/
		if (string.IsNullOrEmpty(item.Name))
			item.Name = string.Empty;
		
		if (string.IsNullOrEmpty(item.Description))
			item.Description = string.Empty;
		
		if (string.IsNullOrEmpty(item.SystemDescription))
			item.SystemDescription = string.Empty;
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("ID");
			
		
		item.ID = EditorGUILayout.IntField(item.ID, GUILayout.Width(300));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.PrefixLabel("Name");
		item.Name = EditorGUILayout.TextField(item.Name, GUILayout.Width(300));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.PrefixLabel("Description");
		item.Description = EditorGUILayout.TextField(item.Description, GUILayout.Width(600));
		EditorGUILayout.EndHorizontal();
		
		
	}
	
	
	public static int NewAttributeID<T>(List<T> items)
	{
		int maximum = 0;
		foreach(IItem p in items)
		{
			if (p.ID > maximum)
				maximum = p.ID;
		}
		maximum++; 
		return maximum;
	}
}
