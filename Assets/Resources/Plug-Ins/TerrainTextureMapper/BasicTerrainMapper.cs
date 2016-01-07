using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class BasicTerrainMapper : MonoBehaviour {
	
	public delegate void ProgressDetailsDelegate(string titleString, string displayString, float percentComplete);
	
	public TerrainData terrainData;
	
	private List<TreeInstance> Trees;
	
	public TextureMapper Textures;
	public DetailPlacer DetailPlacement;
	public ObjectPlacer Objects;
	public TreePlacer TreePlacement;
	public MultiObjectPlacer MultiPlacer;
	
	float xUnit;
	float yUnit;
	
	//single terrain place
	public GameObject ObjectToPlace;
	public bool RotateY;
	public bool RotateX;
	public bool RotateZ;
	public float MinScale;
	public float MaxScale;
	public float YRange;
	public bool PlaceEnable = false;
	
	public MenuType Menu;
	public bool mapperInitialized;
	
	public void InitMapper()
	{
		if (mapperInitialized)
			return;
		
		Init();
		
		PrepareObjects();
		
		PrepareDetails();
		
		PrepareTrees();
		
		PrepareMultiObjects();
		
		Textures = new TextureMapper();
	}
	
	public void PrepareObjects()
	{
		MultiPlacer.Textures = new List<ObjectTextureSettings>();
		int index = 0;
		foreach(SplatPrototype splat in terrainData.splatPrototypes)
		{
			ObjectTextureSettings o = new ObjectTextureSettings();
			o.TextureName = splat.texture.name;
			o.Index = index;
			index++;
			MultiPlacer.Textures.Add(o);
		}
	}
	
	public void PrepareMultiObjects()
	{
		MultiPlacer = new MultiObjectPlacer();
		MultiPlacer.Textures = new List<ObjectTextureSettings>();
		MultiPlacer.ObjectsToPlaceCount = 2;
		MultiPlacer.Objects = new List<MultiObjectSource>();
		for(int x = 0; x < MultiPlacer.ObjectsToPlaceCount; x++)
		{
			MultiPlacer.Objects.Add(new MultiObjectSource());
		}
		int index = 0;
		foreach(SplatPrototype splat in terrainData.splatPrototypes)
		{
			ObjectTextureSettings o = new ObjectTextureSettings();
			o.TextureName = splat.texture.name;
			o.Index = index;
			index++;
			MultiPlacer.Textures.Add(o);
		}
	}
	
	public void PrepareTrees()
	{
		TreePlacement.TreeTextures = new List<TreeTextureSettings>();
		for(int index = 0; index < terrainData.splatPrototypes.Length; index++)
		{
			TreeTextureSettings s = new TreeTextureSettings();
			TreePlacement.TreeTextures.Add(s);
			foreach(TreePrototype tree in terrainData.treePrototypes)
			{
				TreeSettings t = new TreeSettings();
				t.Name = tree.prefab.name;
				t.IsUsed = false;
				s.Trees.Add(t);
			}
		}
		TreePlacement.TreeVariation = 0.2f;
		TreePlacement.TreeSize = 1.0f;
	}
	
	public void PrepareDetails()
	{
		DetailPlacement.Textures = new List<DetailTextureSettings>();
		
		for(int splatIndex = 0; splatIndex < terrainData.splatPrototypes.Length; splatIndex++)
		{
			DetailTextureSettings s = new DetailTextureSettings();
			DetailPlacement.Textures.Add(s);
			for(int index = 1; index <= terrainData.detailPrototypes.Length; index++)
			{
				s.Details.Add(new DetailSettings());
			}
		}
	}
	
	private void Init()
	{
		Terrain terComponent = (Terrain)GetComponent(typeof(Terrain));
		terrainData = terComponent.terrainData; 
		
		DetailPlacement = new DetailPlacer();
		DetailPlacement.Textures = new List<DetailTextureSettings>();
		Objects = new ObjectPlacer();
		TreePlacement = new TreePlacer();
		DetailPlacement = new DetailPlacer();
		Textures = new TextureMapper();
		MultiPlacer = new MultiObjectPlacer();
		
		MinScale = 0.8f;
		MaxScale = 1.2f;
		RotateY = true;
		mapperInitialized = true;
		PlaceEnable = true;
	}
	
	public int[] TexturesIndex()
	{
		Terrain ter = (Terrain) GetComponent(typeof(Terrain));
		terrainData = ter.terrainData;
		
		
		SplatPrototype[] prototypes =  terrainData.splatPrototypes;
		int[] ID = new int[prototypes.Length];
		
		for(int index = 0; index < prototypes.Length; index++)
		{
			ID[index] = index;
		} 
		return ID;
	}
	
	public string[] TexturesNames()
	{
		Terrain ter = (Terrain) GetComponent(typeof(Terrain));
		terrainData = ter.terrainData;
		
		int index = 0;
		SplatPrototype[] prototypes =  terrainData.splatPrototypes;
		string[] ID = new string[prototypes.Length];
		
		foreach(SplatPrototype item in prototypes)
		{
			ID[index] = item.texture.name;
			index++;
		} 
		return ID;
	}
	
	public void PlaceDetails(ProgressDetailsDelegate progressDelegate)
	{
		Terrain ter = (Terrain) GetComponent(typeof(Terrain));
		terrainData = ter.terrainData;
		
		int alphamapWidth = terrainData.alphamapWidth;
		int alphamapHeight = terrainData.alphamapHeight;
		
		int detailWidth = terrainData.detailResolution;
		int detailHeight = detailWidth;
		
		float resolutionDiffFactor = (float)alphamapWidth/detailWidth;
		float[,,] splatmap = terrainData.GetAlphamaps(0,0,alphamapWidth,alphamapHeight);
		float alphaValue = 0;
		int count = terrainData.detailPrototypes.Length * DetailPlacement.Textures.Count;
		float increment = 1 / count;
		float percentCompleted = 0.0f;
		//how big is one unit in real world
		xUnit = (float)terrainData.size.x / (float)(detailWidth);
		yUnit = (float)terrainData.size.z / (float)(detailHeight);
		System.Random random = new System.Random();
		int randomInt;
		for(int detailIndex = 0; detailIndex < terrainData.detailPrototypes.Length; detailIndex++)
		{
			int[,] newDetailLayer = new int[detailWidth,detailHeight];
			foreach(DetailTextureSettings texture in DetailPlacement.Textures)
			{
				DetailSettings ds = texture.Details[detailIndex];
				ds.MinimumHeight = (int)ds.MinHeight;
				ds.MaximumHeight = (int)ds.MaxHeight;
				percentCompleted += increment;
				progressDelegate("Placing details", "Placing details", percentCompleted);
				
				if (ds.IsUsed == false)
					continue;
				//find where the texture is present
				for (int j = 0; j < detailWidth; j++) 
				{
					for (int k = 0; k < detailHeight; k++) 
					{
						alphaValue = splatmap[(int)(resolutionDiffFactor*j),(int)(resolutionDiffFactor*k),texture.Index];
						
						if (alphaValue >= ds.AlphamapValue && !ds.FluentCoverage)
						{
							//recalculate this position to real height
							//also check if minimum height is allowed for this texture
							if (CalculateHeight(ter, j, k, ds))
							{
								//rare plant is only low chance
								if (ds.Rare)
								{
									randomInt = random.Next(ds.ChanceToFind);
									if (randomInt == 1)
										newDetailLayer[j,k] = ds.Intensity;
									else
										newDetailLayer[j,k] = 0;
								}
								else
								{
									newDetailLayer[j,k] = ds.Intensity;
								}
							}
							
							else
							{
								newDetailLayer[j,k] = 0;
							}
						}
						else if (ds.FluentCoverage && alphaValue >= ds.AlphamapValue)
						{
							if (!CalculateHeight(ter, j, k, ds))
							{
								newDetailLayer[j,k] = 0;
								continue;
							}
							float wholeCoverage = 1 - ds.AlphamapValue;
							float partCoverage = 1 - alphaValue;
							int intensity = 1;
							
							if (wholeCoverage != 0 && partCoverage != 0)
							{
								float percent = (wholeCoverage * 100) / partCoverage - 100;
								intensity = (int)(ds.Intensity * percent / 100 / ds.Smoothness);
								if (intensity == 0)
									intensity = 1;
								if (intensity > ds.Intensity)
									intensity = ds.Intensity;
							}
							if (alphaValue == 1 || ds.AlphamapValue == 1)
								intensity = ds.Intensity;
							
							newDetailLayer[j,k] = intensity;
						}
					}
				
				}	
				
			}
			terrainData.SetDetailLayer(0,0,detailIndex,newDetailLayer);		
		}
	}
	
	private bool CalculateHeight(Terrain ter, int x, int y, DetailSettings ds)
	{
		if (ds.MinimumHeight == 0 && ds.MaximumHeight == 0)
			return true;
		
		float xDim = x * xUnit;
		float yDim = y * yUnit;
		Vector3 realPosition = new Vector3(yDim, 0, xDim);
		float positionY = ter.SampleHeight(realPosition);
		if (positionY < ds.MinimumHeight || positionY > ds.MaximumHeight)
		{
			return false;
		}
		return true;
	}
	
	public void DeleteTrees()
	{
		Terrain ter = (Terrain) GetComponent(typeof(Terrain));
		terrainData = ter.terrainData;
		terrainData.treeInstances = new TreeInstance[0];
	}
	
	public void PlaceTrees(TreePlacer.ProgressDelegate progressDelegate)
	{
		Terrain ter = (Terrain) GetComponent(typeof(Terrain));
		terrainData = ter.terrainData;
		

		TreePlacement.terrainData = terrainData;
		TreePlacement.terrain = ter;
		TreePlacement.PlaceTrees(progressDelegate);
	}
	
	public void PlaceTextures()
	{
		Terrain ter = (Terrain) GetComponent(typeof(Terrain));
		Textures.terrain = ter;
		Textures.TextureSplat();
	}
	
	public void PlaceObjects()
	{
		if (MultiPlacer.Objects.Count == 0)
			return;
		
		//calcule iteration number
		int maxIteration = MultiPlacer.Amount * 20;
		//determine splat map
		int alphamapWidth = terrainData.alphamapWidth;
		int alphamapHeight = terrainData.alphamapHeight;
		float[,,] splatMap = terrainData.GetAlphamaps(0,0,alphamapWidth,alphamapHeight);
		
		//determine dimensions
		//dimension of splatmaps
		int xDim = splatMap.GetUpperBound(0);
		int yDim = splatMap.GetUpperBound(1);
		
		float sizeConstantX = terrainData.size.x / xDim;
		float sizeConstantZ = terrainData.size.z / yDim;
		
		GameObject g = new GameObject();
		g.name = MultiPlacer.Objects[0].Prefab.name + " Parent";
		int currentAmount = 0;
		for(int index = 1; index <= maxIteration; index++)
		{
			MultiObjectSource source = MultiPlacer.GetPrefab;
			//get random position
			int x = Random.Range(0, xDim);
			int y = Random.Range(0, yDim);
			
			if (!Utils.IsCorrectTexture(x, y, MultiPlacer.Textures, splatMap))
				continue;
			
			//add to collection
			Vector3 position = new Vector3(x * sizeConstantX,0 , y * sizeConstantZ);
			position.y = Terrain.activeTerrain.SampleHeight(position);
			
			GameObject go = (GameObject)Instantiate(source.Prefab, position, Utils.GetRotation(source.RotateX, source.RotateZ, source.RotateY));
			go.transform.parent = g.transform;
			if (source.MaxSize != 1 || source.MinSize != 1)
			{
				float size = Random.Range(source.MinSize, source.MaxSize + 0.01f);
				go.transform.localScale = new Vector3(size ,size, size);	
			}
			go.name = source.Prefab.name;
			
			if (Utils.IsTouchingOtherCollider(position, go))
			{
				DestroyImmediate(go);
				continue;
			}
			
			currentAmount++;
			if (currentAmount == MultiPlacer.Amount)
				break;
		}
		
		Debug.Log("Objects placed " + currentAmount);
	}
	
	public void PreparePlacer()
	{
		MultiPlacer.ObjectsToPlaceCount = 1;
		MultiPlacer.Objects = new List<MultiObjectSource>();
		
		
		MultiObjectSource source = new MultiObjectSource();
		source.Prefab = Objects.Prefab;
		source.MinSize = Objects.MinSize;
		source.MaxSize = Objects.MaxSize;
		source.RotateX = Objects.RotateX;
		source.RotateY = Objects.RotateY;
		source.RotateZ = Objects.RotateZ;
		MultiPlacer.Amount = Objects.Amount;
		MultiPlacer.Objects.Add(source);
	}
}

public enum MenuType
{
	Textures = 0,
	Trees = 1,
	Details = 2,
	Objects = 3,
	Terrains = 4,
	SingleTerrainPlace = 5,
	MultiTerrainPlacer = 6
}