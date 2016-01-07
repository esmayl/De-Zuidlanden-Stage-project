using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;

public enum ImportType
{
    Woning = 0,
    Natuur = 1,
    Wegen =2
}

public class BuildingPlacerWindow : EditorWindow {

    public static string path = "Selecteer een pad alstublieft.";
    public static string dest = "Selecteer een pad alstublieft.";
    public string id="0080200000490690";
    public static Gebouw script;
    public static string[] ids;
    public static Dictionary<string, Info> dict = new Dictionary<string, Info>();
    public RenderTexture previewImage;
    public static GameObject[] loadedObjects;
    public static string[] loadedNames;
    static int i = 0;
    static int indexX;
    static int indexY;
    static bool loaded = false;

    private List<string> sortingList;
    private bool copied = false;
    private string[] finalArray;
    private static int[] selectedIdPosibilities;
    private int[] posibilities = {2,3,4,5,6};
    private string[] names = {"Gebied 2","Gebied 3","Gebied 4","Gebied 5","Gebied 6"};
    private GameObject previewObject;
    private GameObject tempCam;
    private bool preview =false;
    private Vector2 scroll = new Vector2();
    private Vector2 idScroll = new Vector2(0,0);
    private int selected = 0;
    private int selectedId = 0;
    private int selectedArea = 2;
    private int amount;
    private string root = Application.dataPath;
    private static string xmlPath = "table_export_adressen_Leeuwarden.xml";

    [MenuItem("File/Gebouwen Plaatsen")]
    static void Init()
    {
        BuildingPlacerWindow window = (BuildingPlacerWindow)EditorWindow.GetWindow(typeof(BuildingPlacerWindow));
        window.minSize = new Vector2(340f,300f);
        window.title = "Gebouwen Plaatsen";
        
        loadedObjects = Resources.LoadAll<GameObject>("Huizen");

        if (!loaded)
        {
            #region Download from internet
            //if (Network.TestConnection() != ConnectionTesterStatus.Error)
            //{
            //    WebRequest webTest = WebRequest.Create("http://www.jellyfishdm.com/table_export_adressen_Leeuwarden.xml");
            //    WebResponse download = webTest.GetResponse();
            //    XmlSerializer serial = new XmlSerializer(typeof(Records));
            //    Stream stream = download.GetResponseStream();
            //    Records tempRec = (Records)serial.Deserialize(stream);
            //
            //    ids = new string[tempRec.rows.Length];
            //    selectedIdPosibilities = new int[ids.Length];
            //
            //    for (i = 0; i < ids.Length; i++)
            //    {
            //        if (!dict.ContainsKey(tempRec.rows[i].adresId))
            //        {
            //                selectedIdPosibilities[i] = i;
            //                ids[i] = tempRec.rows[i].adresId;
            //                dict.Add(tempRec.rows[i].adresId, new Info(tempRec.rows[i].adres, tempRec.rows[i].postCode, tempRec.rows[i].woonPlaats, new Vector3(float.Parse(tempRec.rows[i].x) - 125000, 0f, float.Parse(tempRec.rows[i].y) - 500000)));
            //        }
            //        else
            //        {
            //            Debug.Log("Duplicate Key! : " + tempRec.rows[i].adresId);
            //        }
            //    }
            //}
            #endregion
            #region Open Localy
            //else
            //{
                string pathString = Application.dataPath + "/" + xmlPath;
                FileStream stream = File.OpenRead(pathString);
                XmlSerializer serial = new XmlSerializer(typeof(Records));
                Records tempRec = (Records)serial.Deserialize(stream);

                ids = new string[tempRec.rows.Length];
                selectedIdPosibilities = new int[ids.Length];


                for (i = 0; i < ids.Length; i++)
                {
                    if (!dict.ContainsKey(tempRec.rows[i].adresId))
                    {
                            indexX = tempRec.rows[i].x.IndexOf(",");
                            indexY = tempRec.rows[i].y.IndexOf(",");
                        
                            selectedIdPosibilities[i] = i;
                            ids[i] = tempRec.rows[i].adresId;

                            Vector3 pos = new Vector3();
                            
                            if (indexX != -1)
                            {
                                pos.x = float.Parse(tempRec.rows[i].x.Substring(0, indexX));
                            }
                            else
                            {
                                indexX = tempRec.rows[i].x.Length;
                            }
                            
                            if (indexY != -1)
                            {
                                pos.z = float.Parse(tempRec.rows[i].y.Substring(0, indexY));
                            }
                            else
                            {
                                indexY = tempRec.rows[i].y.Length;
                            }
                            pos.y = 0;

                            pos.x = pos.x - 125000;
                            pos.z = pos.z - 500000;
                             
                            Info info = new Info(tempRec.rows[i].adres, tempRec.rows[i].postCode, tempRec.rows[i].woonPlaats, pos);
                        
                            dict.Add(tempRec.rows[i].adresId, info);
                   }
                }
                loaded = true;
            //}
            #endregion
        }
        

        script = (Gebouw)Resources.Load("Scripts/Gebouw.cs");

        loadedNames = new string[loadedObjects.Length];
        for (int p = 0; p < loadedObjects.Length; p++)
        {
            loadedNames[p] = loadedObjects[p].name;
        }
    }

