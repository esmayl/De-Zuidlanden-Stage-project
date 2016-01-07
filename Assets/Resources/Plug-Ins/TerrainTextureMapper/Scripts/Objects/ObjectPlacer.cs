using UnityEngine;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class ObjectPlacer
{
	public GameObject Prefab;
	
	public float MinSize;
	public float MaxSize;
	public int Amount;
	
	public bool RotateX;
	public bool RotateY;
	public bool RotateZ;
	
	public ObjectPlacer()
	{
		MinSize = 1;
		MaxSize = 1;
	}
	  
	
}

