using UnityEngine;
using System.Collections;

public class ObjectTextureSettings 
{
	public bool IsUsed;
	public string TextureName;
	public int Index;
	public float AlphaMapValue;
	
	public ObjectTextureSettings()
	{
		AlphaMapValue = 0.8f;
	}
}
