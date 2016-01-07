using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(BasicTerrainMapper))]
public class BasicTerrainMapperEditor : Editor
{
	private BasicTerrainMapper mapper;
	
	
	private TerrainData sourceTerrain;
	private TerrainData targetTerrain;
	
	
	public override void OnInspectorGUI()
	{
		mapper = (BasicTerrainMapper) target as BasicTerrainMapper;
		mapper.InitMapper();
		
		EditorGUIUtility.LookLikeControls();
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Textures", GUILayout.Width(80))) 
			mapper.Menu = MenuType.Textures;
		if (GUILayout.Button("Trees", GUILayout.Width(80))) 
			mapper.Menu = MenuType.Trees;
		
		if (GUILayout.Button("Details", GUILayout.Width(80))) 
			mapper.Menu = MenuType.Details;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Objects", GUILayout.Width(80))) 
			mapper.Menu = MenuType.Objects;
		
		if (GUILayout.Button("Single obj. placer", GUILayout.Width(160)))
			mapper.Menu = MenuType.SingleTerrainPlace;
		EditorGUILayout.EndHorizontal(); 
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Multi obj. placer", GUILayout.Width(160)))
			mapper.Menu = MenuType.MultiTerrainPlacer;
		
		if (GUILayout.Button("Terrains", GUILayout.Width(80))) 
			mapper.Menu = MenuType.Terrains;
		
		EditorGUILayout.EndHorizontal(); 
		
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Refresh", GUILayout.Width(80)))
		{
			InitMapper();
		}
		EditorGUILayout.EndHorizontal(); 
		
		switch(mapper.Menu)
		{
			case MenuType.Textures: Textures();
				break;
			case MenuType.Trees: Trees();
				break;
			case MenuType.Details: Details();	
				break;
			case MenuType.Objects: Objects();
				break;
			case MenuType.Terrains: Terrains();
				break;
			case MenuType.SingleTerrainPlace: SinglePlace();
				break;
			case MenuType.MultiTerrainPlacer: MultiPlacer();
				break;
		}
	}
	
	void MultiPlacer()
	{
		if (mapper.MultiPlacer == null)
		{
			InitMapper();
		}
		
		mapper.MultiPlacer.Amount = EditorUtils.IntField(mapper.MultiPlacer.Amount, "Amount");
		
		int previousAmount = mapper.MultiPlacer.ObjectsToPlaceCount;
		mapper.MultiPlacer.ObjectsToPlaceCount = EditorUtils.IntField(mapper.MultiPlacer.ObjectsToPlaceCount, "Object count");
		
		if (previousAmount != mapper.MultiPlacer.ObjectsToPlaceCount)
		{
			mapper.MultiPlacer.Objects = new List<MultiObjectSource>();
			for(int x = 0; x < mapper.MultiPlacer.ObjectsToPlaceCount; x++)
			{
				mapper.MultiPlacer.Objects.Add(new MultiObjectSource());
			}
		}
		
		foreach(MultiObjectSource source in mapper.MultiPlacer.Objects)
		{
			EditorGUILayout.BeginHorizontal();
			if (source.Prefab != null)
			{
				source.IsDisplayed = EditorGUILayout.Foldout(source.IsDisplayed , source.Prefab.name);
			}
			else
			{
				source.IsDisplayed = EditorGUILayout.Foldout(source.IsDisplayed , "");	
			}
			
			EditorGUILayout.EndHorizontal();
			
			if (source.IsDisplayed)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
			
				EditorGUILayout.PrefixLabel("Prefab"); 
				source.Prefab = EditorGUILayout.ObjectField(source.Prefab, typeof(GameObject), true) as GameObject;
				EditorGUILayout.EndHorizontal(); 
		
				source.MinSize = EditorUtils.FloatField(source.MinSize, "Min scale");
			
				source.MaxSize = EditorUtils.FloatField(source.MaxSize, "Max scale");
			
				source.RotateX = EditorUtils.Toggle(source.RotateX, "Rotate X");
			
				source.RotateY = EditorUtils.Toggle(source.RotateY, "Rotate Y");
		
				source.RotateZ = EditorUtils.Toggle(source.RotateZ, "Rotate Z");
			}
		}
		
