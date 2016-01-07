using UnityEngine;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class TreePlacer
{
	public delegate void ProgressDelegate(string titleString, string displayString, float percentComplete);
	
	[XmlIgnore] 
	public TerrainData terrainData;
	[XmlIgnore] 
	public Terrain terrain;
	
	//settings for save
	public List<TreeTextureSettings> TreeTextures;
	public float TreeVariation;
	public float WidthVariation;
	public float TreeSize;
	public int MinimumHeight;
	
	[XmlIgnore] 
	private List<TreePosition> TreesPosition;
	
	[XmlIgnore] 
	float[,,] splatmap;
	//how big is one unit in real world
	
	[XmlIgnore] 
	float xUnit;
	[XmlIgnore] 
	float yUnit;
	
	//relative one unit to terrain transform
	[XmlIgnore] 
	float xRelativeUnit;
	[XmlIgnore] 
	float yRelativeUnit;
	
	//tree instances
	[XmlIgnore] 
	ArrayList instances = new ArrayList();
	
	public TreePlacer()
	{
		WidthVariation = 0.1f;
	}
	
	private bool Validate()
	{
		foreach(TreeTextureSettings ts in TreeTextures)
			ts.Prepare();
		return true;
	}
	
	
	
	public void PlaceTrees(ProgressDelegate progressDelegate)
	{
		if (!Validate())
			return;
		instances = new ArrayList();
		TreesPosition = new List<TreePosition>();
		//get alpha map width and height of terrain
		int alphamapWidth = terrainData.alphamapWidth;
		int alphamapHeight = terrainData.alphamapHeight;
		
		//get splat map of terrain
		splatmap = terrainData.GetAlphamaps(0,0,alphamapWidth,alphamapHeight);
		
		//dimension of splatmaps
		int xDim = splatmap.GetUpperBound(0);
		int yDim = splatmap.GetUpperBound(1);
		
		//how big is one unit in real world
		xUnit = (float)terrainData.size.x / (float)(xDim + 1);
		yUnit = (float)terrainData.size.z / (float)(yDim + 1);
		
		float percentPosition = 0.1f;
		progressDelegate("Placing trees", "Determining basic position", percentPosition);
		float calculation = 0.4f / xDim;
		for(int x = 0; x <= xDim; x ++)
		{
			for(int y = 0; y <= yDim; y ++)
			{
				foreach(TreeTextureSettings treeTexture in TreeTextures)
				{
					//this texture is not used for planting trees
					if (treeTexture.IsUsed == false)
						continue;
					
					float alphaMap = splatmap[x,y,treeTexture.Index];
					//this texture is not where we are currently in
					if (alphaMap < treeTexture.AlphamapValue)
						continue;
					
					//check distance from the nearest tree
					if (IsTreePositionClose(x, y, treeTexture.TreeDistance, treeTexture.Index))
					    continue;
					TreePosition position = new TreePosition();
					position.PosX = x;
					position.PosY = y;
					position.TreeIndex = treeTexture.DetermineTreePrototype;
					position.TextureIndex = treeTexture.Index;
					TreesPosition.Add(position);
				}
			}
			percentPosition += calculation;
			progressDelegate("Placing trees", "Determining basic position", percentPosition);
		}
		Place(progressDelegate);
	}
	
	
	private void Place(ProgressDelegate progressDelegate)
	{
		float percentPosition = 0.5f;
		progressDelegate("Placing trees", "Randomizing position", percentPosition);
		foreach(TreePosition tree in TreesPosition)
		{
			TreeInstance instance = new TreeInstance();

			Vector3 position  = RandomizeTreePosition(tree);
			//recalculate tree position height to real terrain
			position.y = TreeHeight(position);
			if (position.y < -5)
				continue;
			instance.position = position;
			instance.color = Color.white;
			instance.lightmapColor = Color.white;
			instance.prototypeIndex = tree.TreeIndex;

			instance.widthScale = TreeSize * (1f + Random.Range(-TreeVariation, TreeVariation));
			instance.heightScale = TreeSize * (1f + Random.Range(-WidthVariation, WidthVariation));
			instances.Add(instance);
			
			percentPosition += 0.00001f;
			progressDelegate("Placing trees", "Randomizing position", percentPosition);
		}
		Debug.Log("Tree placed " +instances.Count);
		terrainData.treeInstances = (TreeInstance[])instances.ToArray(typeof(TreeInstance));
	}
	
	
	private Vector3 RandomizeTreePosition(TreePosition tree)
	{
		//dimension of splatmaps
		int xDim = splatmap.GetUpperBound(0) + 1;
		int yDim = splatmap.GetUpperBound(1) + 1;
		
		TreeTextureSettings ts = TreeTextures[tree.TextureIndex];
		
		//relative one unit to terrain transform
		xRelativeUnit = 1 / (float)(xDim + 1);
		yRelativeUnit = 1 / (float)(yDim + 1);
		
		xRelativeUnit = (xRelativeUnit * 0.8f);
		yRelativeUnit = (yRelativeUnit * 0.8f);
		int tryPlace = 0;
		for ( ;;)
		{
			float xpos = (float)tree.PosX / (float)xDim; 
			float ypos = (float)tree.PosY / (float)yDim;
			
			xpos = xpos + Random.Range(-xRelativeUnit, xRelativeUnit);
			ypos = ypos + Random.Range(-yRelativeUnit, yRelativeUnit);
			
			//there is position ypos, 0, xpos
			//some wrong vector calculation
			Vector3 position  = new Vector3(ypos, 0 , xpos); 
			
			int xDimPos = (int)(xpos * xDim);
			int yDimPos = (int)(ypos * yDim);
			tryPlace++;
			
			if (tryPlace > 10)
				return position;
			
			if (xDimPos < 0 && xDimPos > splatmap.GetUpperBound(0))
				continue;
			if (yDimPos < 0 && yDimPos > splatmap.GetUpperBound(1))
				continue;
			try
			{
			if (splatmap[xDimPos, yDimPos, ts.Index] < 0.8f)
				continue;
			}
			catch
			{
				continue;
			}
			
			if (IsTreeClose(xpos, ypos, (xRelativeUnit * (float)ts.TreeDistance) * 0.1f))
				continue;
			
			return position;
			
		}
	}
	
	//recalculate tree according terrain
	private float TreeHeight(Vector3 treePosition)
	{
		//dimension of splatmaps
		int xDim = splatmap.GetUpperBound(0) + 1;
		int yDim = splatmap.GetUpperBound(1) + 1;
		
		//recalculate tree position to size of terrain
		float realPosX = treePosition.x * xDim * xUnit;
		float realPosY = treePosition.z * yDim * yUnit;
		 
		Vector3 realPosition = new Vector3(realPosX, 0, realPosY);
		float positionY = terrain.SampleHeight(realPosition) - 0.5f;
		if (positionY < MinimumHeight && MinimumHeight > 0)
		{
			return -10;
		}
		
		positionY = positionY / terrainData.size.y;
		return positionY;
	}
	
	private bool IsTreePositionClose(int xDim, int yDim, int treeUnitDistance, int textureIndex)
	{
		foreach(TreePosition position in TreesPosition)
		{
			if (xDim - treeUnitDistance < position.PosX && xDim + treeUnitDistance > position.PosX
			   	&& yDim - treeUnitDistance < position.PosY && yDim + treeUnitDistance > position.PosY)
			{
				return true;
			}
		}
		return false;
	}
	
	private bool IsTreeClose(float xDim, float yDim, float treeUnitDistance)
	{
		foreach(TreeInstance tree in instances)
		{
			if (xDim - treeUnitDistance < tree.position.x && xDim + treeUnitDistance > tree.position.x
			   	&& yDim - treeUnitDistance < tree.position.z && yDim + treeUnitDistance > tree.position.z)
			{
				return true;
			}
		}
		return false;
	}
}
