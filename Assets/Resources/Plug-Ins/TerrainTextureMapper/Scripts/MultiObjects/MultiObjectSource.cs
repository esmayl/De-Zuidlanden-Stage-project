using UnityEngine;
using System.Collections;

public class MultiObjectSource
{
	public bool IsDisplayed;
	public float MinSize;
	public float MaxSize;
	public GameObject Prefab;
	
	public bool RotateX;
	public bool RotateY;
	public bool RotateZ;
	
	public MultiObjectSource()
	{
		MinSize = 1;
		MaxSize = 1;
		RotateY = true;
	}
}