    void OnGUI()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.control)
        {
            id = EditorGUIUtility.systemCopyBuffer;
        }

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Object selecteren"))
        {
            string temp = EditorUtility.OpenFilePanel("Object selecteren", root, "fbx");
            Debug.Log(temp);
            path = temp;
        }

        if (GUILayout.Button("Pad selecteren"))
        {
            string temp = EditorUtility.OpenFolderPanel("Pad selecteren", root, "");
            Debug.Log(temp);
            dest = temp;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Object locatie: "+path);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Importeer locatie: " + dest);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Importeren"))
        {
            int p;

            //Directory.CreateDirectory(dest+"/"+path.Substring(path.LastIndexOf("/")));
            //File.Copy(path, dest + "/" + path.Substring(path.LastIndexOf("/"))+"/" + path.Substring(path.LastIndexOf("/")));
            //AssetDatabase.ImportAsset(dest + "/" + path.Substring(path.LastIndexOf("/")), ImportAssetOptions.Default);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();

            loadedObjects = new GameObject[Resources.LoadAll<GameObject>("Huizen").Length];
            loadedObjects = Resources.LoadAll<GameObject>("Huizen");

            loadedNames = new string[loadedObjects.Length];
            Dictionary<string,GameObject> dictTemp = new Dictionary<string,GameObject>();
            for (p = 0; p < loadedObjects.Length; p++)
            {
                loadedNames[p] = loadedObjects[p].name;
                if (!dictTemp.ContainsKey(loadedObjects[p].name))
                {
                    dictTemp.Add(loadedObjects[p].name, loadedObjects[p]);
                    MonoBehaviour.print("Unfilterd: " + loadedNames[p]);
                }
            }
            

            loadedNames = new string[dictTemp.Count];

            p = 0;

            foreach (KeyValuePair<string, GameObject> k in dictTemp)
            {
                loadedNames[p] = k.Key;
                MonoBehaviour.print("Filterd: " + loadedNames[p]);
                p++;
            }
           
            
        }
        //select obj from array
        GUILayout.Label("1. Selecteer een gebouw.");
        scroll = GUILayout.BeginScrollView(scroll);

        GUILayout.BeginVertical();
        selected = GUILayout.SelectionGrid(selected, loadedNames,1);
        GUILayout.EndVertical();


        GUILayout.EndScrollView();
            //W.I.P multiple objects plaatsen
            //GUILayout.BeginHorizontal();
            //GUILayout.Label("Aantal te plaatsen: ");
            //amount = EditorGUILayout.IntField(amount);
            //GUILayout.EndHorizontal();    

            GUILayout.Space(Screen.height / 32);            

            GUILayout.BeginHorizontal();
                GUILayout.Label("2. Vul het verblijfsID in: ");
                id = GUILayout.TextField(id, 16);

                GUILayout.Space(Screen.width / 32);
            GUILayout.EndHorizontal();
            
            selectedArea = EditorGUILayout.IntPopup(selectedArea, names, posibilities);

            
            //instantiate camera looking at obj
            if (GUILayout.Button("3. Plaats in de scene"))
            {
                if(previewObject != null){DestroyImmediate(previewObject);}
                previewObject = PrefabUtility.InstantiatePrefab(loadedObjects[selected]) as GameObject;

                Vector3 tempPos = FindBuilding(id).position;
                tempPos += GameObject.Find("Huizen Stuk " + selectedArea).transform.position;
                tempPos.y = 0;

                previewObject.transform.position = tempPos;

                Selection.activeGameObject = previewObject;
                SceneView.FrameLastActiveSceneViewWithLock();
                SceneView.lastActiveSceneView.pivot = previewObject.transform.position - previewObject.transform.forward;
                SceneView.lastActiveSceneView.LookAt(previewObject.transform.position);
                preview = true;
            }

            GUILayout.FlexibleSpace();

            if (preview)
            {
                Vector3 tempPos =previewObject.transform.position;

                GUILayout.BeginHorizontal();
            
                GUILayout.BeginVertical();
                GUILayout.Label("Positie");
                tempPos.x = EditorGUILayout.FloatField("X: ", tempPos.x);
                tempPos.y = EditorGUILayout.FloatField("Hoogte: ", tempPos.y);
                tempPos.z = EditorGUILayout.FloatField("Z: ", tempPos.z);
                GUILayout.EndVertical();
                previewObject.transform.position = tempPos;

                previewObject.transform.rotation = Quaternion.Euler(previewObject.transform.rotation.eulerAngles.x, EditorGUILayout.FloatField("Rotatie: ", previewObject.transform.rotation.eulerAngles.y), previewObject.transform.rotation.eulerAngles.z);
                GUILayout.EndHorizontal();
            }


                if (GUILayout.Button("4. Opslaan"))
                {
                    GameObject parent = GameObject.Find("Huizen Stuk "+selectedArea.ToString());

                    if (!previewObject.GetComponent<Gebouw>())
                    {
                        previewObject.AddComponent<Gebouw>();
                    }

                    previewObject.GetComponent<Gebouw>().buildingId = id;
                    previewObject.transform.parent = parent.transform;
                    previewObject.tag = "Huizen";
                    previewObject = new GameObject();
                    //save scene.
                }

        GUILayout.Space(Screen.width / 32);
        EditorGUILayout.EndVertical();
        
    }


    static string[][] SplitDictionary(Dictionary<string, Info> dict)
    {
        string[][] splitArrays = new string[6][];
        string[] tempArray;
        int i = 0;

        foreach (Info info in dict.Values)
        {
            if (i < dict.Count/10)
            {
                tempArray = new string[dict.Count / 10];
                tempArray[i] = ""+selectedIdPosibilities[i];
                splitArrays[0] = tempArray;
                i++;
                Debug.Log(i);
            }
            if (i < dict.Count / 8)
            {
                tempArray = new string[dict.Count / 10];
                tempArray[i] = "" + selectedIdPosibilities[i];
                splitArrays[1] = tempArray;
                i++;
                Debug.Log(i);
            }
            if (i < dict.Count / 6)
            {
                tempArray = new string[dict.Count / 10];
                tempArray[i] = "" + selectedIdPosibilities[i];
                splitArrays[2] = tempArray;
                i++;
                Debug.Log(i);
            }
            if (i < dict.Count / 4)
            {
                tempArray = new string[dict.Count / 10];
                tempArray[i] = "" + selectedIdPosibilities[i];
                splitArrays[3] = tempArray;
                i++;
                Debug.Log(i);
            }
            if (i < dict.Count / 2)
            {
                tempArray = new string[dict.Count / 10];
                tempArray[i] = "" + selectedIdPosibilities[i];
                splitArrays[4] = tempArray;
                i++;
                Debug.Log(i);
            }
            if (i < dict.Count /1.5f)
            {
                tempArray = new string[dict.Count/10];
                tempArray[i] = "" + selectedIdPosibilities[i];
                splitArrays[5] = tempArray;
                i++;
                Debug.Log(i);
            }
            if (i < dict.Count)
            {
                tempArray = new string[dict.Count/10];
                tempArray[i] = "" + selectedIdPosibilities[i];
                splitArrays[6] = tempArray;
                i++;
                Debug.Log(i);
            }
            
            
        }
        return splitArrays;
    }

    Info FindBuilding(string Id)
    {
        Info result = new Info();

        if (dict.ContainsKey(Id))
        {
            foreach (KeyValuePair<string, Info> pair in dict)
            {
                if (pair.Key == Id)
                {
                    result.adres = pair.Value.adres;
                    result.postcode = pair.Value.postcode;
                    result.position = pair.Value.position;
                    result.woonplaats = pair.Value.woonplaats;
                    return result;
                }
            }
        }
        else
        {
            Debug.LogError("Not found");
            return result;
        }

        return new Info();
    }

    void OnDestroy()
    {
        DestroyImmediate(previewObject);
    }
}
