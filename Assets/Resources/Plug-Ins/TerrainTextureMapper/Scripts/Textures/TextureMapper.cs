using UnityEngine;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[XmlInclude(typeof(TextureGroup))]
public class TextureMapper : IItem
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
	
	protected string preffix = "TEXTURESMAP";
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
	
	public List<TextureGroup> Textures;
	
	[XmlIgnore]
	public int TextureCount;
	
	[XmlIgnore]
	public string ErrorMessage;
	
	public float Amount;
	
	[XmlIgnore]
	public Terrain terrain;
	
	public TextureMapper()
	{
		Textures = new List<TextureGroup>();
		TextureCount = 4;
	}
	
	public bool Validate()
	{
		TextureGroup current = new TextureGroup();;
		foreach(TextureGroup tg in Textures)
		{
			if (tg.Index == 0)
			{
				current = tg;
				continue;
			}
			
			if (!tg.TwoTextures && !current.TwoTextures && tg.Texture1 == current.Texture1)
			{
				ErrorMessage = current.Index + " " + tg.Index + " have same textures";
				return false;
			}
			if (tg.TwoTextures && current.TwoTextures && tg.Texture1 == current.Texture1 && tg.Texture2 == current.Texture2)
			{
				ErrorMessage = current.Index + " " + tg.Index + " have same textures";
				return false;
			}
			
			if (current.EndPoint > tg.StartPoint)
			{
				ErrorMessage = current.Index + " " + tg.Index + " have incorret intervals";
			}
			current = tg;
		}
		
		return true;
	}
	
	public void AddDefaultAmount()
	{
		Textures = new List<TextureGroup>();
		for(int index = 1; index <= TextureCount; index++)
		{
			Textures.Add(new TextureGroup());
		}
		
		AssignNumbers();
		
		DefaultRanges();
	}
	
	public void TextureSplat()
	{
		TerrainData terData = terrain.terrainData;
		
		int Tw = terData.heightmapWidth - 1;
		int Th = terData.heightmapHeight - 1;
		float[,] heightMapData = new float[Tw, Th];
		float[,,] splatMapData;
		int Px;
		int Py;
		
		terData.alphamapResolution = Tw;
		splatMapData = terData.GetAlphamaps(0, 0, Tw, Th);
		for (Py = 0; Py < Th; Py++)
		{
			for (Px = 0; Px < Tw; Px++)
			{
				for(int textureIndex = 0; textureIndex < terData.splatPrototypes.Length; textureIndex++)
				{
					splatMapData[Px, Py,textureIndex] = 0;
				}
			}
		}
		
		float greatestHeight = 0.0f;
		
		int xShift;
		int yShift;
		int xIndex;
		int yIndex;
		float[,] heightMap = terData.GetHeights(0, 0, Tw, Th);
		for (int Ty = 0; Ty < Th; Ty++) {
			// y...
			if (Ty == 0) {
				yShift = 0;
				yIndex = 0;
			} else if (Ty == Th - 1) {
				yShift = -1;
				yIndex = 1;
			} else {
				yShift = -1;
				yIndex = 1;
			}
			for (int Tx = 0; Tx < Tw; Tx++) {
				// x...
				if (Tx == 0) {
					xShift = 0;
					xIndex = 0;
				} else if (Tx == Tw - 1) {
					xShift = -1;
					xIndex = 1;
				} else {
					xShift = -1;
					xIndex = 1;
				}
				// Get height...
				float h = heightMap[Tx + xIndex + xShift, Ty + yIndex + yShift];
				if (h > greatestHeight) {
					greatestHeight = h;
				}
				// ...and apply to height map...
				heightMapData[Tx, Ty] = h;
			}
		}
		foreach(TextureGroup tg in Textures)
		{
			for (Py = 0; Py < Th; Py++)
			{
				for (Px = 0; Px < Tw; Px++)
				{
					float hBlendInMinimum = 0;
					float hBlendInMaximum = 0;
					float hBlendOutMinimum = 1;
					float hBlendOutMaximum = 1;
					
					if (tg.Index > 0)
					{
						hBlendInMinimum = Textures[tg.Index - 1].EndPoint;
						hBlendInMaximum = tg.StartPoint;
					}
					if (!IsLast(tg.Index))
					{
						hBlendOutMinimum = tg.EndPoint;
						hBlendOutMaximum = Textures[tg.Index + 1].StartPoint;
					}
					
					float hValue = heightMapData[Px, Py];
					float hBlended = 0;
					
					if (hValue < hBlendInMinimum || hValue > hBlendOutMaximum)
						continue;
						
					if (hValue >= hBlendInMaximum && hValue <= hBlendOutMinimum) {
							// Full...
							hBlended = 1;
					} else if (hValue >= hBlendInMinimum && hValue < hBlendInMaximum) {
						// Blend in...
						hBlended = (hValue - hBlendInMinimum) / (hBlendInMaximum - hBlendInMinimum);
					} else if (hValue > hBlendOutMinimum && hValue <= hBlendOutMaximum) {
							// Blend out...
						hBlended = 1 - ((hValue - hBlendOutMinimum) / (hBlendOutMaximum - hBlendOutMinimum));
					}
					
					if (tg.TwoTextures)
					{
						if (hBlended < 1)
						{
							float hPartialBlended;
							if (splatMapData[Px, Py, tg.Texture1] > 0 && splatMapData[Px, Py, tg.Texture2] >0)
							{
								continue;
							} 
							else if (splatMapData[Px, Py, tg.Texture1] > 0)
							{
								hPartialBlended = (1 - splatMapData[Px, Py, tg.Texture1]) / 2;
								
								splatMapData[Px, Py, tg.Texture1] +=  hPartialBlended;
								splatMapData[Px, Py, tg.Texture2] =  hPartialBlended;
							} 
							else if (splatMapData[Px, Py, tg.Texture2] > 0)
							{
								hPartialBlended = (1 - splatMapData[Px, Py, tg.Texture2]) / 2;
								
								splatMapData[Px, Py, tg.Texture1] =  hPartialBlended;
								splatMapData[Px, Py, tg.Texture2] +=  hPartialBlended;
							}
							else
							{
								hBlended = hBlended * 0.5f;
								splatMapData[Px, Py, tg.Texture1] =  hBlended;
								splatMapData[Px, Py, tg.Texture2] =  hBlended;
							}
						}
						else
						{
							hBlended = hBlended * 0.5f;
							splatMapData[Px, Py, tg.Texture1] =  hBlended;
							splatMapData[Px, Py, tg.Texture2] =  hBlended;	
						}
					}
					else
					{
						if (hBlended < 1)
						{
							float sum = 0;
							for(int textureIndex = 0; textureIndex < terData.splatPrototypes.Length; textureIndex++)
							{
								if (textureIndex != tg.Texture1)
									sum += splatMapData[Px, Py, textureIndex];
							}
							if (sum == 0)
							{
								splatMapData[Px, Py, tg.Texture1] = hBlended;
							}
							else
							{
								splatMapData[Px, Py, tg.Texture1] = 1 - sum;	
							}
							
						}
						else
						{
							splatMapData[Px, Py, tg.Texture1] = hBlended;
						}
					}
				}
			}
		}
		CorrectValue(splatMapData);
		CorrectValue(splatMapData);
		CorrectValue(splatMapData);
		terData.SetAlphamaps(0, 0, splatMapData);
	}
	
	private void CorrectValue(float[,,] splatMapData)
	{
		TerrainData terData = terrain.terrainData;
		
		int Tw = terData.heightmapWidth - 1;
		int Th = terData.heightmapHeight - 1;
		
		for(int x = 0; x < Tw; x++)
		{
			for(int y = 0; y < Th; y++)
			{
				float sum = 0;
				int count = 0;
				for(int textureIndex = 0; textureIndex < terData.splatPrototypes.Length; textureIndex++)
				{
					if (splatMapData[x, y, textureIndex] > 0)
					{
						sum += splatMapData[x, y, textureIndex];
						count++;
					}
				}
					
				if (sum != 1)
				{
					Correct(x, y, splatMapData, count, sum);			
				}
			}
		}
	}
	
	private void Correct(int x, int y, float[,,] splatMapData, int count, float sum)
	{
		TerrainData terData = terrain.terrainData;
		float difference = 0;
		if (sum > 1)
		{
			difference = sum - 1;
		}
		else
		{
			difference = 1 - sum;
		}
		difference = difference / count;
		
		for(int textureIndex = 0; textureIndex < terData.splatPrototypes.Length; textureIndex++)
		{
			if (splatMapData[x, y, textureIndex] > 0)
			{
				if (sum > 1)
				{
					splatMapData[x, y, textureIndex] -= difference;
				}
				else
				{
					splatMapData[x, y, textureIndex] += difference;
				}
			}
		}
	}
	
	public bool IsLast(int index)
	{
		if (Textures.Count == index + 1)
			return true;
		
		return false;
	}
	
	public void Add()
	{
		Textures.Add(new TextureGroup());
		
		AssignNumbers();
	}
	
	public void Delete(TextureGroup textureGroup)
	{
		foreach(TextureGroup tg in Textures)
		{
			if (tg == textureGroup)
			{
				Textures.Remove(tg);
				break;
			}
		}
		AssignNumbers();
	}
	
	private void AssignNumbers()
	{
		int index = 0;
		foreach(TextureGroup tg in Textures)
		{
			tg.Index = index;
			index++;
		}
	}
	
	private void DefaultRanges()
	{
		float defaultIndex = 1.0f / (float)((TextureCount * 2)-1);
		float currentIndex = defaultIndex;
		foreach(TextureGroup tg in Textures)
		{
			if (tg.Index == 0)
			{
				tg.EndPoint = currentIndex;
				currentIndex += defaultIndex;
				continue;
			}
			if (IsLast(tg.Index))
			{
				tg.StartPoint = currentIndex;
				continue;
			}
			
			tg.StartPoint = currentIndex;
			currentIndex += defaultIndex;
			tg.EndPoint = currentIndex;
			currentIndex += defaultIndex;
		}
	}
}
