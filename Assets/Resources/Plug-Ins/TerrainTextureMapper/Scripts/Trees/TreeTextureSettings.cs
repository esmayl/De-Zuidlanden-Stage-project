using UnityEngine;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[XmlInclude(typeof(TreeSettings))]
public class TreeTextureSettings
{
	public int TreeDistance;
	public int Index;
	public string TextureName;
	public bool Randomize;
	public bool Display;
	public float AlphamapValue;
	public List<TreeSettings> Trees;
	
	[XmlIgnore] 
	int count;
	
	[XmlIgnore] 
	List<TreeSettings> tempTrees;
	
	public TreeTextureSettings()
	{
		Trees = new List<TreeSettings>();
		Randomize = true;
		TreeDistance = 5;
		AlphamapValue = 0.8f;
		tempTrees = new List<TreeSettings>();
	}
	
	public bool IsUsed
	{
		get
		{
			foreach(TreeSettings t in Trees)
			{
				if (t.IsUsed && TreeDistance > 0)
					return true;
			}
			return false;
		}
	}
	
	public void CheckAllTrees()
	{
		foreach(TreeSettings tree in Trees)
		{
			tree.IsUsed = true;
		}
	}
	
	public void UncheckAllTrees()
	{
		foreach(TreeSettings tree in Trees)
		{
			tree.IsUsed = false;
		}
	}
	
	public void Prepare()
	{
		tempTrees = new List<TreeSettings>();
		foreach(TreeSettings ts in Trees)
		{
			if (ts.IsUsed)
				tempTrees.Add(ts);
		}
		count = tempTrees.Count;
	}
	
	//return tree count for current texture
	private int TreeCount
	{
		get
		{
			int result = 0;
			foreach(TreeSettings tree in Trees)
			{
				if (tree.IsUsed)
					result++;
			}
			return result;
		}
	}
	
	//return which tree will be used for this texture
	public int DetermineTreePrototype
	{
		get
		{
			if (!IsUsed)
				return 0;
			
			int random = Random.Range(1, count);
			random--;
			try
			{
				return tempTrees[random].Index;
			}
			catch
			{
				return 0;
			}
		}
	}
}
