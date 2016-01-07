using UnityEngine;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System;

[XmlInclude(typeof(DetailSettings))]
public class DetailTextureSettings
{
	public List<DetailSettings> Details;
	public string TextureName;
	public int Index;
	public bool AllDetails;
	public bool Display;
	
	
	public DetailTextureSettings()
	{
		Details = new List<DetailSettings>();
	}
	
	public void CheckAllDetails()
	{
		foreach(DetailSettings ds in Details)
		{
			ds.IsUsed = true;
		}
	}
	
	public void UncheckAllDetails()
	{
		foreach(DetailSettings ds in Details)
		{
			ds.IsUsed = false;
		}
	}
}


