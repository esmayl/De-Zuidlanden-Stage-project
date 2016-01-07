using UnityEngine;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;


public class MultiObjectPlacer
{
	public List<ObjectTextureSettings> Textures;
	public List<MultiObjectSource> Objects;
	
	public int Amount;
	
	public int ObjectsToPlaceCount;
	
	
	[XmlIgnore] 
	private float[,,] splatMap;
	
	public MultiObjectPlacer()
	{
		Objects = new List<MultiObjectSource>();
	}

	
	
	public MultiObjectSource GetPrefab
	{
		get
		{
			if (Objects.Count == 1)
				return Objects[0];
			
			int index = Random.Range(0, Objects.Count);
			return Objects[index];
		}
	}
}
