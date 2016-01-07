using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Combine : MonoBehaviour {
    public MeshFilter[] meshFilters;
    public CombineInstance[] combine;
    public CombineInstance[] combine2;
	public int vertCounter;

	void Start () {
		meshFilters = GetComponentsInChildren<MeshFilter>();
		combine = new CombineInstance[meshFilters.Length];
        combine2 = new CombineInstance[meshFilters.Length];
		int i = 0;
        int p = 0;
		while (i < meshFilters.Length) {
            if (vertCounter <= 64000)
            {
                vertCounter += meshFilters[i].mesh.vertexCount;
                combine[i].mesh = Instantiate(meshFilters[i].mesh) as Mesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.renderer.enabled = false;
                i++;
            }
            if (vertCounter >= 64000)
            {
                combine2[p].mesh = Instantiate(meshFilters[i].mesh) as Mesh;
                combine2[p].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.renderer.enabled = false;
                i++;
                p++;
            }
                
		}

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, false,true);
        transform.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Decal"));
        transform.gameObject.SetActive(true);
	}

}
