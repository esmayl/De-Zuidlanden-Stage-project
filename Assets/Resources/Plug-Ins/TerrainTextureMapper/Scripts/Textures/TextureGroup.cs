using UnityEngine;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class TextureGroup
{
	[XmlIgnore]
	public bool Display;
	[XmlIgnore]
	public string ErrorMessage;
	
	public float StartPoint;
	public float EndPoint;
	public bool TwoTextures;
	public int Texture1;
	public string Texture1Name;
	public int Texture2;
	public bool Randomize;
	public float Texture1Strength;
	public int Index;
	
	public TextureGroup()
	{
		Texture1Strength = 0.5f;
		Display = true;
	}
	
	public bool Validate()
	{
		if (TwoTextures && Texture2 == 0)
		{
			ErrorMessage = "Second texture empty";
			return false;
		}
		else if (!TwoTextures && Texture1 == Texture2)
		{
			ErrorMessage = "Same textures!";
			return false;
		}
		return true;
	}
}
