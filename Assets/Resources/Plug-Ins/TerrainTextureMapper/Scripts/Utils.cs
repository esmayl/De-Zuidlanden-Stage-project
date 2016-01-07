using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils
{
	public static Quaternion GetRotation(bool RotateX, bool RotateZ, bool RotateY)
	{
		int x = 0;
		int y = 0;
		int z = 0;
		if (RotateX)
		{
			x = Random.Range(0, 360);
		}
		if (RotateZ)
		{
			z = Random.Range(0, 360);
		}
		if (RotateY)
		{
			y = Random.Range(0, 360);
		}
		return Quaternion.Euler(new Vector3(x,y,z));
	}
	
	public static bool IsTouchingOtherCollider(Vector3 position, GameObject Prefab)
	{
		if (Prefab.collider == null)
			return false;
		
		
		float minX = Prefab.collider.bounds.min.x;
		float minZ = Prefab.collider.bounds.min.z;
		
		float maxX = Prefab.collider.bounds.max.x;
		float maxZ = Prefab.collider.bounds.max.z;
		
		float radiusX = (maxX - minX) / 2;
		float radiusZ = (maxZ - minZ) / 2;
		
		Collider[] colliders = Physics.OverlapSphere(position, radiusX);
		if (colliders.Length > 2)
		{
			return true;
		}
		
		return false;
	}
	
	public static bool IsCorrectTexture(int x, int y, List<ObjectTextureSettings> Textures, float[,,] splatMap)
	{
		foreach(ObjectTextureSettings b in Textures)
		{
			if (!b.IsUsed)
				continue;
			
			float alphaMap = splatMap[x,y,b.Index];
			if (alphaMap > b.AlphaMapValue)
				return true;
		}
		
		return false;
	}
}