		EditorUtils.Label("Textures");
		
		foreach(ObjectTextureSettings ots in mapper.MultiPlacer.Textures)
		{
			ots.IsUsed = EditorUtils.Toggle(ots.IsUsed, ots.TextureName);
			
			if (ots.IsUsed)
			{
				ots.AlphaMapValue = EditorUtils.Slider(ots.AlphaMapValue, 0.0f, 1.0f, "Alpha map value");
			}
		}
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Place", GUILayout.Width(150))) 
		{
			if (mapper.Objects.MinSize > mapper.Objects.MaxSize)
				return;
			
			Undo.RegisterSceneUndo("Mass place random objects");
			mapper.PlaceObjects();
		}
		EditorGUILayout.EndHorizontal();
	}
	
	void SinglePlace()
	{
		EditorUtils.Label("Single object placer");
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
			
		EditorGUILayout.PrefixLabel("Prefab"); 
		mapper.ObjectToPlace = EditorGUILayout.ObjectField(mapper.ObjectToPlace, typeof(GameObject), true) as GameObject;
		EditorGUILayout.EndHorizontal();
		
		mapper.RotateX = EditorUtils.Toggle(mapper.RotateX, "Rotate X");
		
		mapper.RotateY = EditorUtils.Toggle(mapper.RotateY, "Rotate Y");
		
		mapper.RotateZ = EditorUtils.Toggle(mapper.RotateX, "Rotate Z");
		
		mapper.MinScale = EditorUtils.FloatField(mapper.MinScale, "Min scale");
		
		mapper.MaxScale = EditorUtils.FloatField(mapper.MaxScale, "Max scale");
		
		mapper.YRange = EditorUtils.FloatField(mapper.YRange, "Y range");
		
		if (mapper.PlaceEnable)
		{
			if (GUILayout.Button("Place enable"))
			{
				mapper.PlaceEnable = true;
			}
		}
		else
		{
			if (GUILayout.Button("Place disable"))
			{
				mapper.PlaceEnable = false;
			}
		}
	}
	
	void OnSceneGUI()   
	{
		if (mapper == null || !mapper.PlaceEnable)
			return;
		
		Event currentEvent = Event.current; 
		if(currentEvent.isKey && (currentEvent.character == 'r' || currentEvent.character == 'R'))
		{
			Vector3 position = GetTerrainCollisionInEditor(currentEvent);
			
			if (position != Vector3.zero && mapper.ObjectToPlace != null) 
			{
				Quaternion rotation = Utils.GetRotation(mapper.RotateX, mapper.RotateZ, mapper.RotateY);
				position.y -= mapper.YRange;
				
				GameObject go = (GameObject)Instantiate(mapper.ObjectToPlace, position, rotation);
				
				float size = UnityEngine.Random.Range(mapper.MinScale, mapper.MaxScale);
				go.transform.localScale = new Vector3(size ,size, size);	
				
			}
		} 
	}
	
	void InitMapper()
	{
		mapper.mapperInitialized = false;
		mapper.InitMapper();
	}
	
	void Textures()
	{
		if (mapper.Textures == null)
		{
			InitMapper();
		}
		
		EditorUtils.Label("Textures");
		
		mapper.Textures.TextureCount = EditorUtils.IntField(mapper.Textures.TextureCount, "Textures count", 50 ,FieldTypeEnum.BeginningOnly);
		if (GUILayout.Button("Create", GUILayout.Width(100)))
		{
			mapper.Textures.AddDefaultAmount();
		}
		    
		EditorGUILayout.EndHorizontal();
		
		//min and max value
		foreach(TextureGroup tg in mapper.Textures.Textures)
		{
			if (tg.Index == 0)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
			
				EditorGUILayout.PrefixLabel("Group " + tg.Index); 

				float min = 0.0f;
                EditorGUILayout.FloatField(min);
				EditorGUILayout.MinMaxSlider(ref min, ref tg.EndPoint, 0.0f, 1.0f, GUILayout.Width(200));
                EditorGUILayout.FloatField(tg.EndPoint);
                
				if (GUILayout.Button("Delete", GUILayout.Width(100)))
				{
					mapper.Textures.Delete(tg);
					break;
				}
				EditorGUILayout.EndHorizontal();
				if (mapper.Textures.IsLast(tg.Index))
					break;
			}
			
			if (mapper.Textures.IsLast(tg.Index))
			{
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
			
				EditorGUILayout.PrefixLabel("Group " + tg.Index); 

				float max = 1.0f;
                EditorGUILayout.FloatField(tg.StartPoint);
				EditorGUILayout.MinMaxSlider(ref tg.StartPoint, ref max, 0.0f, 1.0f, GUILayout.Width(200));
                EditorGUILayout.FloatField(max);
                tg.EndPoint = max;

				if (GUILayout.Button("Delete", GUILayout.Width(100)))
				{	
					mapper.Textures.Delete(tg);
					break;
				}
				EditorGUILayout.EndHorizontal();
			}
			
			if (tg.Index > 0 && !mapper.Textures.IsLast(tg.Index))
			{
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
			
				EditorGUILayout.PrefixLabel("Group " + tg.Index );
                EditorGUILayout.FloatField(tg.StartPoint);
				EditorGUILayout.MinMaxSlider(ref tg.StartPoint,ref tg.EndPoint, 0.0f, 1.0f, GUILayout.Width(200));
                EditorGUILayout.FloatField(tg.EndPoint);

				if (GUILayout.Button("Delete", GUILayout.Width(100)))
				{
					mapper.Textures.Delete(tg);
					break;
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
			
		if (GUILayout.Button("Add texture group", GUILayout.Width(200)))
		{
			mapper.Textures.Add();
		}
		EditorGUILayout.EndHorizontal();
		
		
		int[] textureID = mapper.TexturesIndex();
		string[] texturesName = mapper.TexturesNames();
		
		//texture settings
		foreach(TextureGroup tg in mapper.Textures.Textures)
		{
			EditorGUILayout.BeginHorizontal();
			tg.Display = EditorGUILayout.Foldout(tg.Display, "Group " + tg.Index);
			EditorGUILayout.EndHorizontal();
			
			if (tg.Display)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
			
				EditorGUILayout.PrefixLabel("Texture ");
				tg.Texture1 = EditorGUILayout.IntPopup(tg.Texture1,texturesName, textureID ,GUILayout.Width(150)); 
				EditorGUILayout.EndHorizontal();
			
				tg.TwoTextures = EditorUtils.Toggle(tg.TwoTextures, "Two textures");
			
				if (tg.TwoTextures)
				{
					EditorGUILayout.Separator();
					EditorGUILayout.BeginHorizontal();
			
					EditorGUILayout.PrefixLabel("Texture 2");
					tg.Texture2 = EditorGUILayout.IntPopup(tg.Texture2,texturesName, textureID ,GUILayout.Width(150)); 
					EditorGUILayout.EndHorizontal();
				}
			}
		}
		bool result = mapper.Textures.Validate();
		
		if (result)
		{
			if (GUILayout.Button("Apply textures", GUILayout.Width(150))) 
			{
				mapper.PlaceTextures();
			}
		}
		else
		{
			EditorUtils.Label(mapper.Textures.ErrorMessage);
		}
	}
	
	//trees
	void Trees()
	{
		if (mapper.TreePlacement == null)
		{
			InitMapper();
		}
		
		EditorUtils.Label("Basic info");
		
		mapper.TreePlacement.TreeSize = EditorUtils.Slider(mapper.TreePlacement.TreeSize, 0.2f, 5f, "Tree size"); 
		
		mapper.TreePlacement.TreeVariation = EditorUtils.Slider(mapper.TreePlacement.TreeVariation, 0.2f, 5f, "Size variation"); 
		
		mapper.TreePlacement.WidthVariation = EditorUtils.Slider(mapper.TreePlacement.WidthVariation, 0.2f, 5f, "Width variation"); 
		
		mapper.TreePlacement.MinimumHeight = (int)EditorUtils.Slider(mapper.TreePlacement.MinimumHeight, 0, 300, "Minimum height"); 
			
		
		TreeTextureSettings tree;
		for(int index = 0; index < mapper.terrainData.splatPrototypes.Length; index++)
		{
			tree = mapper.TreePlacement.TreeTextures[index];
			tree.Index = index;
			
			EditorGUILayout.BeginHorizontal();
			tree.Display = EditorGUILayout.Foldout(tree.Display, mapper.terrainData.splatPrototypes[index].texture.name);
			EditorGUILayout.EndHorizontal();
			
			if (tree.Display)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("All trees", GUILayout.Width(150)))
				{
					tree.CheckAllTrees();
				}
				if (GUILayout.Button("Clear selection", GUILayout.Width(200)))
				{
					tree.UncheckAllTrees();
				}
		
				EditorGUILayout.EndHorizontal(); 
				
				tree.AlphamapValue = EditorUtils.FloatField(tree.AlphamapValue, "Alpha value");
			
				tree.TreeDistance = EditorUtils.IntField(tree.TreeDistance, "Distance");
			
				for(int treeIndex = 0;  treeIndex < mapper.terrainData.treePrototypes.Length; treeIndex++)
				{
					TreeSettings treeSettings = tree.Trees[treeIndex];
					treeSettings.Index = treeIndex;
					EditorGUILayout.Separator();
					EditorGUILayout.BeginHorizontal();
		
					EditorGUILayout.PrefixLabel(treeSettings.Name);
					treeSettings.IsUsed =  EditorGUILayout.Toggle(treeSettings.IsUsed, GUILayout.Width(30));
					EditorGUILayout.EndHorizontal(); 
				}
			}
			
		}
		
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Place trees", GUILayout.Width(150))) 
		{
			TreePlacer.ProgressDelegate progress = new TreePlacer.ProgressDelegate(updateProgress);
			//placing trees
			mapper.PlaceTrees(progress);
			
			EditorUtility.ClearProgressBar();
		}
		
		if (GUILayout.Button("Delete", GUILayout.Width(100))) 
		{
			mapper.DeleteTrees();
		}
		
		EditorGUILayout.EndHorizontal();
	}
	
	void Details()
	{
		if (mapper.DetailPlacement == null)
		{
			InitMapper();
		}
		
		for(int index = 0; index < mapper.terrainData.splatPrototypes.Length; index++)
		{
			DetailTextureSettings texture = mapper.DetailPlacement.Textures[index];
			texture.Index = index;
			
			EditorGUILayout.BeginHorizontal();
			texture.Display = EditorGUILayout.Foldout(texture.Display, mapper.terrainData.splatPrototypes[index].texture.name);
			EditorGUILayout.EndHorizontal();
			
			if (texture.Display)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("All details", GUILayout.Width(150)))
				{
					texture.CheckAllDetails();
				}
				
				if (GUILayout.Button("Clear selection", GUILayout.Width(200)))
				{
					texture.UncheckAllDetails();
				}
		
				EditorGUILayout.EndHorizontal();
				
			
				for(int detailIndex = 0; detailIndex < mapper.terrainData.detailPrototypes.Length; detailIndex++)
				{
					DetailSettings ds = texture.Details[detailIndex];
					ds.Index = detailIndex;
					EditorGUILayout.Separator();
					EditorGUILayout.BeginHorizontal();
					string detailName = string.Empty;
					if (mapper.terrainData.detailPrototypes[detailIndex].prototypeTexture != null)
					{
						detailName = mapper.terrainData.detailPrototypes[detailIndex].prototypeTexture.name;
					}
					else
					{
						detailName = mapper.terrainData.detailPrototypes[detailIndex].prototype.name;
					}
					EditorGUILayout.PrefixLabel(detailName);
					ds.IsUsed =  EditorGUILayout.Toggle(ds.IsUsed, GUILayout.Width(30));
				
					if (ds.IsUsed)
					{
						EditorGUILayout.PrefixLabel("Intensity");
						ds.Intensity =  EditorGUILayout.IntField(ds.Intensity, GUILayout.Width(30));
					}
			
					EditorGUILayout.EndHorizontal(); 
					
					if (ds.IsUsed)
					{
						EditorGUILayout.Separator();
						EditorGUILayout.BeginHorizontal();
			
						EditorGUILayout.PrefixLabel("Coverage in %");
						ds.AlphamapValue = EditorGUILayout.Slider(ds.AlphamapValue, 0.0f, 1.0f);
			
						EditorGUILayout.EndHorizontal(); 
						
						EditorGUILayout.Separator();
						EditorGUILayout.BeginHorizontal();
			
						EditorGUILayout.PrefixLabel("Height"); 
						EditorGUILayout.MinMaxSlider(ref ds.MinHeight, ref ds.MaxHeight, 0,2000);
					
						EditorGUILayout.EndHorizontal(); 
						
						if (!ds.Rare)
						{
							EditorGUILayout.Separator();
							EditorGUILayout.BeginHorizontal();
			
							EditorGUILayout.PrefixLabel("Fluent");
							ds.FluentCoverage = EditorGUILayout.Toggle(ds.FluentCoverage);
							if (ds.FluentCoverage)
							{
								ds.Smoothness = EditorGUILayout.IntSlider(ds.Smoothness,4,10);
							}
							EditorGUILayout.EndHorizontal();
						}
						
						if (!ds.FluentCoverage)
						{
							EditorGUILayout.Separator();
							EditorGUILayout.BeginHorizontal();
						
							EditorGUILayout.PrefixLabel("Rare");
							ds.Rare =  EditorGUILayout.Toggle(ds.Rare, GUILayout.Width(30));
							if (ds.Rare)
							{
								EditorGUILayout.PrefixLabel("1 : ");
								ds.ChanceToFind =  EditorGUILayout.IntField(ds.ChanceToFind, GUILayout.Width(30));
							}
							EditorGUILayout.EndHorizontal(); 
						}
					}
				}
			}
		}
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Place details", GUILayout.Width(150))) 
		{
			BasicTerrainMapper.ProgressDetailsDelegate progress = new BasicTerrainMapper.ProgressDetailsDelegate(updateProgress);
			Undo.RegisterUndo(mapper.terrainData, "Mass detail placement");
			mapper.PlaceDetails(progress);
			EditorUtility.ClearProgressBar();
		}
		
		EditorGUILayout.EndHorizontal();; 
		
	}
	
	//game objects
	void Objects()
	{
		if (mapper.Objects == null)
		{
			mapper.mapperInitialized = false;
			mapper.InitMapper();
		}
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
			
		EditorGUILayout.PrefixLabel("Prefab"); 
		mapper.Objects.Prefab = EditorGUILayout.ObjectField(mapper.Objects.Prefab, typeof(GameObject), true) as GameObject;
		EditorGUILayout.EndHorizontal(); 
		
		mapper.Objects.Amount = EditorUtils.IntField(mapper.Objects.Amount, "Amount");
		
		mapper.Objects.MinSize = EditorUtils.FloatField(mapper.Objects.MinSize, "Min scale");
		
		mapper.Objects.MaxSize = EditorUtils.FloatField(mapper.Objects.MaxSize, "Max scale");
		
		mapper.Objects.RotateX = EditorUtils.Toggle(mapper.Objects.RotateX, "Rotate X");
		
		mapper.Objects.RotateY = EditorUtils.Toggle(mapper.Objects.RotateY, "Rotate Y");
		
		mapper.Objects.RotateZ = EditorUtils.Toggle(mapper.Objects.RotateZ, "Rotate Z");
		
		
		EditorUtils.Label("Textures");
		
		foreach(ObjectTextureSettings ots in mapper.MultiPlacer.Textures)
		{
			ots.IsUsed = EditorUtils.Toggle(ots.IsUsed, ots.TextureName);
			
			if (ots.IsUsed)
			{
				ots.AlphaMapValue = EditorUtils.Slider(ots.AlphaMapValue, 0.0f, 1.0f, "Alpha map value");
			}
		}
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Place", GUILayout.Width(150))) 
		{
			if (mapper.Objects.MinSize > mapper.Objects.MaxSize)
				return;
			
			Undo.RegisterSceneUndo("Mass place random objects");
			mapper.PreparePlacer();
			mapper.PlaceObjects();
		}
		EditorGUILayout.EndHorizontal();
	}
	
	//copying textures / details and tree prototypes
	void Terrains()
	{
		EditorUtils.Label("Terrains");
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		
		
		EditorGUILayout.PrefixLabel("Source terrain"); 
		sourceTerrain = EditorGUILayout.ObjectField(sourceTerrain, typeof(TerrainData), true) as TerrainData;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
			
		EditorGUILayout.PrefixLabel("Target terrain"); 
		targetTerrain = EditorGUILayout.ObjectField(targetTerrain, typeof(TerrainData), true) as TerrainData;
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();

        Undo.RegisterUndo(targetTerrain, "Copying trees, details and textures");
        if (GUILayout.Button("Copy trees / details / textures"))
        {
			if (targetTerrain == null || sourceTerrain == null)
				return;
            targetTerrain.detailPrototypes = sourceTerrain.detailPrototypes;
            targetTerrain.treePrototypes = sourceTerrain.treePrototypes;
            targetTerrain.splatPrototypes = sourceTerrain.splatPrototypes;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Only trees"))
        {
			if (targetTerrain == null || sourceTerrain == null)
				return;
            targetTerrain.treePrototypes = sourceTerrain.treePrototypes;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Only details"))
        {
			if (targetTerrain == null || sourceTerrain == null)
				return;
            targetTerrain.detailPrototypes = sourceTerrain.detailPrototypes;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Only textures"))
        {
			if (targetTerrain == null || sourceTerrain == null)
				return;
            targetTerrain.splatPrototypes = sourceTerrain.splatPrototypes;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add trees"))
        {
			if (targetTerrain == null || sourceTerrain == null)
				return;
            TreePrototype[] newArray = new TreePrototype[targetTerrain.treePrototypes.Length + sourceTerrain.treePrototypes.Length];

            int currentIndex = 0;
            foreach (TreePrototype tp in targetTerrain.treePrototypes)
            {
                newArray[currentIndex] = tp;
                currentIndex++;
            }

            foreach (TreePrototype tp in sourceTerrain.treePrototypes)
            {
                newArray[currentIndex] = tp;
                currentIndex++;
            }
            targetTerrain.treePrototypes = newArray;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add details"))
        {
			if (targetTerrain == null || sourceTerrain == null)
				return;
            DetailPrototype[] newArray = new DetailPrototype[targetTerrain.detailPrototypes.Length + sourceTerrain.detailPrototypes.Length];

            int currentIndex = 0;
            foreach (DetailPrototype tp in targetTerrain.detailPrototypes)
            {
                newArray[currentIndex] = tp;
                currentIndex++;
            }

            foreach (DetailPrototype tp in sourceTerrain.detailPrototypes)
            {
                newArray[currentIndex] = tp;
                currentIndex++;
            }
            targetTerrain.detailPrototypes = newArray;
        }


        EditorGUILayout.EndHorizontal();
	}
	
	public void updateProgress(string titleString, string displayString, float percentComplete) 
	{
		EditorUtility.DisplayProgressBar(titleString, displayString, percentComplete);
	}
	
	public Vector3 GetTerrainCollisionInEditor(Event currentEvent)
	{
		Camera SceneCameraReceptor = new Camera();
		
		Terrain terComponent = (Terrain) mapper.GetComponent(typeof(Terrain));
		
		TerrainCollider terCollider = (TerrainCollider)terComponent.GetComponent(typeof(TerrainCollider));
		
		Ray terrainRay = new Ray();
		
		if(Camera.current != null)
		{
			SceneCameraReceptor = Camera.current;
		
			RaycastHit raycastHit = new RaycastHit();
			Vector2 newMousePosition = new Vector2(currentEvent.mousePosition.x, Screen.height - (currentEvent.mousePosition.y + 25));
			
			terrainRay = SceneCameraReceptor.ScreenPointToRay(newMousePosition);
			
			if(terCollider.Raycast(terrainRay, out raycastHit, Mathf.Infinity))
			{
				return raycastHit.point;
			}
			else
			{
				Debug.LogError("Error: No collision with terrain to create node");
			}
		}
		
		return Vector3.zero;
	} 
}

