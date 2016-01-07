using UnityEngine;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System;

[XmlInclude(typeof(DetailTextureSettings))]
public class DetailPlacer : IItem
{
	

	
	#region implementing interface IItem
	private int id;
	public int ID
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}
	
	private string _name;
	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}
	
	private string description;
	public string Description
	{
		get
		{
			return description;
		}
		set
		{
			description = value;
		}
	}
	
	private string systemDescription;
	public string SystemDescription
	{
		get
		{
			return systemDescription;
		}
		set
		{
			systemDescription = value;
		}
	}
	
	protected string preffix = "DETAILS";
	public string Preffix
	{
		get
		{
			return preffix;
		}
	}
	
	public string UniqueId
	{
		get
		{
			return preffix + ID.ToString();
		}
	}
	#endregion 
	
	public List<DetailTextureSettings> Textures;
	
	public DetailPlacer()
	{
		Textures = new List<DetailTextureSettings>();
	}
}
