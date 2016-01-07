using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class PrefabFixer : MonoBehaviour 
{
    public void FixScale()
    {
        //select all children
        //select 1 child
        //change scale of 1 child to prefab
    }

    public void Fix(GameObject[] foundObjs)
    {
        GameObject tempObj = null;

        if (foundObjs != null)
        {
            foreach (Transform t in transform)
            {
                Debug.Log(t.name);
                if(t != null)
                {
                    if (t.tag == "Replace")
                    {

                        for (int i = 0; i < foundObjs.Length; i++)
                        {

                            if (t != null)
                            {
                                if (foundObjs[i].name == t.name)
                                {
                                    if (tempObj != null) { tempObj = null; }

                                    #if UNITY_EDITOR
                                    tempObj = (GameObject)PrefabUtility.InstantiatePrefab(foundObjs[i]);
                                    #endif

                                    tempObj.transform.position = t.position;
                                    tempObj.transform.parent = t.parent;
                                    DestroyImmediate(t.gameObject);
                                    break;
                                }
                            }

                        }

                    }
                }

                if (t != null) 
                { 
                    if (t.tag == "Huizen")
                    {
                        
                        for (int i = 0; i < foundObjs.Length; i++)
                        {
                            string temp = t.GetComponent<Gebouw>().buildingId;

                                if (foundObjs[i].name == t.name)
                                {
                                    if (tempObj != null) { tempObj = null; }

                                    #if UNITY_EDITOR
                                    tempObj = (GameObject)PrefabUtility.InstantiatePrefab(foundObjs[i]);
                                    #endif

                                    if (tempObj.GetComponent<Gebouw>())
                                    {
                                        tempObj.GetComponent<Gebouw>().buildingId = temp;
                                        tempObj.transform.position = t.position;
                                        tempObj.transform.rotation = t.rotation;
                                        tempObj.transform.parent = t.parent;
                                        tempObj.transform.localScale = new Vector3(1, 1, 1);
                                        DestroyImmediate(t.gameObject);
                                        break;
                                        
                                    }
                                    else
                                    {
                                        tempObj.AddComponent<Gebouw>().buildingId = temp;
                                        tempObj.transform.position = t.position;
                                        tempObj.transform.rotation = t.rotation;
                                        tempObj.transform.parent = t.parent;
                                        tempObj.transform.localScale = new Vector3(1, 1, 1);
                                        DestroyImmediate(t.gameObject);
                                        break;
                                    }
                                }
                            }


                       }


                    }
                }
            }
        }
}
